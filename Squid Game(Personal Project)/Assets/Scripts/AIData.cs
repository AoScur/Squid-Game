using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AIData", menuName = "Scriptable/AIData")]
public class AIData : ScriptableObject
{
    public float speed = 5f;
    public float mass = 1f;
    public float power = 5f;
    public int aiType = -1;
}
