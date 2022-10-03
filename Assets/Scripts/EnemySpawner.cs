using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] List<EnemySpawnData> spawnableObjects;
    [SerializeField] Transform enemyParentObject;
    [SerializeField] int numberOfObjectToPool;
    [SerializeField] float spawnRate;

    List<EnemySpawnData> inActiveObjectPool = new();
    List<EnemySpawnData> activeObjectPool = new();
    void Start()
    {
        InstiateObjectToPool();
        StartCoroutine(ObjectSpawner(spawnRate));
    }

    // Update is called once per frame
    void Update()
    {
        SwapObjects();
    }

    IEnumerator ObjectSpawner(float _spawnrate)
    {
        while(true)
        {
            Spawner();
            yield return new WaitForSeconds(_spawnrate);
        }
    }

    private void InstiateObjectToPool()
    {
        for (int i = 0; i < spawnableObjects.Count; i++)
        {
            for (int j = 0; j < spawnableObjects[i].amountOnMap; j++)
            {
                GameObject go = Instantiate(spawnableObjects[i].spawnObject, enemyParentObject);
                go.SetActive(false);
                inActiveObjectPool.Add(new EnemySpawnData(go, spawnableObjects[i].spawnPosY, 0, spawnableObjects[i].spawnRotation));
            }
        }
    }

    private void Spawner()
    {
        if (inActiveObjectPool.Count <= 0) return;

        float Ypos = Random.Range(0.1f, 0.9f);
        Vector3 spawnPos = new Vector3(1.2f, Ypos, 5.0f);
        int randomIndex = Random.Range(0, inActiveObjectPool.Count);

        if (inActiveObjectPool[randomIndex].spawnPosY != 0) spawnPos = new Vector3(spawnPos.x, inActiveObjectPool[randomIndex].spawnPosY, spawnPos.z);

        Vector3 spwanPosWorldToPoint = Camera.main.ViewportToWorldPoint(spawnPos);
        Vector3 spawnRotation = inActiveObjectPool[randomIndex].spawnRotation;

        inActiveObjectPool[randomIndex].spawnObject.gameObject.transform.position = spwanPosWorldToPoint;
        inActiveObjectPool[randomIndex].spawnObject.SetActive(true);
        inActiveObjectPool[randomIndex].spawnObject.gameObject.transform.rotation = Quaternion.Euler(spawnRotation);
        activeObjectPool.Add(inActiveObjectPool[randomIndex]);
        inActiveObjectPool.RemoveAt(randomIndex);
    }

    private void SwapObjects()
    {
        if (activeObjectPool.Count <= 0) return;

        for (int i = 0; i < activeObjectPool.Count; i++)
        {
            Vector3 pos = Camera.main.WorldToViewportPoint(activeObjectPool[i].spawnObject.transform.position);

            if (pos.x < -0.5)
            {
                activeObjectPool[i].spawnObject.SetActive(false);
                //inActiveObjectPool.Add(activeObjectPool[i]);
                activeObjectPool.RemoveAt(i);
            }
        }
    }
}


[System.Serializable]
public class EnemySpawnData
{
    public GameObject spawnObject;
    public float spawnPosY;
    public int amountOnMap;
    public Vector3 spawnRotation;

    public EnemySpawnData (GameObject _spawnObject, float _spawnPosY, int _amountOnMap, Vector3 _spawnRotation)
    {
        spawnObject = _spawnObject;
        spawnPosY = _spawnPosY;
        amountOnMap = _amountOnMap;
        spawnRotation = _spawnRotation;
    }
}
