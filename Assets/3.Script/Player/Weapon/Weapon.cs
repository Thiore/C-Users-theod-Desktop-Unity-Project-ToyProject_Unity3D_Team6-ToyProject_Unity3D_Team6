using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private enum Type { Melee, Range}
    [SerializeField]private Type type;
    [SerializeField] private int damage;
    [SerializeField] public float rate;
    [SerializeField] private BoxCollider meleeArea;
    [SerializeField] private GameObject trailEffect;
    public Transform bulletPoistion;
    public GameObject bullet;
    public Transform bulletCasePosition;
    public GameObject bulletCase;

    public int Damage { get => damage; }

    public void Use()
    {
        if (type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        else if (type == Type.Range)
        {
            StartCoroutine("Shot");
        }
    }

    private IEnumerator Swing()
    {
        meleeArea.enabled = true;
        trailEffect.SetActive(true);
        yield return new WaitForSeconds(0.7f);
        meleeArea.enabled = false;
        trailEffect.SetActive(false);
    }

    private IEnumerator Shot()
    {
        //ÃÑ¾Ë ¹ß»ç
        GameObject instantBullet = Instantiate(bullet, bulletPoistion.position, bulletPoistion.rotation);
            Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPoistion.forward * 50;
        yield return null;

        //ÅºÇÇ
        GameObject instantCase = Instantiate(bulletCase, bulletCasePosition.position, bulletCasePosition.rotation);
        Rigidbody caseaRigid = instantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePosition.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        caseaRigid.AddForce(caseVec, ForceMode.Impulse);
        caseaRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);
    }
    

}
