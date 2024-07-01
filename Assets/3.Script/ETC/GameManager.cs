using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ePlayer
{
    Mango = 0,
    Runa
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        selectPlayer = ePlayer.Mango;
    }

    private ePlayer selectPlayer;

    public ePlayer SelectPlayer
    {
        get
        {
            return selectPlayer;
        }
        set
        {
            selectPlayer = value;
        }
    }
   
    



}
