using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData",menuName = "GameData/PlayerData")]
public class PlayerDataSO : ScriptableObject
{
    public string characterName; // キャラ名
    public int maxHP; // 最大体力
    public int maxMP; // 最大魔力
    public int atk; // 物理攻撃力
    public int matk; // 魔法攻撃力
    public int def; // 物理防御力
    public int mdef; // 魔法防御力
    public int agi; // 速度

    public int critical; // クリ率
    public int criticalDmg; // クリダメ

    public List<SkillDataSO> skills;
}