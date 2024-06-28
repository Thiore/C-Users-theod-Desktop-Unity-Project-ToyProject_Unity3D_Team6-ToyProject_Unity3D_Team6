using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase_Player_Camera : MonoBehaviour
{
    [SerializeField] private Transform Player;

    private void Update()
    {
        Quaternion PlayerRot = Player.transform.rotation;
        Quaternion CameraAngleX = Quaternion.Euler(50, 0, 0);

        transform.rotation = PlayerRot * CameraAngleX;

        Vector3 playerPos = Player.transform.position;
        playerPos += (Player.transform.up.normalized)*40f;
        playerPos -= (Player.transform.forward.normalized)*25f;

        transform.position = playerPos;

        

    }
}
