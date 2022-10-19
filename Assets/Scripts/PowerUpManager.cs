using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager instance;

    public List<GameObject> availiblePowerups = new();
    [SerializeField] GameObject[] powerUpTypes;
    [SerializeField] int numberOfEachPowerupType;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        availiblePowerups = AvailablePowerupList();
    }

    private List<GameObject> AvailablePowerupList()
    {
        List<GameObject> containedList = new();
        foreach (var powerUp in powerUpTypes)
        {
            for (int i = 0; i < numberOfEachPowerupType; i++)
            {
                containedList.Add(powerUp);
            }
        }

        Shuffle(containedList);
        return containedList;

    }
    private void Shuffle(List<GameObject> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i);
            GameObject temp = list[i];
            list[i] = list[rnd];
            list[rnd] = temp;
        }
    }

    public void ClearAvailiblePowerUps() { availiblePowerups.Clear(); }
}
