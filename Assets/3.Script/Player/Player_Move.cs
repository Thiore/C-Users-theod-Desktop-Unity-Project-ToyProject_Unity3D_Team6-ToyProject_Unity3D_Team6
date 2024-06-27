using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Move : MonoBehaviour
{
    [SerializeField]public float speed = 10f;
    private float hAxis;
    private float vAxis;
    Vector3 moveVec;

    private void Update()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        transform.position += moveVec * speed*Time.deltaTime;
    }
}
