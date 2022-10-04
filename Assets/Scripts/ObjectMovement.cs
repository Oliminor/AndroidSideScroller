using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMovement : MonoBehaviour
{
    [SerializeField] Vector3 rotation;
    [SerializeField] float rotationSpeed;
    [SerializeField] Vector3 movement;
    [SerializeField] float movementSpeed;
    [SerializeField] float sinValue;
    [SerializeField] float sinFrequency;
    float startY;

    private void Start()
    {
        startY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotation, Time.deltaTime * rotationSpeed);
        float sinY = startY + Mathf.Sin(Time.time * sinFrequency) * sinValue;
        transform.Translate(movement * movementSpeed * Time.deltaTime ,Space.World);
        transform.position = new Vector3(transform.position.x, sinY, transform.position.z);
    }
}
