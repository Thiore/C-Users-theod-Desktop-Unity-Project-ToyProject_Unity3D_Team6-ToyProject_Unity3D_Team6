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

    

    public List<RankData> Rank_List = new List<RankData>();
   
    
    public RankData rank = new RankData();
    
    public void SaveRank()
    {
        for(int i = Rank_List.Count-1; i>=0;i--)
        {
            if(Rank_List[i].score>rank.score&&i.Equals(Rank_List.Count-1))
            {
                break;
            }
            else if (Rank_List[i].score > rank.score)
            {
                Rank_List.Insert(i, rank);
                Rank_List.RemoveAt(Rank_List.Count - 1);
                string jsonData = JsonUtility.ToJson(Rank_List);
                string path = Path.Combine(Application.dataPath, "rankData.json");

                File.WriteAllText(path, jsonData);
                return;
            }
            
        }
        
        
    }

    public void SetRanking_Data()
    {
        rank.name = GameManager.instance.SelectPlayer.ToString();
        rank.score = GameManager.instance.Score;
        Debug.Log(rank.name);
        Debug.Log(rank.score);
    }

    public List<RankData> LoadRank()
    {
        string path = Path.Combine(Application.dataPath, "rankData.json");
        string jsonData = File.ReadAllText(path);
        return JsonUtility.FromJson<List<RankData>> (jsonData);
    }

    

    
}


