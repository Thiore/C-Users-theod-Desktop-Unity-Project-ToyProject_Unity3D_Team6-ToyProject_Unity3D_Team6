using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Health : MonoBehaviour
{
    [SerializeField] private int StartHealth = 100;  // 시작 체력
    public int CurrentHealth;  // 현재 체력
    private Animator playerAnimator;
    public bool isDie;

    private void Awake()
    {
        CurrentHealth = StartHealth;  // 시작할 때 현재 체력을 시작 체력으로 설정
        playerAnimator = GetComponentInChildren<Animator>();
        isDie = false;
    }

    public void OnDamage(int damage)
    {
        if (!isDie)//isdie가 false일 경우에만 ondamage 메서드 호출
        {
            CurrentHealth -= damage;  // 데미지를 받으면 현재 체력을 감소
            Debug.Log("데미지");
        }
        else // isdie가 true일 경우 즉시 리턴
        {
            return;
        }
        
        if (CurrentHealth <= 0)
        {
            Die();  // 체력이 0 이하가 되면 Die 메서드 호출
        }
    }

    private void Die()
    {
        // 플레이어가 죽었을 때 처리
        Debug.Log("Player is dead!");
        // 여기서 플레이어 사망 시 처리를 추가할 수 있습니다. 예: 게임 오버 화면 표시, 플레이어 비활성화 등
        playerAnimator.SetTrigger("Die");
        isDie = true;

    }

    private void OnCollisionEnter(Collision collision)
    {
        // 충돌한 오브젝트가 Enemy 태그를 가지고 있는지 확인
        if (collision.gameObject.CompareTag("Enemy"))
        {
            OnDamage(1);  // 데미지를 1만큼 받음 (필요에 따라 데미지 값을 조정할 수 있음)
        }
    }
}
