using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType
{
    Attack,
    Skill,
    Defend,
    Item,
    Special
}

[System.Serializable]
[CreateAssetMenu(fileName = "ActionData",menuName = "GameData/ActionData")]
public class ActionData : ScriptableObject
{
    public Sprite actionIcon;
    public string actionName;
    public ActionType actionType;
}
