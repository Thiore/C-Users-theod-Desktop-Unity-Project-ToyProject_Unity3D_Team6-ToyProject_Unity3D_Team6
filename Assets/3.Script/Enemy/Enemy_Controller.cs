using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Controller : MonoBehaviour
{
    [Header("추적할 대상 레이어")]
    public LayerMask TartgetLayer;

    public float MaxHp;
    public float CurrentHp { get; protected set; }
    float damage;
    private bool isDead;
    public bool IsDead { get => isDead; set => isDead = value; }

    private NavMeshAgent agent;

    private void Awake()
    {
        isDead = false;
    }

    private void Update()
    {
        if(transform.position.y <= 0.1f)
        {
            agent = GetComponent<NavMeshAgent>();
            agent.enabled = true;
        }

    }

    public void SetupData(Enemy_Data data)
    {
        MaxHp = data.MaxHp;
        CurrentHp = data.MaxHp;
        damage = data.damage;
       // agent.speed = data.speed;
    }

    public void OnDamage(int damage)
    {
        CurrentHp -= damage;
        if(CurrentHp <= 0 && !isDead)
        {
            Die();
        }

    }

    public void Die()
    {
        if (isDead)
        {
            isDead = true;
        }
    }

    private IEnumerator Update_tartget_position_co()
    {
        while(!isDead)
        {

        }
    }
}
