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
    private GameObject player;

    private EnemySpawner spawner;

    private Enemy_Data data;

    public float MaxHp;
    public float CurrentHp { get; protected set; }
    float damage;
    private bool isDead;
    public bool IsDead { get => isDead; set => isDead = value; }

    private bool isGround;

    public NavMeshAgent agent;
    public NavMeshAgent Agent { get => agent; protected set => agent = value; }

    private Rigidbody enemy_r;


    private void Awake()
    {
        isDead = false;
        spawner = GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>();
        enemy_r = GetComponent<Rigidbody>();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        isGround = false;
        player = GameObject.FindWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(CheckGround());
        MaxHp = data.MaxHp;
        CurrentHp = data.MaxHp;
        damage = data.damage;
    }

    private void FixedUpdate()
    {
        if(!isGround)
        {
            enemy_r.AddForce(Vector3.down * 50f);
        }
    }

    private IEnumerator CheckGround()
    {
        while(!isGround)
        {
            if (transform.position.y <= 1.3f)
            {
                agent.enabled = true;
                isGround = true;
                transform.rotation = Quaternion.identity;
                agent.speed = data.speed;
                StartCoroutine(Update_target_position_co());
            }
            else
            {
                if(player != null)
                {
                    Vector3 direction = (player.transform.position - transform.position).normalized;
                    transform.position += direction * 5f/*agent.speed*/ * Time.deltaTime;
                }
            }
            yield return null;
        }
        yield break;
    }

    public void SetupData(Enemy_Data data)
    {
        this.data = data;       
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.transform.CompareTag("Bullet"))
        {
            OnDamage(collision.transform.GetComponent<Bullet>().Damage);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Melee"))
        {
            Debug.Log("처맞음");
            OnDamage(other.transform.GetComponent<Weapon>().Damage);
        }
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
            GameManager.instance.AddScore(data.score);
            isDead = true;
            StopCoroutine(Update_target_position_co());
            agent.enabled = false;
            gameObject.SetActive(false);
            gameObject.transform.position = spawner.transform.position;
            spawner.Enemy_list.Add(this);
        }
    }



    private IEnumerator Update_target_position_co()
    {
        while (isGround == true && isDead == false)
        {
            if (player != null)
            {
                agent.isStopped = false;
                agent.SetDestination(player.transform.position);
            }
            yield return null;
        }
    }

}
