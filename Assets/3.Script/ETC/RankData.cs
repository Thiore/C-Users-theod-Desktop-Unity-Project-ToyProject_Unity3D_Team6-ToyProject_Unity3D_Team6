using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RankData 
{
    
    public string name;
    public int score;
}

[System.Serializable]

public class RankList
{
    public List<RankData> ranks;
}