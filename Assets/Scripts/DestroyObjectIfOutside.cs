using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObjectIfOutside : MonoBehaviour
{
    void Update()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

        if (pos.x < -0.5)
        {
            Destroy(gameObject);
        }
    }
}
