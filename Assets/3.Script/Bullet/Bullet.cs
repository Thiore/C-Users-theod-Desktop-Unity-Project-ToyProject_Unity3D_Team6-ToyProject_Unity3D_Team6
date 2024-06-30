using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public int Damage { get => damage; }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "prop")
        {
            gameObject.SetActive(false);
        }
        else if (collision.gameObject.tag == "Floor")
        {
            gameObject.SetActive(false);
        }
        else if (collision.gameObject.tag == "Enemy")
        {
            Debug.Log("±¦Âú´Ï?...");
            gameObject.SetActive(false);
        }



    }
}
