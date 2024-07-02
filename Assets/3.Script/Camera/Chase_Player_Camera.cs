using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase_Player_Camera : MonoBehaviour
{
    [SerializeField] private Transform Mango;
    [SerializeField] private Transform Runa;

    [SerializeField] private LayerMask Obstacle;

    private AudioSource audio;
    private Transform Player;
    public AudioClip dieClip;

    private GameObject CullingObject = null;
    [SerializeField] private float CullingAlpha = 0.3f;

    [Range(20,60)]
    [SerializeField] private float CameraPosY = 40f;
    [Range(10, 40)]
    [SerializeField] private float CameraPosZ = 25f;
    [Range(10, 80)]
    [SerializeField] private float CameraRotX = 50f;

    private bool isDead = false;
    private void Start()
    {
        GameManager.instance.Score = 0;
        if (GameManager.instance.SelectPlayer.Equals(ePlayer.Mango))
        {
            Debug.Log(GameManager.instance.SelectPlayer);
            Player = Mango;
            Runa.gameObject.SetActive(false);
            Destroy(Runa.gameObject);
            
        }
        else
        {
            Player = Runa;
            Mango.gameObject.SetActive(false);
            Destroy(Mango.gameObject);
        }
        Cursor.lockState = CursorLockMode.Confined;
        //Debug.Log("나안불림");
        audio = GetComponent<AudioSource>();
        isDead = false;
    }

    private void Update()
    {
        Quaternion PlayerRot = Player.transform.rotation;
        Quaternion CameraAngleX = Quaternion.Euler(CameraRotX, 0, 0);

        transform.rotation = PlayerRot * CameraAngleX;

        Vector3 playerPos = Player.transform.position;
        playerPos += (Player.transform.up.normalized)* CameraPosY;
        playerPos -= (Player.transform.forward.normalized)* CameraPosZ;

        transform.position = playerPos;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, Vector3.Distance(Player.position, transform.position), Obstacle))
        {
            if(CullingObject == null)
            {
                CullingObject = hit.transform.gameObject;
                
            }
            Color DownAlpha = CullingObject.GetComponentInChildren<Renderer>().material.color;
            DownAlpha.a = Mathf.Lerp(DownAlpha.a, CullingAlpha, Time.deltaTime);
            
            CullingObject.GetComponentInChildren<Renderer>().material.color = DownAlpha;
        }
        else
        {
            if (CullingObject != null)
            {
                CullingObject.GetComponentInChildren<Renderer>().material.color = Color.white;               
                CullingObject = null;
            }

        }
        Debug.DrawRay(transform.position, transform.forward * Vector3.Distance(Player.position, transform.position) , Color.red);


        DeadClip();


    }
    private void DeadClip()
    {
        if (Player.gameObject.GetComponent<Player_Health>().isDie)
        {
            if (!isDead)
            {
                audio.Stop();
                audio.PlayOneShot(dieClip);
                
                isDead = true;
            }
           
        }
    }
}
