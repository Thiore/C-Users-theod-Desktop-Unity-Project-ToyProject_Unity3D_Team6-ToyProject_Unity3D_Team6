using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //해당 스크립트 오브젝트 포지션은 (0, 50, 0) 이여야함 
    public Enemy_Data[] enemy_datas;
    public Enemy_Controller enemy_common;
 

    //맵 왼쪽아래/오른쪽 끝으로 넓이 및 포인트구하기
    [SerializeField] private Transform minPoint;
    [SerializeField] private Transform maxPoint;
    private Vector3 spawnPoint;
    private float spawnX;
    private float spawnZ;

    //각 개체 만들어두기 마지막 번호는 보스 
    [SerializeField] private Enemy_Controller[] enemy_Controllers;
    private List<Enemy_Controller> enemy_list = new List<Enemy_Controller>();


    private void Awake()
    {
        Setup_Enemy_co();
    }


    private Vector3 Setup_SpawnPoint()
    {
        // 랜덤 범주로 랜덤한 위치에서 등장
        spawnX = Random.Range(minPoint.position.x, maxPoint.position.x);
        spawnZ = Random.Range(minPoint.position.z, maxPoint.position.z);

        spawnPoint = new Vector3(spawnX, 50f, spawnZ);
        return spawnPoint;
    }

    // 풀링까지 기다리기 위해서 코루틴?
    private void Setup_Enemy_co()
    {
        int count = 30;
        //각 개체 만들어두기 마지막 번호는 보스 / 30 20 10
        for (int i = 0; i < enemy_datas.Length; i++)
        {
            //일반 enemy만 boss 제외
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
