using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundSpawner : MonoBehaviour
{
    [SerializeField] List<GameObject> spawnableObjects;
    [SerializeField] Transform backgroundParentObj;
    [SerializeField] int numberOfObjectToPool;
    [SerializeField] int objectOnTheSceneN;
    [SerializeField] int fixedXPosition;
    [SerializeField] int fixedZPosition;
    [SerializeField] Vector2 xPosMinMax;
    [SerializeField] Vector2 yPosMinMax;
    [SerializeField] Vector2 zPosMinMax;
    [SerializeField] Vector2 yRotMinMax;
    [SerializeField] Vector2 scaleMinMax;

    List<GameObject> inActiveObjectPool = new();
    List<GameObject> activeObjectPool = new();
    int counter = 0;
    // Start is called before the first frame update
    void Start()
    {
        InstiateObjectToPool();
        Spawner(objectOnTheSceneN, 0);
    }

    // Update is called once per frame
    void Update()
    {
        SwapObjects();
        BackgroundMovement();
    }

    private void BackgroundMovement()
    {
        transform.Translate(GameManager.instance.GetSpeed() * new Vector3(-1, 0, 0) * Time.deltaTime);
    }

    private void InstiateObjectToPool()
    {
        for (int i = 0; i < spawnableObjects.Count; i++)
        {
            for (int j = 0; j < numberOfObjectToPool; j++)
            {
                GameObject go = Instantiate(spawnableObjects[i], backgroundParentObj);
                go.SetActive(false);
                inActiveObjectPool.Add(go);
            }
        }
    }

    private void Spawner(int _spawnNumber, float _xPos)
    {
        for (int i = 0; i < _spawnNumber; i++)
        {
            int randomNumber = Random.Range(0, inActiveObjectPool.Count);
            inActiveObjectPool[randomNumber].SetActive(true);

            float xPos = _xPos + (i + 1) * fixedXPosition + Random.Range(xPosMinMax.x, xPosMinMax.y);
            float yPos = Random.Range(yPosMinMax.x, yPosMinMax.y);
            float zPos = Random.Range(zPosMinMax.x, zPosMinMax.y);
            if (counter % 2 == 0) zPos += fixedZPosition;
            inActiveObjectPool[randomNumber].transform.position = new Vector3(xPos, yPos, zPos);

            float yRot = Random.Range(yRotMinMax.x, yRotMinMax.y);
            inActiveObjectPool[randomNumber].transform.rotation = Quaternion.Euler(new Vector3(0, yRot, 0));

            float scale = Random.Range(scaleMinMax.x, scaleMinMax.y);
            inActiveObjectPool[randomNumber].transform.localScale = new Vector3(scale, scale, scale);

            activeObjectPool.Add(inActiveObjectPool[randomNumber]);
            inActiveObjectPool.RemoveAt(randomNumber);

            counter++;
        }
    }

    private void SwapObjects()
    {
        if (activeObjectPool.Count <= 0) return;

        Vector3 pos = Camera.main.WorldToViewportPoint(activeObjectPool[0].transform.position);

        if (pos.x < -0.2)
        {
            activeObjectPool[0].SetActive(false);
            inActiveObjectPool.Add(activeObjectPool[0]);
            activeObjectPool.RemoveAt(0);
            Spawner(1, activeObjectPool[activeObjectPool.Count - 1].transform.position.x);
        }
    }
}
