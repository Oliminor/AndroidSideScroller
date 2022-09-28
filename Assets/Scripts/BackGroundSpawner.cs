using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundSpawner : MonoBehaviour
{
    [SerializeField] GameObject spawnObject;
    [SerializeField] int min;
    [SerializeField] int max;
    [SerializeField] float xSpacing;
    [SerializeField] float YSpacing;
    [SerializeField] float yRotate;
    int middle;
    bool middleSwitch = false;
    // Start is called before the first frame update
    void Start()
    {
        Spawner();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Spawner()
    {
        for (int i = 0; i < 21; i++)
        {
            for (int j = -middle; j < 1 + max; j++)
            {
                float addSpace = 0; ;
                if (j % 2 == 0 || j == 0) addSpace = xSpacing / 2;
                Vector3 spawnPosition = new Vector3(i * xSpacing + addSpace, 0, j * YSpacing);
                Vector3 spawnRotation = new Vector3(0, yRotate, 0);
                Instantiate(spawnObject, spawnPosition, Quaternion.Euler(spawnRotation));
            }

            if (middle == min) middleSwitch = true;
            if (middle == max) middleSwitch = false;

            if (middleSwitch) middle++;
            else middle--;

        }
    }
}
