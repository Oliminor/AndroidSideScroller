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
    [SerializeField] float incrementalAdditionalSpeedStep;
    [SerializeField] float exponentialStep;
    [SerializeField] bool exponentialSpeed = false;

    [SerializeField] bool oldSchoolMovement = false;
    [SerializeField] float stepSize;
    [SerializeField] float stepTime;

    [SerializeField] bool random90Rotation;
    float startY;

    private void Start()
    {
        Random90Rotation();

        startY = transform.position.y;

        if (oldSchoolMovement) StartCoroutine(RetroMovement());
    }

    // Update is called once per frame
    void Update()
    {
        if (oldSchoolMovement) return;

        transform.Rotate(rotation, Time.deltaTime * rotationSpeed);
        float sinY = startY + Mathf.Sin(Time.time * sinFrequency) * sinValue;
        transform.Translate(movement * movementSpeed * Time.deltaTime, Space.World);
        transform.position = new Vector3(transform.position.x, sinY, transform.position.z);

        movementSpeed += incrementalAdditionalSpeedStep;
        if (exponentialSpeed) movementSpeed *= exponentialStep;
    }

    public void Random90Rotation()
    {
        if (random90Rotation)
        {
            int rotation = Random.Range(0, 4);
            transform.rotation = Quaternion.Euler(new Vector3(rotation * 90, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z));
        }
    }

    IEnumerator RetroMovement()
    {
        while(true)
        {
            yield return new WaitForSeconds(stepTime);
            transform.position += movement.normalized * stepSize;
        }
    }
}
