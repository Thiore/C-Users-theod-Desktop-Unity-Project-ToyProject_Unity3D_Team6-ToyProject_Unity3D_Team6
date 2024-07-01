using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class RankingManager : MonoBehaviour
{
    public static RankingManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    public List<RankData> ranks = new List<RankData>();
    private RankData rank = new RankData();
    public void SortRank()
    {

        
    }
    public void SaveRank()
    {
        if (ranks.Count > 3)
            ranks.RemoveAt(ranks.Count - 1);

        rank.name = GameManager.instance.SelectPlayer.ToString();
        rank.score = GameManager.instance.Score;
        ranks.Add(rank);
        string jsonData = JsonUtility.ToJson(ranks);
        string path = Path.Combine(Application.dataPath, "rankData.json");
        
        File.WriteAllText(path, jsonData);
    }

    public void LoadRank()
    {
        string path = Path.Combine(Application.dataPath, "rankData.json");
        string jsonData = File.ReadAllText(path);
        ranks = JsonUtility.FromJson<List<RankData>>(jsonData);
    }
}


