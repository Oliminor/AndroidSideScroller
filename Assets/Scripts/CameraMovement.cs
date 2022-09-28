using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public static CameraMovement instance;
    [SerializeField] float speed;

    public float GetSpeed() { return speed; }
    public void SetSpeed(float _speed) { speed = _speed; }

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(speed * new Vector3(1, 0, 0) * Time.deltaTime);
    }
}
