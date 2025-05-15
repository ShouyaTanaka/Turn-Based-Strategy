using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// スキル選択ホイール：スクロールで回転してスペースで決定
/// </summary>
public class SkillWheelController : MonoBehaviour
{
    public Image[] skillIcons;                // スキルアイコン画像（UI）
    public SkillDataSO[] skillDataList;       // 所持スキル
    public Text descriptionText;              // スキル説明（任意）
    private int currentIndex = 2;             // 現在の中央インデックス
    public Text currentText;
    private bool isRotating = false;          // 回転中フラグ
    private float rotationDuration = 0.2f;    // 回転時間
    private Vector3[] initPos;                // 各UIの初期座標

    void Start()
    {
        gameObject.SetActive(false); // 最初は非表示
    }

    public void OpenSkillWheel(SkillDataSO[] skills)
    {
        skillDataList = skills;
        initPos = new Vector3[skillIcons.Length];

        for (int i = 0; i < skillIcons.Length; i++)
        {
            initPos[i] = skillIcons[i].rectTransform.anchoredPosition;
            skillIcons[i].sprite = skillDataList[i].icon;
        }

        StartDisplay();
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (!gameObject.activeSelf || isRotating) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
            RotateWheel(1);
        else if (scroll < 0f)
            RotateWheel(-1);

        if (Input.GetKeyDown(KeyCode.Space))
            ConfirmSkillSelection();
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

        Vector2[] startPos = new Vector2[skillIcons.Length];
        Vector3[] startScales = new Vector3[skillIcons.Length];
        float[] startAlphas = new float[skillIcons.Length];

        for (int i = 0; i < skillIcons.Length; i++)
        {
            startPos[i] = skillIcons[i].rectTransform.anchoredPosition;
            startScales[i] = skillIcons[i].transform.localScale;
            startAlphas[i] = skillIcons[i].color.a;
        }

        while (elapsedTime < rotationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / rotationDuration;

            for (int i = 0; i < skillIcons.Length; i++)
            {
                int targetIndex = (i + direction + skillIcons.Length) % skillIcons.Length;
                int relativeIndex = (targetIndex - currentIndex + skillIcons.Length) % skillIcons.Length;

                float targetAlpha = (relativeIndex == 0) ? 1f : (relativeIndex == 1 || relativeIndex == skillIcons.Length - 1) ? 0.5f : 0f;
                Vector3 targetScale = (targetIndex == currentIndex) ? new Vector3(1.5f, 1.5f, 1f) : Vector3.one;

                skillIcons[i].rectTransform.anchoredPosition = Vector2.Lerp(startPos[i], initPos[targetIndex], t);
                skillIcons[i].transform.localScale = Vector3.Lerp(startScales[i], targetScale, t);

                Color iconColor = skillIcons[i].color;
                iconColor.a = Mathf.Lerp(startAlphas[i], targetAlpha, t);
                skillIcons[i].color = iconColor;
            }

            yield return null;
        }

        if (direction == 1)
            RotateArraysRight();
        else
            RotateArraysLeft();

        isRotating = false;
        UpdateDisplay();
    }

    void RotateArraysRight()
    {
        var lastIcon = skillIcons[^1];
        var lastData = skillDataList[^1];

        for (int i = skillIcons.Length - 1; i > 0; i--)
        {
            skillIcons[i] = skillIcons[i - 1];
            skillDataList[i] = skillDataList[i - 1];
        }

        skillIcons[0] = lastIcon;
        skillDataList[0] = lastData;
    }

    void RotateArraysLeft()
    {
        var firstIcon = skillIcons[0];
        var firstData = skillDataList[0];

        for (int i = 0; i < skillIcons.Length - 1; i++)
        {
            skillIcons[i] = skillIcons[i + 1];
            skillDataList[i] = skillDataList[i + 1];
        }

        skillIcons[^1] = firstIcon;
        skillDataList[^1] = firstData;
    }

    void StartDisplay()
    {
        for (int i = 0; i < skillIcons.Length; i++)
            UpdateImageAppearance(i);

        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        descriptionText.text = skillDataList[currentIndex].skillText;
        currentText.text = skillDataList[currentIndex].skillName;
    }

    void UpdateImageAppearance(int i)
    {
        if (i == currentIndex)
        {
            skillIcons[i].transform.localScale = new Vector3(1.5f, 1.5f, 1);
            skillIcons[i].color = new Color(1f, 1f, 1f, 1.0f);
        }
        else if (i == (currentIndex - 1 + skillIcons.Length) % skillIcons.Length || i == (currentIndex + 1) % skillIcons.Length)
        {
            skillIcons[i].transform.localScale = Vector3.one;
            skillIcons[i].color = new Color(1f, 1f, 1f, 0.5f);
        }
        else
        {
            skillIcons[i].transform.localScale = Vector3.one;
            skillIcons[i].color = new Color(1f, 1f, 1f, 0f);
        }
    }

    void ConfirmSkillSelection()
    {
        SkillDataSO selectedSkill = GetCurrentSkill();
        Debug.Log($"スキル選択：{selectedSkill.skillName}");

        // 今後ここで対象選択へ進む処理を追加する！
        gameObject.SetActive(false);
    }

    SkillDataSO GetCurrentSkill()
    {
        return skillDataList[currentIndex];
    }
}
