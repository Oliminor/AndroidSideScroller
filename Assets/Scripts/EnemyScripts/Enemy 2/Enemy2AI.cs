using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2AI : BaseEnemy
{
    //Movement speed
    public float horizontalSpeed;

    // Update is called once per frame
    void Update()
    {
        //move forward
        transform.Translate(Vector3.left * horizontalSpeed * Time.deltaTime);
    }
}


