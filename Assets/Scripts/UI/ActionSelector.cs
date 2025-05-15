using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 行動選択ホイール
/// 通常攻撃／スキル／防御／アイテム／スペシャル
/// </summary>
public class ActionWheelController : MonoBehaviour
{
    [Header("UI関連")]
    public Image[] actionImages;             // アイコン画像
    public Text currentText;

    public SkillWheelController skillWheelController;

    [Header("データ")]
    public ActionData[] actionDataList;       // 行動データ（アイコン＋名前）
    private int currentIndex = 2;             // 現在中央にあるIndex
    private bool isRotating = false;          // 回転中フラグ
    private float rotationDuration = 0.2f;    // 回転アニメ時間

    private Vector3[] initPos;                // 初期座標保持
    private int currentTension;

    void Start()
    {
        initPos = new Vector3[actionImages.Length];
        for (int i = 0; i < actionImages.Length; i++)
        {
            initPos[i] = actionImages[i].rectTransform.anchoredPosition;
            actionImages[i].sprite = actionDataList[i].actionIcon;
        }
        StartDisplay();
    }

    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f && !isRotating)
        {
            RotateWheel(1);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f && !isRotating)
        {
            RotateWheel(-1);
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isRotating)
        {
            ConfirmSelection();
        }
    }

    void RotateWheel(int direction)
    {
        if (isRotating) return;
        isRotating = true;
        StartCoroutine(RotateWheelCoroutine(direction));
    }

    IEnumerator RotateWheelCoroutine(int direction)
    {
        float elapsedTime = 0f;

        Vector2[] startPos = new Vector2[actionImages.Length];
        Vector3[] startScales = new Vector3[actionImages.Length];
        float[] startAlphas = new float[actionImages.Length];

        for (int i = 0; i < actionImages.Length; i++)
        {
            startPos[i] = actionImages[i].rectTransform.anchoredPosition;
            startScales[i] = actionImages[i].transform.localScale;
            startAlphas[i] = actionImages[i].color.a;
        }

        while (elapsedTime < rotationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / rotationDuration;

            for (int i = 0; i < actionImages.Length; i++)
            {
                int targetIndex = (i + direction + actionImages.Length) % actionImages.Length;

                int relativeIndex = (targetIndex - currentIndex + actionImages.Length) % actionImages.Length;

                float targetAlpha = (relativeIndex == 0) ? 1f : (relativeIndex == 1 || relativeIndex == actionImages.Length - 1) ? 0.5f : 0f;
                Vector3 targetScale = (targetIndex == currentIndex) ? new Vector3(1.5f, 1.5f, 1) : Vector3.one;

                actionImages[i].rectTransform.anchoredPosition = Vector2.Lerp(startPos[i], initPos[targetIndex], t);

                Color color = actionImages[i].color;
                color.a = Mathf.Lerp(startAlphas[i], targetAlpha, t);
                actionImages[i].color = color;

                actionImages[i].transform.localScale = Vector3.Lerp(startScales[i], targetScale, t);
            }

            yield return null;
        }

        if (direction == 1)
        {
            RotateArraysRight();
        }
        else
        {
            RotateArraysLeft();
        }

        isRotating = false;
        UpdateDisplay();
    }

    void RotateArraysRight()
    {
        var lastImage = actionImages[actionImages.Length - 1];
        var lastData = actionDataList[actionDataList.Length - 1];

        for (int i = actionImages.Length - 1; i > 0; i--)
        {
            actionImages[i] = actionImages[i - 1];
            actionDataList[i] = actionDataList[i - 1];
        }

        actionImages[0] = lastImage;
        actionDataList[0] = lastData;
    }

    void RotateArraysLeft()
    {
        var firstImage = actionImages[0];
        var firstData = actionDataList[0];

        for (int i = 0; i < actionImages.Length - 1; i++)
        {
            actionImages[i] = actionImages[i + 1];
            actionDataList[i] = actionDataList[i + 1];
        }

        actionImages[actionImages.Length - 1] = firstImage;
        actionDataList[actionDataList.Length - 1] = firstData;
    }

    void StartDisplay()
    {
        for (int i = 0; i < actionImages.Length; i++)
        {
            initPos[i] = actionImages[i].rectTransform.anchoredPosition;
            UpdateImageAppearance(i);
        }

        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        currentText.text = actionDataList[currentIndex].actionName;
    }

    void UpdateImageAppearance(int i)
    {
        if (i == currentIndex)
        {
            actionImages[i].transform.localScale = new Vector3(1.5f, 1.5f, 1);
            actionImages[i].color = new Color(1f, 1f, 1f, 1.0f);
        }
        else if (i == (currentIndex - 1 + actionImages.Length) % actionImages.Length || i == (currentIndex + 1) % actionImages.Length)
        {
            actionImages[i].transform.localScale = Vector3.one;
            actionImages[i].color = new Color(1f, 1f, 1f, 0.5f);
        }
        else
        {
            actionImages[i].transform.localScale = Vector3.one;
            actionImages[i].color = new Color(1f, 1f, 1f, 0f);
        }
    }

    public ActionData GetCurrentActionData()
    {
        return actionDataList[currentIndex];
    }

    void ConfirmSelection()
    {
        var selectedAction = GetCurrentActionData();
        Debug.Log($"選択した行動: {selectedAction.actionName}");

        switch (selectedAction.actionType)
        {
            case ActionType.Attack:
                Debug.Log("通常攻撃を選択 → 対象選択へ");
                // TODO: 対象選択UI呼び出し
                break;

            case ActionType.Skill:
                Debug.Log("スキルを選択 → スキルホイールを表示");
                skillWheelController.gameObject.SetActive(true);
                this.gameObject.SetActive(false);
                break;

            case ActionType.Defend:
                Debug.Log("防御 → 防御処理即実行");
                // TODO: 防御処理を実行してターン終了
                break;

            case ActionType.Item:
                Debug.Log("アイテム → アイテムホイール表示");
                // TODO: アイテム選択画面表示
                break;

            case ActionType.Special:
                if (IsTensionMax())
                {
                    Debug.Log("スペシャル発動！");
                    // TODO: スペシャルスキル処理呼び出し
                }
                else
                {
                    Debug.Log("テンションが足りません！");
                    // TODO: エラーUI表示など
                }
                break;
        }

        bool IsTensionMax()
        {
            // 仮に100がMAXだとする
            return currentTension >= 100;
        }
    }
}

