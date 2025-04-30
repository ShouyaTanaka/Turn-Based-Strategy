using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public string characterName; // キャラ名

    [Header("ステータス")]
    public int maxHP; // 最大体力
    public int currentHP; // 現在の体力
    public int maxMP; // 最大魔力
    public int currentMP; // 現在の魔力
    public int atk; // 物理攻撃力
    public int matk; // 魔法攻撃力
    public int def; // 物理防御力
    public int mdef; // 魔法防御力
    public int agi; // 速度

    public int critical; // クリ率
    public int criticalDmg; // クリダメ

    [Header("その他")]
    public Vector2Int position; // マス座標（ｘ,ｙ）
    public List<SkillDataSO> skills;


    public enum CharacterState
    {
        Idle, // 待機中
        SelectionAction, // 行動選択
        Acting, // 行動中
        End, // 行動終了
        Dead, // 死亡
    }
    public CharacterState state = CharacterState.Idle;

    /// <summary>
    /// ScriptableObjectから初期化する
    /// </summary>
    public virtual void Initialize(PlayerDataSO data)
    {
        characterName = data.characterName;
        maxHP = data.maxHP;
        currentHP = maxHP;
        maxMP = data.maxMP;
        currentMP = maxMP;
        atk = data.atk;
        def = data.def;
        agi = data.agi;
        skills = new List<SkillDataSO>(data.skills);

        Debug.Log($"{characterName} を初期化しました！");
    }

    /// <summary>
    /// 行動処理,行動選択など
    /// </summary>
    public abstract void ActionCharacter();

    /// <summary>
    /// ダメージ処理
    /// </summary>
    /// <param name="dmg">未計算ダメージ</param>
    public virtual void TakeDamage(int dmg)
    {
        int enDmg = dmg - def;
        currentHP -= enDmg;

        if(currentHP <= 0)
        {
            Die();
        }
    }
    /// <summary>
    /// 死亡処理
    /// </summary>
    protected virtual void Die()
    {
        state = CharacterState.Dead;
        // アニメーション、除外処理などここで
    }
}
