using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private enum Type { Melee, Range}
    [SerializeField]private Type type;
    [SerializeField] private int damage;
    [SerializeField] public float rate;
    [SerializeField] private BoxCollider meleeArea;
    [SerializeField] private GameObject trailEffect;

    public void Use()
    {
        if (type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
    }

    private IEnumerator Swing()
    {
        meleeArea.enabled = true;
        trailEffect.SetActive(true);
        yield return new WaitForSeconds(0.7f);
        meleeArea.enabled = false;
        trailEffect.SetActive(false);
    }

}
