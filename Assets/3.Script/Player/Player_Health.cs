using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Health : MonoBehaviour
{
    [SerializeField] private int StartHealth = 3;  // ���� ü��
    private int CurrentHealth;  // ���� ü��
    private Animator playerAnimator;
    public bool isDie = false;

    private void Start()
    {
        CurrentHealth = StartHealth;  // ������ �� ���� ü���� ���� ü������ ����
        playerAnimator = GetComponentInChildren<Animator>();
    }

    public void OnDamage(int damage)
    {
        CurrentHealth -= damage;  // �������� ������ ���� ü���� ����
        Debug.Log("������");
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
