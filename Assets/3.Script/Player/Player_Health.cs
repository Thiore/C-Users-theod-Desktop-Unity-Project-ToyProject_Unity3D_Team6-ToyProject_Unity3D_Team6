using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_Health : MonoBehaviour
{
    [SerializeField] private int StartHealth = 3;  // ���� ü��
    public int CurrentHealth;  // ���� ü��
    private Animator playerAnimator;
    public bool isDie;
    
    

    private float behittime = 3f;
    private float belasthit;
    [SerializeField] private GameObject ScoreBoard;

    private void Awake()
    {
        belasthit = 0;
        CurrentHealth = StartHealth;  // ������ �� ���� ü���� ���� ü������ ����
        playerAnimator = GetComponentInChildren<Animator>();
        isDie = false;
       
    }
    private void Update()
    {
        if (isDie)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                RankingManager.instance.SaveRank();
                SceneManager.LoadScene("TitleScene");
            }
        }
        if (transform.position.y<-1f)
        {
            OnDamage(3);
        }
        
        
    }

    public void OnDamage(int damage)
    {
        if(CurrentHealth>=0)
        {
            if (!isDie)//isdie�� false�� ��쿡�� ondamage �޼��� ȣ��
            {
                CurrentHealth -= damage;  // �������� ������ ���� ü���� ����
                //Debug.Log("������");
            }
            else // isdie�� true�� ��� ��� ����
            {

                return;
            }

            if (CurrentHealth <= 0)
            {
                //RankingManager.instance.SaveRank();
                Die();  // ü���� 0 ���ϰ� �Ǹ� Die �޼��� ȣ��
                RankingManager.instance.SetRanking_Data();
                RankingManager.instance.SaveRank();
                FindObjectOfType<GameUI>().LoadBoard();
            }
        }
        
    }

    private void Die()
    {
        // �÷��̾ �׾��� �� ó��
        //Debug.Log("Player is dead!");
        // ���⼭ �÷��̾� ��� �� ó���� �߰��� �� �ֽ��ϴ�. ��: ���� ���� ȭ�� ǥ��, �÷��̾� ��Ȱ��ȭ ��
        playerAnimator.SetTrigger("Die");
        isDie = true;
        ScoreBoard.SetActive(true);
        



    }

    private void OnTriggerStay(Collider other)
    {
        if(!isDie && Time.time >= behittime + belasthit)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                belasthit = Time.time;
                //Debug.Log(belasthit);
                //Debug.Log(":��");
                OnDamage(1);
            }
        }

    }
}
