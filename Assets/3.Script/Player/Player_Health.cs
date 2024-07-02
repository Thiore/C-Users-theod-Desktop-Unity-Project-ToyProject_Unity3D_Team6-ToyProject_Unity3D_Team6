using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_Health : MonoBehaviour
{
    [SerializeField] private int StartHealth = 3;  // 시작 체력
    public int CurrentHealth;  // 현재 체력
    private Animator playerAnimator;
    public bool isDie;
    
    

    private float behittime = 3f;
    private float belasthit;
    [SerializeField] private GameObject ScoreBoard;

    private void Awake()
    {
        belasthit = 0;
        CurrentHealth = StartHealth;  // 시작할 때 현재 체력을 시작 체력으로 설정
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
            if (!isDie)//isdie가 false일 경우에만 ondamage 메서드 호출
            {
                CurrentHealth -= damage;  // 데미지를 받으면 현재 체력을 감소
                //Debug.Log("데미지");
            }
            else // isdie가 true일 경우 즉시 리턴
            {

                return;
            }

            if (CurrentHealth <= 0)
            {
                //RankingManager.instance.SaveRank();
                Die();  // 체력이 0 이하가 되면 Die 메서드 호출
                RankingManager.instance.SetRanking_Data();
                RankingManager.instance.SaveRank();
                FindObjectOfType<GameUI>().LoadBoard();
            }
        }
        
    }

    private void Die()
    {
        // 플레이어가 죽었을 때 처리
        //Debug.Log("Player is dead!");
        // 여기서 플레이어 사망 시 처리를 추가할 수 있습니다. 예: 게임 오버 화면 표시, 플레이어 비활성화 등
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
                //Debug.Log(":맞");
                OnDamage(1);
            }
        }

    }
}
