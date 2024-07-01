using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Health : MonoBehaviour
{
    [SerializeField] private int StartHealth = 100;  // ���� ü��
    public int CurrentHealth;  // ���� ü��
    private Animator playerAnimator;
    public bool isDie;

    private void Awake()
    {
        CurrentHealth = StartHealth;  // ������ �� ���� ü���� ���� ü������ ����
        playerAnimator = GetComponentInChildren<Animator>();
        isDie = false;
    }

    public void OnDamage(int damage)
    {
        if (!isDie)//isdie�� false�� ��쿡�� ondamage �޼��� ȣ��
        {
            CurrentHealth -= damage;  // �������� ������ ���� ü���� ����
            Debug.Log("������");
        }
        else // isdie�� true�� ��� ��� ����
        {
            return;
        }
        
        if (CurrentHealth <= 0)
        {
            Die();  // ü���� 0 ���ϰ� �Ǹ� Die �޼��� ȣ��
        }
    }

    private void Die()
    {
        // �÷��̾ �׾��� �� ó��
        Debug.Log("Player is dead!");
        // ���⼭ �÷��̾� ��� �� ó���� �߰��� �� �ֽ��ϴ�. ��: ���� ���� ȭ�� ǥ��, �÷��̾� ��Ȱ��ȭ ��
        playerAnimator.SetTrigger("Die");
        isDie = true;

    }

    private void OnCollisionEnter(Collision collision)
    {
        // �浹�� ������Ʈ�� Enemy �±׸� ������ �ִ��� Ȯ��
        if (collision.gameObject.CompareTag("Enemy"))
        {
            OnDamage(1);  // �������� 1��ŭ ���� (�ʿ信 ���� ������ ���� ������ �� ����)
        }
    }
}
