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
    public Text[] actionTexts;               // 行動名テキスト

    [Header("データ")]
    public ActionData[] actionDataList;       // 行動データ（アイコン＋名前）
    private int currentIndex = 2;             // 現在中央にあるIndex
    private bool isRotating = false;          // 回転中フラグ
    private float rotationDuration = 0.2f;    // 回転アニメ時間

    private Vector3[] initPos;                // 初期座標保持

    void Start()
    {
        initPos = new Vector3[actionImages.Length];
        for (int i = 0; i < actionImages.Length; i++)
        {
            initPos[i] = actionImages[i].rectTransform.anchoredPosition;
            actionImages[i].sprite = actionDataList[i].actionIcon;
            actionTexts[i].text = actionDataList[i].actionName;
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

                Color textColor = actionTexts[i].color;
                textColor.a = Mathf.Lerp(startAlphas[i], targetAlpha, t);
                actionTexts[i].color = textColor;

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
        var lastText = actionTexts[actionTexts.Length - 1];

        for (int i = actionImages.Length - 1; i > 0; i--)
        {
            actionImages[i] = actionImages[i - 1];
            actionDataList[i] = actionDataList[i - 1];
            actionTexts[i] = actionTexts[i - 1];
        }

        actionImages[0] = lastImage;
        actionDataList[0] = lastData;
        actionTexts[0] = lastText;
    }

    void RotateArraysLeft()
    {
        var firstImage = actionImages[0];
        var firstData = actionDataList[0];
        var firstText = actionTexts[0];

        for (int i = 0; i < actionImages.Length - 1; i++)
        {
            actionImages[i] = actionImages[i + 1];
            actionDataList[i] = actionDataList[i + 1];
            actionTexts[i] = actionTexts[i + 1];
        }

        actionImages[actionImages.Length - 1] = firstImage;
        actionDataList[actionDataList.Length - 1] = firstData;
        actionTexts[actionTexts.Length - 1] = firstText;
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
        // 必要なら中央に出てる行動の詳細説明とかもここで更新
    }

    void UpdateImageAppearance(int i)
    {
        if (i == currentIndex)
        {
            actionImages[i].transform.localScale = new Vector3(1.5f, 1.5f, 1);
            actionImages[i].color = new Color(1f, 1f, 1f, 1.0f);
            actionTexts[i].color = new Color(0f, 0f, 0f, 1.0f);
        }
        else if (i == (currentIndex - 1 + actionImages.Length) % actionImages.Length || i == (currentIndex + 1) % actionImages.Length)
        {
            actionImages[i].transform.localScale = Vector3.one;
            actionImages[i].color = new Color(1f, 1f, 1f, 0.5f);
            actionTexts[i].color = new Color(0f, 0f, 0f, 0.5f);
        }
        else
        {
            actionImages[i].transform.localScale = Vector3.one;
            actionImages[i].color = new Color(1f, 1f, 1f, 0f);
            actionTexts[i].color = new Color(0f, 0f, 0f, 0f);
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

        // 選択に応じた次処理（例えば、スキルホイールを開くとか）を書く
    }
}

