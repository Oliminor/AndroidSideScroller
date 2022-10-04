using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform PlayerMuzzle;

    public float speed;
    public float acceleration;

    
    public List<GameObject> bulletInPool;
    public GameObject poolThis;
    public int poolSize;

    bool isHolding;

    private float xRot=0;
    private float minAngle = -45;
    private float maxAngle = 45;
    float rotationSpeed = 75.0f;

    Quaternion initialRotation;

    Rigidbody body;

    public JoystickController inputManager;

    private void Start()
    {
        body = GetComponent<Rigidbody>();
        body.useGravity = false;
        initialRotation = transform.rotation;

        bulletInPool = new List<GameObject>();
        GameObject bullet;
        for (int i = 0; i < poolSize; i++)
        {
            bullet = Instantiate(poolThis);
            bullet.SetActive(false);
            bulletInPool.Add(bullet);

        }

        StartCoroutine(AutoFire());
    }

    private void FixedUpdate()
    {
        Vector2 moveDir = inputManager.GetInputAxis();
        isHolding = inputManager.ReturnHoldStatus();
 

        Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);
        
        screenPos.x = Mathf.Clamp01(screenPos.x);
        screenPos.y = Mathf.Clamp01(screenPos.y);

        Vector3 speedTemp = body.velocity;

        if (screenPos.x == 0 || screenPos.x == 1)
            speedTemp.x= 0;
        if (screenPos.y == 0 || screenPos.y == 1)
            speedTemp.y = 0;

        transform.position = Camera.main.ViewportToWorldPoint(screenPos);
        
  
        speed = Mathf.Lerp(acceleration, speed, Time.deltaTime);
        body.AddForce(moveDir * speed);



        if (isHolding)
        {

            xRot += moveDir.y * rotationSpeed * Time.deltaTime;
            xRot = Mathf.Clamp(xRot, minAngle, maxAngle);
            transform.rotation = Quaternion.Euler(-xRot, 90f, 0f);  
        }
        if(!isHolding)
        {
            float currentAngle = transform.rotation.eulerAngles.x;
            Debug.Log(currentAngle);
            transform.rotation = Quaternion.Slerp(transform.rotation, initialRotation, Time.deltaTime);
            xRot = currentAngle - transform.rotation.eulerAngles.x+xRot;
            body.AddForce(-body.velocity);
            
        }

    
        //Debug.Log(difference);

        
    }
    IEnumerator AutoFire()
    {
        while (true)
        {
            GameObject bullet = GetPooled();
            bullet.transform.SetPositionAndRotation(PlayerMuzzle.position, PlayerMuzzle.rotation);
            bullet.SetActive(true);
            yield return new WaitForSeconds(1);
        }

    }
    public GameObject GetPooled()
    {
        for (int i = 0; i < poolSize; i++)
        {
            if (!bulletInPool[i].activeInHierarchy)
            {
                return bulletInPool[i];
            }
        }
        return null;
    }
}
