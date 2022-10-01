using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingScript : MonoBehaviour
{

    public TapHandler test;


    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position +=new Vector3(test.GetInputAxis().x,test.GetInputAxis().y,0);  
    }
}
