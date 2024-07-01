using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingManager : MonoBehaviour
{
    public string name;
    public int Score = 0;

    public void SortRank()
    {

    }
    public void SaveRank()
    {
        name = GameManager.instance.SelectPlayer.ToString();
        Score = GameManager.instance.Score;
    }

    public void LoadRank()
    {

    }
}

