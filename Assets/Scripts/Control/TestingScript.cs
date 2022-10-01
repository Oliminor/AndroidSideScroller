using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingScript : MonoBehaviour
{

    public JoystickController test;

    public Rigidbody body;
    public float speed = 2f;

    public GameObject minXobj, maxXobj, minYobj, maxYobj;

    private void Start()
    {
        float minX = minXobj.transform.position.x;
        float maxX = maxXobj.transform.position.x;
        float minY = minYobj.transform.position.y;
        float maxY = maxYobj.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 dir = test.GetInputAxis();
        body.AddForce(dir * speed);

        float x = gameObject.transform.position.x;

        if (gameObject.transform.position.x > maxXobj.transform.position.x)
            gameObject.transform.Translate(minXobj.transform.position.x*2,0,0);

        if (gameObject.transform.position.y < minYobj.transform.position.y)
            gameObject.transform.Translate(0, maxYobj.transform.position.y, 0);

        if (gameObject.transform.position.x < minXobj.transform.position.x)
            gameObject.transform.Translate(maxXobj.transform.position.x*2, 0, 0);
    }
}

