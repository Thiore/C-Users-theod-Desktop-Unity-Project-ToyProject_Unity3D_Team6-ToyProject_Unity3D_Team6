using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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
        //LoadRank();
    }

    public List<RankData> ranks = new List<RankData>();
    public RankData[] ranks_a = new RankData[4];
    
    private RankData rank = new RankData();
    public void SortRank()
    {
        if(ranks.Count > 0)
        {
            ranks.Sort((rank1, rank2) => rank1.score.CompareTo(rank2.score));
        }



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


