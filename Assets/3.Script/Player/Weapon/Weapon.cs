using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private enum Type { Melee, Range }
    [SerializeField] private Type type;
    [SerializeField] private int damage;
    [SerializeField] public float rate;
    [SerializeField] private BoxCollider meleeArea;
    [SerializeField] private GameObject trailEffect;
    public Transform bulletPoistion;
    public GameObject bullet;
    public Transform bulletCasePosition;
    public GameObject bulletCase;

    public int Damage { get => damage; }

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject bulletCasePrefab;
    private GameObject[] bulletPool;
    private GameObject[] bulletCasePool;
    private int poolSize = 10;
    private int nextBullet = 0;
    private int nextBulletCase = 0;

    private void Start()
    {
        //�ҷ� �ʱ�ȭ
        bulletPool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            bulletPool[i] = Instantiate(bulletPrefab);
            bulletPool[i].SetActive(false);
        }
        // ź�� �ʱ�ȭ
        bulletCasePool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            bulletCasePool[i] = Instantiate(bulletCasePrefab);
            bulletCasePool[i].SetActive(false);
        }
    }

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

    //private IEnumerator Shot()
    //{
    //    //�Ѿ� �߻�
    //    GameObject instantBullet = Instantiate(bullet, bulletPoistion.position, bulletPoistion.rotation);
    //        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
    //    bulletRigid.velocity = bulletPoistion.forward * 50;
    //    yield return null;

    //    //ź��
    //    GameObject instantCase = Instantiate(bulletCase, bulletCasePosition.position, bulletCasePosition.rotation);
    //    Rigidbody caseaRigid = instantCase.GetComponent<Rigidbody>();
    //    Vector3 caseVec = bulletCasePosition.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
    //    caseaRigid.AddForce(caseVec, ForceMode.Impulse);
    //    caseaRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);
    //}
    private IEnumerator Shot()
    {
        // ������Ʈ Ǯ���� �Ѿ� ��������
        GameObject bullet = GetNextBulletFromPool();
        GameObject bulletCase = GetNextBulletCaseFromPool();
        if (bullet != null)
        {
            bullet.transform.position = bulletPoistion.position;
            bullet.transform.rotation = bulletPoistion.rotation;
            bullet.SetActive(true);

            // �Ѿ� �߻� ����
            Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
            bulletRigidbody.velocity = bullet.transform.forward * 50;
        }

        // ź�� �߻� ����
        bulletCase = Instantiate(bulletCasePrefab, bulletCasePosition.position, bulletCasePosition.rotation);
        Rigidbody caseRigidbody = bulletCase.GetComponent<Rigidbody>();
        Vector3 caseForce = bulletCasePosition.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        caseRigidbody.AddForce(caseForce, ForceMode.Impulse);
        caseRigidbody.AddTorque(Vector3.up * 10, ForceMode.Impulse);

        yield return null;
    }

    private GameObject GetNextBulletFromPool()
    {
        // ���� ����� �Ѿ��� Ǯ���� ��������
        GameObject bullet = bulletPool[nextBullet];
        nextBullet = (nextBullet + 1) % poolSize;
        return bullet;
    }
    private GameObject GetNextBulletCaseFromPool()
    {
        // ���� ����� �Ѿ��� Ǯ���� ��������
        GameObject bulletCase = bulletCasePool[nextBulletCase];
        nextBulletCase = (nextBulletCase + 1) % poolSize;
        return bulletCase;
    }
    private void OnCollisionEnter(Collision collision)
    {
        // �Ѿ��� ���� �浹���� �� ��Ȱ��ȭ ó��
        if (collision.gameObject.CompareTag("prop"))
        {
            gameObject.SetActive(false); // ���� �Ѿ��� ��Ȱ��ȭ
                                         
        }
    }
}
