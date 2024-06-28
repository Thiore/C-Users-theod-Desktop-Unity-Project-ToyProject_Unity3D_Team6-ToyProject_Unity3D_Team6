using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Controller : MonoBehaviour
{
    [Header("추적할 대상 레이어")]
    public LayerMask TartgetLayer;
    private Player_Controller player;

    private EnemySpawner spawner;

    public float MaxHp;
    public float CurrentHp { get; protected set; }
    float damage;
    private bool isDead;
    public bool IsDead { get => isDead; set => isDead = value; }

    private bool isGround;

    private NavMeshAgent agent;


    private void Awake()
    {
        isDead = false;
        player = GameObject.Find("Player").transform.GetComponent<Player_Controller>();
        spawner = GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>();
    }

    private void OnEnable()
    {
        isGround = false;
        StartCoroutine(CheckGround());
    }


    private IEnumerator CheckGround()
    {
        while(!isGround)
        {
            if (transform.position.y <= 0.1f)
            {
                agent = GetComponent<NavMeshAgent>();
                agent.enabled = true;
                isGround = true;
                StartCoroutine(Update_target_position_co());
            }
            yield return null;
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
        if (CurrentHp <= 0 && !isDead)
        {
            Die();
        }

    }

    public void Die()
    {
        if (!isDead)
        {
            isDead = true;
            gameObject.SetActive(false);
            gameObject.transform.position = spawner.transform.position;
            spawner.Enemy_list.Add(this);
        }
    }



    private IEnumerator Update_target_position_co()
    {
        while (isGround == true && player != null)
        {
            agent.isStopped = false;
            agent.SetDestination(player.transform.position);
            yield return null;
        }
    }


}
