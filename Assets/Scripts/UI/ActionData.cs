using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActionData",menuName = "GameData/ActionData")]
public class ActionData : ScriptableObject
{
    public Sprite actionIcon;
    public string actionName;
}
