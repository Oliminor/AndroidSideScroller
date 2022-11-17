using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;

    [SerializeField] List<EnemySpawnData> spawnableEnemyObjects;
    [SerializeField] List<SpawnRate> enemySpawnRate;

    [SerializeField] List<EnemySpawnData> spawnablObstacleObjects;
    [SerializeField] List<SpawnRate> obstacleSpawnRate;

    [SerializeField] float delayStartTime;
    [SerializeField] float delayEndTime;

    private int gameTime;

    private float currentEnemySpawnaRate;
    private float currentObstacleSpawnaRate;

    private List<EnemySpawnData> enemyList = new();
    private List<EnemySpawnData> obstacleList = new();

    public List<SpawnRate> GetEnemySpawnRate() { return enemySpawnRate; }
    public List<EnemySpawnData> GetEnemySpawnerObjects() { return spawnableEnemyObjects; }

    public List<SpawnRate> GetObstacleSpawnRate() { return obstacleSpawnRate; }
    public List<EnemySpawnData> GetObstaclepawnerObjects() { return spawnablObstacleObjects; }
    public void SetGameTime(int _gameTIme) { gameTime = _gameTIme; }        
    

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        EnemyInstantiate();
        ObjectInstantiate();
        StartCoroutine(StartDelay());
    }

    private void Update()
    {
        if (enemyList.Count <= 0 && obstacleList.Count <= 0 && !GameManager.instance.GetIsLevelEnded())
        {
            StartCoroutine(GameEnded());
            return;
        }
    }

    IEnumerator SpawnEnemy()
    {
        int spawnIndex = 0;
        while (true)
        {
            Spawner(enemyList);

            if (enemySpawnRate[spawnIndex].spawnRateTimeLimit < GameManager.instance.GetGameTIme()) spawnIndex++;
            currentEnemySpawnaRate = enemySpawnRate[spawnIndex].spawnRate;

            yield return new WaitForSeconds(currentEnemySpawnaRate);
        }
    }

    IEnumerator SpawnObstacle()
    {
        int spawnIndex = 0;
        while (true)
        {
            Spawner(obstacleList);

            if (obstacleSpawnRate[spawnIndex].spawnRateTimeLimit < GameManager.instance.GetGameTIme()) spawnIndex++;
            currentObstacleSpawnaRate = obstacleSpawnRate[spawnIndex].spawnRate;

            yield return new WaitForSeconds(currentObstacleSpawnaRate);
        }
    }


    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(delayStartTime);
        StartCoroutine(SpawnEnemy());
        StartCoroutine(SpawnObstacle());
        StartCoroutine(GameManager.instance.ScoreAdditionPerSecond());
    }

    IEnumerator GameEnded()
    {
        GameManager.instance.LevelFinished();
        yield return new WaitForSeconds(delayEndTime);
        GamePlayUI.instance.EndLevelSceneTrigger();
    }

    private void EnemyInstantiate()
    {
        for (int i = 0; i < spawnableEnemyObjects.Count; i++)
        {
            for (int j = 0; j < spawnableEnemyObjects[i].amountOnMap; j++)
            {
                GameObject go = Instantiate(spawnableEnemyObjects[i].spawnObject, transform);
                GameManager.instance.SetEnemyNumber();
                GameManager.instance.SetEnemyScore(go.GetComponent<BaseEnemy>().GetEnemyScore());
                go.SetActive(false);

                enemyList.Add(new EnemySpawnData(go, spawnableEnemyObjects[i].spawnPosY, 0, spawnableEnemyObjects[i].spawnRotation));
            }
        }
    }

    private void ObjectInstantiate()
    {
        for (int i = 0; i < spawnablObstacleObjects.Count; i++)
        {
            for (int j = 0; j < spawnablObstacleObjects[i].amountOnMap; j++)
            {
                GameObject go = Instantiate(spawnablObstacleObjects[i].spawnObject, transform);

                go.SetActive(false);

                obstacleList.Add(new EnemySpawnData(go, spawnablObstacleObjects[i].spawnPosY, 0, spawnablObstacleObjects[i].spawnRotation));
            }
        }
    }

    private void Spawner(List<EnemySpawnData> _list)
    {
        if (GameManager.instance.GetIsLevelEnded()) return;

        if (_list.Count <= 0) return;

        float Ypos = Random.Range(0.1f, 0.9f);
        Vector3 spawnPos = new Vector3(1.2f, Ypos, GameManager.instance.GetZPosition());
        int randomIndex = Random.Range(0, _list.Count);

        if (_list[randomIndex].spawnPosY != Vector2.zero)
        {
            float randomY = Random.Range(_list[randomIndex].spawnPosY.x, _list[randomIndex].spawnPosY.y);
            spawnPos = new Vector3(spawnPos.x, randomY, spawnPos.z);
        }

        Vector3 spwanPosWorldToPoint = Camera.main.ViewportToWorldPoint(spawnPos);
        Vector3 spawnRotation = _list[randomIndex].spawnRotation;

        _list[randomIndex].spawnObject.gameObject.transform.position = spwanPosWorldToPoint;
        _list[randomIndex].spawnObject.SetActive(true);
        _list[randomIndex].spawnObject.gameObject.transform.rotation = Quaternion.Euler(spawnRotation);
        _list.RemoveAt(randomIndex);

    }

    public int GetGameTime()
    {
        float enemyTime = CalculateTotalEnemySpawnTime(GetEnemySpawnerObjects(), GetEnemySpawnRate());
        float obstacleTime = CalculateTotalEnemySpawnTime(GetObstaclepawnerObjects(), GetObstacleSpawnRate());

        int longerTime = (int)enemyTime;
        if (obstacleTime > enemyTime) longerTime = (int)obstacleTime;

        return longerTime;
    }

    public int CalculateTotalEnemySpawnTime(List<EnemySpawnData> _spawnData, List<SpawnRate> _spawnRate)
    {
        float objectNumber = 0;
        for (int i = 0; i < _spawnData.Count; i++)
        {
            objectNumber += _spawnData[i].amountOnMap;
        }

        float totalTime = 0;
        for (int i = 0; i < _spawnRate.Count; i++)
        {
            float spawnRateTime;

            float timeSlot = _spawnRate[i].spawnRateTimeLimit;
            if (i > 0) timeSlot = _spawnRate[i].spawnRateTimeLimit - _spawnRate[i - 1].spawnRateTimeLimit;

            spawnRateTime = timeSlot / _spawnRate[i].spawnRate;

            objectNumber -= spawnRateTime;

            if (objectNumber < 0)
            {
                objectNumber *= -1;
                timeSlot -= objectNumber * _spawnRate[i].spawnRate;
            }

            totalTime += timeSlot;
        }

        return (int)totalTime;
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

[System.Serializable]
public class SpawnRate
{
    public float spawnRate;
    public float spawnRateTimeLimit;

    public SpawnRate (float _spawnRate, float _startSpawnRateFrom)
    {
        spawnRate = _spawnRate;
        spawnRateTimeLimit = _startSpawnRateFrom;
    }
}
