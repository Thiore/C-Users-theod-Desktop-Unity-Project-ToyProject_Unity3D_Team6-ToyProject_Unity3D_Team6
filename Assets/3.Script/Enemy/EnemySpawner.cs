using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //�ش� ��ũ��Ʈ ������Ʈ �������� (0, 50, 0) �̿����� 
    public Enemy_Data[] enemy_datas;
    public Enemy_Controller enemy_common;
 

    //�� ���ʾƷ�/������ ������ ���� �� ����Ʈ���ϱ�
    [SerializeField] private Transform minPoint;
    [SerializeField] private Transform maxPoint;
    private Vector3 spawnPoint;
    private float spawnX;
    private float spawnZ;

    //�� ��ü �����α� ������ ��ȣ�� ���� 
    [SerializeField] private Enemy_Controller[] enemy_Controllers;
    private List<Enemy_Controller> enemy_list = new List<Enemy_Controller>();


    private void Awake()
    {
        Setup_Enemy_co();
    }


    private Vector3 Setup_SpawnPoint()
    {
        // ���� ���ַ� ������ ��ġ���� ����
        spawnX = Random.Range(minPoint.position.x, maxPoint.position.x);
        spawnZ = Random.Range(minPoint.position.z, maxPoint.position.z);

        spawnPoint = new Vector3(spawnX, 50f, spawnZ);
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
            for (int j = count; j > 0; j++)
            {
                if (i == enemy_datas.Length - 1)
                {
                    return;
                }
                Create_Enemy(i);
            }
            count -= 10;
            Create_Enemy(enemy_datas.Length - 1);
        }


    }

    private void Create_Enemy(int index)
    {
        Enemy_Data data = enemy_datas[index];
        Vector3 spawnPoint = Setup_SpawnPoint();

        Enemy_Controller enemy = Instantiate(enemy_Controllers[index], spawnPoint, Quaternion.identity);
        enemy.SetupData(data);
        enemy_list.Add(enemy);
        enemy.gameObject.SetActive(false);
    }


}
