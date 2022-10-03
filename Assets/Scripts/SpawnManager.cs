using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    
    [SerializeField] private GameObject[] spawnObject;

    //index of objects to spawn
    //    #0 enemy_model_1

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(enemySpawnrRoutine());
      
    }

    IEnumerator enemySpawnrRoutine() 
    {
        while (true)
        {
            float randomY = Random.Range(-5, 3);
            Instantiate(spawnObject[0], new Vector3(-14.0f, randomY, 2.5f), Quaternion.identity);
            yield return new WaitForSeconds(5.0f);
        }
    }

}
