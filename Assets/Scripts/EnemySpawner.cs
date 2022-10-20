using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;

    [SerializeField] List<EnemySpawnData> spawnableObjects;
    [SerializeField] int numberOfObjectToPool;
    [SerializeField] float spawnRate;
    [SerializeField] float delayStartTime;
    [SerializeField] float delayEndTime;

    List<EnemySpawnData> inActiveObjectPool = new();
    List<EnemySpawnData> activeObjectPool = new();

    public float GetSpawnRate() { return spawnRate; }
    public int GetObjectNumber() { return spawnableObjects.Count; }

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        InstiateObjectToPool();
        StartCoroutine(StartDelay());
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


    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(delayStartTime);
        StartCoroutine(ObjectSpawner(spawnRate));
        StartCoroutine(GameManager.instance.ScoreAdditionPerSecond());
    }

    IEnumerator GameEnded()
    {
        GameManager.instance.LevelFinished();
        yield return new WaitForSeconds(delayEndTime);
        GamePlayUI.instance.EndLevelSceneTrigger();
    }

    private void InstiateObjectToPool()
    {
        for (int i = 0; i < spawnableObjects.Count; i++)
        {
            for (int j = 0; j < spawnableObjects[i].amountOnMap; j++)
            {
                GameObject go = Instantiate(spawnableObjects[i].spawnObject, transform);
                if (go.tag == "Enemy")
                {
                    GameManager.instance.SetEnemyNumber();
                    GameManager.instance.SetEnemyScore(go.GetComponent<BaseEnemy>().GetEnemyScore());
                }
                go.SetActive(false);
                inActiveObjectPool.Add(new EnemySpawnData(go, spawnableObjects[i].spawnPosY, 0, spawnableObjects[i].spawnRotation));
            }
        }
    }

    private void Spawner()
    {
        if (GameManager.instance.GetIsLevelEnded()) return;

        if (inActiveObjectPool.Count <= 0 && !GameManager.instance.GetIsLevelEnded())
        {
            StartCoroutine(GameEnded());
            return;
        }

        float Ypos = Random.Range(0.1f, 0.9f);
        Vector3 spawnPos = new Vector3(1.2f, Ypos, GameManager.instance.GetZPosition());
        int randomIndex = Random.Range(0, inActiveObjectPool.Count);

        if (inActiveObjectPool[randomIndex].spawnPosY != Vector2.zero)
        {
            float randomY = Random.Range(inActiveObjectPool[randomIndex].spawnPosY.x, inActiveObjectPool[randomIndex].spawnPosY.y); 
            spawnPos = new Vector3(spawnPos.x, randomY, spawnPos.z);
        }

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
            if (!activeObjectPool[i].spawnObject) return;
            Vector3 pos = Camera.main.WorldToViewportPoint(activeObjectPool[i].spawnObject.transform.position);

            if (pos.x < -0.5)
            {
                activeObjectPool[i].spawnObject.SetActive(false);
                activeObjectPool.RemoveAt(i);
            }
        }
    }
}


[System.Serializable]
public class EnemySpawnData
{
    public GameObject spawnObject;
    public Vector2 spawnPosY;
    public int amountOnMap;
    public Vector3 spawnRotation;

    public EnemySpawnData (GameObject _spawnObject, Vector2 _spawnPosY, int _amountOnMap, Vector3 _spawnRotation)
    {
        spawnObject = _spawnObject;
        spawnPosY = _spawnPosY;
        amountOnMap = _amountOnMap;
        spawnRotation = _spawnRotation;
    }
}
