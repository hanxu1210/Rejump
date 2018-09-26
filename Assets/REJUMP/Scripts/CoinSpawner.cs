using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CoinSpawner : MonoBehaviour 
{
    public BoxCollider2D spawnArea;     //Coins spawn area trigger;
    public float spawnDelay;            //Coins spawn delay;
    public Transform[] coinsPool;       //Coins transforms pool;

    private float time;

	// Update is called once per frame
    void Update()
    {
        //Iterate through coins pool array;
        for (int i = 0; i < coinsPool.Length; i++)
        {
            //If time is less than Game time;
            if (time < Time.time)
            {
                //Check if coin object disabled, and if so:
                if (!coinsPool[i].gameObject.activeSelf)
                {
                    //Move coin to random position based on spawn area trigger;
                    coinsPool[i].position = new Vector3(Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x), spawnArea.bounds.center.y, 0);
                    //Enable coin object;
                    coinsPool[i].gameObject.SetActive(true);
                    //Increase time for next spawn;
                    time = Time.time + spawnDelay;
                    //Quit loop;
                    break;
                }
            }
        }
    }
}