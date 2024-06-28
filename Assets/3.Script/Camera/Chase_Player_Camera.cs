using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase_Player_Camera : MonoBehaviour
{
    [SerializeField] private Transform Player;
    [Range(20,60)]
    [SerializeField] private float CameraPosY = 40f;
    [Range(10, 40)]
    [SerializeField] private float CameraPosZ = 25f;
    [Range(10, 80)]
    [SerializeField] private float CameraRotX = 50f;


    private void Update()
    {
        Quaternion PlayerRot = Player.transform.rotation;
        Quaternion CameraAngleX = Quaternion.Euler(CameraRotX, 0, 0);

        transform.rotation = PlayerRot * CameraAngleX;

        Vector3 playerPos = Player.transform.position;
        playerPos += (Player.transform.up.normalized)* CameraPosY;
        playerPos -= (Player.transform.forward.normalized)* CameraPosZ;

        transform.position = playerPos;

        

    }
}
