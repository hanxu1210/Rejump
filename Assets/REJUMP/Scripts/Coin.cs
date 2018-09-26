using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Coin : MonoBehaviour 
{
    //Disable coin object on entering dead zone trigger;
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("DeathZone"))
            gameObject.SetActive(false);
    }
}