using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //�ش� ��ũ��Ʈ ������Ʈ �������� (0, 50, 0) �̿����� 
    public Enemy_Data[] enemy_datas;

    //�� ���ʾƷ�/������ ������ ���� �� ����Ʈ ���ϱ�
    [SerializeField] private Transform minPoint;
    [SerializeField] private Transform maxPoint;
    private Vector3 spawnPoint;
    private float spawnX;
    private float spawnZ;

    //�� ��ü �����α� ������ ��ȣ�� ���� 
    [SerializeField] private Enemy_Controller[] enemy_Controllers;
    private List<Enemy_Controller> enemy_list = new List<Enemy_Controller>();

    public List<Enemy_Controller> Enemy_list { get => enemy_list; }


    private void Awake()
    {
        Setup_Enemy_co();
    }

    private void OnEnable()
    {
        StartCoroutine(DropEnemy_co());        
    }


    private Vector3 Setup_SpawnPoint()
    {
        // ���� ���ַ� ������ ��ġ���� ����
        spawnX = Random.Range(minPoint.position.x, maxPoint.position.x);
        spawnZ = Random.Range(minPoint.position.z, maxPoint.position.z);

        spawnPoint = new Vector3(spawnX, 60f, spawnZ);
        return spawnPoint;
    }

    // Ǯ������ ��ٸ��� ���ؼ� �ڷ�ƾ?
    private void Setup_Enemy_co()
    {
        int count = 30;
        //�� ��ü �����α� ������ ��ȣ�� ���� / 30 20 10
        for (int i = 0; i < enemy_datas.Length; i++)
        {
            //�Ϲ� enemy�� boss ����
            for (int j = count; j > 0; j--)
            {
                if (i == enemy_datas.Length - 1)
                {
                    return;
                }
                Create_Enemy(i);
            }
            count -= 10;
            Create_Enemy(enemy_datas.Length-1);
        }
    }

    private void Create_Enemy(int index)
    {
        Enemy_Data data = enemy_datas[index];

        Enemy_Controller enemy = Instantiate(enemy_Controllers[index], transform.position, Quaternion.identity);
        enemy.SetupData(data);
        enemy_list.Add(enemy);
        enemy.gameObject.SetActive(false);
    }

    private IEnumerator DropEnemy_co()
    {
        while (true)
        {
            if(enemy_list.Count >= 0)
            {
                int index = Random.Range(0, enemy_list.Count);
                var enemy = enemy_list[index];
                enemy_list.RemoveAt(index);

                enemy.transform.position = Setup_SpawnPoint();
                enemy.gameObject.SetActive(true);
                yield return new WaitForSeconds(1.5f);
            }
        }    
    }


}
