using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Enemy_data", fileName = "Enemy_Data")]
public class Enemy_Data : ScriptableObject
{
    public float MaxHp = 10f;
    public float damage = 2;
    public float speed = 5f;
}
