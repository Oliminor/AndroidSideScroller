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

    private void Awake()
    {
        GameManager.instance.SetPlayer(this.gameObject);
    }

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
        StartCoroutine(AutoFire());

    }

    private void FixedUpdate()
    {
        Vector2 moveDir = inputManager.GetInputAxis();

        if (moveDir == Vector2.zero)
        {
            speed = Mathf.Lerp(speed, 0, 0.5f);
        }
        speed = Mathf.Lerp(speed, 4, 0.5f);

        body.velocity = moveDir * speed;
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
