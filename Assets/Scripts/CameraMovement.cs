using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public static CameraMovement instance;
    [SerializeField] float speed;

    Vector3 startPos;
    Vector3 tempPos;

    public float GetSpeed() { return speed; }
    public void SetSpeed(float _speed) { speed = _speed; }

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        startPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(speed * new Vector3(1, 0, 0) * Time.deltaTime);
    }

    public float GetDistance()
    {
        float distance = Vector3.Distance(startPos, transform.position);

        return distance;
    }

    public float GetDeltaDistance()
    {
        float distance = Vector3.Distance(tempPos, transform.position);

        tempPos = transform.position;
        return distance;
    }
}
