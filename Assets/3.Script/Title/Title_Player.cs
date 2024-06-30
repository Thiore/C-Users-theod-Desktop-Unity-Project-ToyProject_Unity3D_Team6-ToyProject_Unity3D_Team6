using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Title_Player : MonoBehaviour
{
    private Animator Anim;

    private float NextAnim;
    private bool isSelect = false;
    private ePlayer selectPlayer;

    private void Awake()
    {
        Anim = GetComponent<Animator>();
        isSelect = false;
    }

    private void Update()
    {

    }


}
