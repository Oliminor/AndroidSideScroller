using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{

    public Vector3 rotation;

    // Update is called once per frame
    void Update()
    {

        //X rotation animation
        transform.Rotate(rotation * Time.deltaTime);

    }
}
