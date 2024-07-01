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

    public bool isGame = false;

    
    private void Update()
    {
        if(!isGame)
        {
            if (SceneManager.GetActiveScene().name.Equals("MainGame"))
            {
                Debug.Log("여기 안들어오니?");
                if (FindObjectOfType<Player_Move>().gameObject.activeSelf)
                    Destroy(FindObjectOfType<Player_Gunner>().gameObject);
                if (FindObjectOfType<Player_Gunner>().gameObject.activeSelf)
                    Destroy(FindObjectOfType<Player_Move>().gameObject);
                Cursor.lockState = CursorLockMode.Locked;
                isGame = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                isGame = true;
            }
                
        }
    }



}
