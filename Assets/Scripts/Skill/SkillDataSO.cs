using UnityEngine;

[CreateAssetMenu(fileName = "SkillData",menuName = "GameData/SkillData")]
public class SkillDataSO : ScriptableObject
{
    public string skillName;         // スキル名
    public string skillText;
    public float power;              // 威力倍率
    public int range;                // 射程（例：何マス先まで届くか）
    public int mpCost;               // 消費MP
    public SkillMold skillMold;      // スキルの型（物理、魔法）
    public SkillType skillType;      // スキルの種類（属性とか）
    public Sprite icon; // スキル表示用のアイコン



    public enum SkillMold
    {
        physics,
        magic
    }

    public enum SkillType
    {
        Physical,   //無属性
        Fire,       //火属性
        Water,      //水属性
        Lightning,  //雷属性
        Earth,      //地属性
        Dark,       //闇属性
        Light,      //光属性
    }
}