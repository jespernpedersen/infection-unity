using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Action
{
    public CharacterStates action;
    public Vector2 coordenate;
    public GameObject interactWith;
    public float duration;
}
