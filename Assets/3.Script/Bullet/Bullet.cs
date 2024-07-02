using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public int Damage { get => damage; }
    private Rigidbody Rigid;
    private void Awake()
    {
        Rigid = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "prop")
        {
            Rigid.velocity = Vector3.zero;
            gameObject.SetActive(false);
        }
        else if (collision.gameObject.tag == "Floor")
        {
            Rigid.velocity = Vector3.zero;
            gameObject.SetActive(false);
        }
        else if (collision.gameObject.tag == "Enemy")
        {
            Rigid.velocity = Vector3.zero;
            //Debug.Log("±¦Âú´Ï?...");
            gameObject.SetActive(false);
        }



    }
}
