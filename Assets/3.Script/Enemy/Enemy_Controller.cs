using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
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

    public NavMeshAgent agent;
    public NavMeshAgent Agent { get => agent; protected set => agent = value; }


    private void Awake()
    {
        isDead = false;
        player = FindObjectOfType<Player_Controller>();
        spawner = GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        isGround = false;
        StartCoroutine(CheckGround());
    }

    private void Update()
    {

    }

    private IEnumerator CheckGround()
    {
        while(!isGround)
        {
            if (transform.position.y <= 0.1f)
            {
                agent.enabled = true;
                isGround = true;
                transform.rotation = Quaternion.identity;
                StartCoroutine(Update_target_position_co());
            }
            else
            {
                Vector3 direction = (player.transform.position - transform.position).normalized;
                transform.position += direction * agent.speed * Time.deltaTime;
            }
            yield return null;
        }
    }

    public void SetupData(Enemy_Data data)
    {
        MaxHp = data.MaxHp;
        CurrentHp = data.MaxHp;
        damage = data.damage;
        agent.speed = data.speed;
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
