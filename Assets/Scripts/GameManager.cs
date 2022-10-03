using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] float speed;
    public static GameManager instance;
    // Start is called before the first frame update

    public float GetSpeed() { return speed; }

    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
