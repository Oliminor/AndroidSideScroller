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

    float timerReset;

    private float xRot=0;
    private float minAngle = -45;
    private float maxAngle = 45;
    float rotationSpeed = 75.0f;
    Quaternion initialRotation;

    public float timer = 5.0f;

    Rigidbody body;

    public JoystickController inputManager;

    private void Start()
    {
        body = GetComponent<Rigidbody>();
        body.useGravity = false;
        timerReset = timer;
        initialRotation = transform.rotation;

        
        bulletInPool = new List<GameObject>();
        GameObject bullet;
        for (int i = 0; i < poolSize; i++)
        {
            bullet = Instantiate(poolThis);
            bullet.SetActive(false);
            bulletInPool.Add(bullet);

        }


    }

    private void FixedUpdate()
    {
        Vector2 moveDir = inputManager.GetInputAxis();
 

        Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);
        
        screenPos.x = Mathf.Clamp01(screenPos.x);
        screenPos.y = Mathf.Clamp01(screenPos.y);

        Vector3 speedTemp = body.velocity;

        if (screenPos.x == 0 || screenPos.x == 1)
            speedTemp.x= 0;
        if (screenPos.y == 0 || screenPos.y == 1)
            speedTemp.y = 0;

        transform.position = Camera.main.ViewportToWorldPoint(screenPos);
        body.velocity = speedTemp;
        
  
        speed = Mathf.Lerp(acceleration, speed, Time.deltaTime);
        body.AddForce(moveDir * speed);
        
        xRot += moveDir.y * rotationSpeed * Time.deltaTime;
        xRot = Mathf.Clamp(xRot, minAngle, maxAngle);


        if (moveDir.y != 0)
            transform.rotation = Quaternion.Euler(-xRot, 90f, 0f);
        //if(moveDir.y==0)
        //    transform.rotation= Quaternion.Slerp(transform.rotation, initialRotation, Time.deltaTime);


        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            StartCoroutine(AutoFire());
            timer = timerReset;
        }

        
    }
    IEnumerator AutoFire()
    {
        GameObject bullet = GetPooled();
        bullet.transform.SetPositionAndRotation(PlayerMuzzle.position, PlayerMuzzle.rotation);
        bullet.SetActive(true);
        
        yield return new WaitForSeconds(1);

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
