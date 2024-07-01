using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField] private Transform Mango;
    [SerializeField] private Transform Runa;

    public void ChangeCharacter()
    {
        if(GameManager.instance.SelectPlayer == ePlayer.Mango)
        {
            GameManager.instance.SelectPlayer = ePlayer.Runa;
            StartCoroutine(ChangeRuna());
        }
        else
        {
            GameManager.instance.SelectPlayer = ePlayer.Mango;
            StartCoroutine(ChangeMango());
        }
    }

    private IEnumerator ChangeMango()
    {
        Runa.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        Mango.gameObject.SetActive(true);
    }

    private IEnumerator ChangeRuna()
    {
        Mango.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        Runa.gameObject.SetActive(true);
    }

    public void OnClickStartButton()
    {
        GameManager.instance.isGame = false;
        SceneManager.LoadScene("MainGame");
    }

    public void OnClickQuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
Application.Quit();
#endif
    }
}
