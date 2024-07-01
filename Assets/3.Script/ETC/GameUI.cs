using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameObject HeartPrefabs;
    [SerializeField] private Camera FinalCamera;
    private GameObject Heart;
    private List<GameObject> HeartList = new List<GameObject>();
    private Player_Health currentHealth;
    private Vector3 HP_Pos = new Vector3(-80f, -45f, 0f);
    private void Start()
    {
        currentHealth = FindObjectOfType<Player_Health>();
        for (int i = 0; i < currentHealth.CurrentHealth; i++)
        {
            Heart = Instantiate(HeartPrefabs, transform.position, Quaternion.LookRotation(FinalCamera.gameObject.transform.forward));
            Heart.transform.SetParent(transform);
            Heart.transform.position = transform.position + HP_Pos + Vector3.right * 15f * i;
            Heart.transform.localScale = new Vector3(50, 50, 50);
            HeartList.Add(Heart);
        }
        Debug.Log(currentHealth.gameObject.name);
        Debug.Log(currentHealth.CurrentHealth);
    }
    private void Update()
    {
        for(int i = 0; i < HeartList.Count;i++)
        {
            HeartList[i].transform.Rotate(Vector3.up,Time.deltaTime*10f);
        }
        SetHp();
    }

    private void SetHp()
    {
        if (!HeartList.Count.Equals(currentHealth.CurrentHealth))
        {
            Debug.Log("¿©±âµé¾î¿È?");
            Destroy(HeartList[HeartList.Count - 1]);
            HeartList.RemoveAt(HeartList.Count - 1);
        }
    }
}
