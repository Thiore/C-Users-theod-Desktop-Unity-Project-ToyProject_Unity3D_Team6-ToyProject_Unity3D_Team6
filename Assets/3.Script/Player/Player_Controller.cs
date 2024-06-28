using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    private bool isDead;

    public bool IsDead { get => isDead; set { isDead = value;} }

    private void Awake()
    {
        isDead = false;
    }
}
