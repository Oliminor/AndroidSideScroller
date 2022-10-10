using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform PlayerMuzzle;

    [SerializeField] float reverseMultiplier;
    [SerializeField] float topSpeed;

    [SerializeField] float acceleration;

    public float TopAcceleration;

    
    public List<GameObject> bulletInPool;
    public GameObject poolThis;
    public int poolSize;

    float timerReset;

    private float xRot=0;
    private float minAngle = -15;
    private float maxAngle = 15;
    float rotationSpeed = 75.0f;
    Quaternion initialRotation;

    public float timer = 5.0f;
    bool isHolding;

    Rigidbody body;

    public JoystickController inputManager;

    private void Start()
    {
        GameManager.instance.SetPlayer(this.gameObject);

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
        isHolding = inputManager.ReturnHoldStatus();

        Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);

        screenPos.x = Mathf.Clamp01(screenPos.x);
        screenPos.y = Mathf.Clamp01(screenPos.y);
        Vector3 speedTemp = body.velocity;
        if (screenPos.x == 0 || screenPos.x == 1)
            speedTemp.x = 0;
        if (screenPos.y == 0 || screenPos.y == 1)
            speedTemp.y = 0;
        transform.position = Camera.main.ViewportToWorldPoint(screenPos);





        body.AddForce(moveDir * acceleration);

        if (isHolding)
        {
            acceleration = inputManager.GetInputAxis().magnitude * TopAcceleration;
            xRot += (body.velocity.y / 3) * rotationSpeed * Time.deltaTime;
            xRot = Mathf.Clamp(xRot, minAngle, maxAngle);
            transform.rotation = Quaternion.Euler(-xRot, 90f, 0f);
        }
        if (!isHolding)
        {
            acceleration = Mathf.Lerp(acceleration, 0, 0.2f);
            float currentAngle = transform.rotation.eulerAngles.x;
            //Debug.Log(currentAngle);
            transform.rotation = Quaternion.Slerp(transform.rotation, initialRotation, Time.deltaTime * reverseMultiplier);
            xRot = currentAngle - transform.rotation.eulerAngles.x + xRot;
            body.AddForce(-body.velocity * reverseMultiplier);

        }

        Vector3 velocity = new Vector3(body.velocity.x, body.velocity.y, body.velocity.z);

        if (velocity.magnitude > topSpeed)
        {
            Vector3 limit = velocity.normalized * topSpeed;
            body.velocity = new Vector3(limit.x, limit.y, limit.z);
        }
    }
    IEnumerator AutoFire()
    {
        while (true)
        {
            GameObject bullet = GetPooled();
            bullet.transform.SetPositionAndRotation(PlayerMuzzle.position, PlayerMuzzle.rotation);
            bullet.SetActive(true);

            yield return new WaitForSeconds(0.3f);
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
