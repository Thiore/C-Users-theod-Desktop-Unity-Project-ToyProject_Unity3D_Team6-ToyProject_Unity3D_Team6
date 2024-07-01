using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation_Map : MonoBehaviour
{
    [SerializeField] GameObject Map;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }
    private void Update()
    {
        transform.RotateAround(Map.transform.position,Vector3.up,Time.deltaTime*10f);
    }
}
