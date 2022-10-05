using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    //health and body 
    int lives;
    Rigidbody body;

    //speed stuff
    [SerializeField] float reverseMultiplier;
    [SerializeField] float topSpeed=100f;
    [SerializeField] float acceleration;
    public float TopAcceleration;

    //weapon stuff
    public Transform PlayerMuzzle;

    //bullet pool
    public List<GameObject> bulletInPool;
    public GameObject poolThis;
    public int poolSize;

    //powerup shoot modes
    [SerializeField] int barrelNumber;
    [SerializeField] int angleNumber;


    //player input
    Vector2 moveDir;
    bool isHolding;
    public JoystickController inputManager;

    //rotation management
    private float xRot=0;
    private float minAngle = -15;
    private float maxAngle = 15;
    float rotationSpeed = 75.0f;
    Quaternion initialRotation;

    float timer = 2.0f;

    private void Start()
    {
        GameManager.instance.SetPlayer(this.gameObject);
        
        body = GetComponent<Rigidbody>();
        body.useGravity = false;
        initialRotation = transform.rotation;
        
        lives = 3;
        barrelNumber = 1;
        angleNumber = 0;

        //instatiate bullets and add to pool
        bulletInPool = new List<GameObject>();
        GameObject bullet;
        for (int i = 0; i < poolSize; i++)
        {
            bullet = Instantiate(poolThis);
            bullet.SetActive(false);
            bulletInPool.Add(bullet);

        }

        //start shooting coroutine
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

        //shooting timer: double


        //shooting timer: cone

    }
    IEnumerator AutoFire()
    {
        while (true)
        {

            StartCoroutine(FiringPattern());
            
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
   

    public void TakeDamage()
    {
        lives--;
        if(lives<=0)
        {
            //player death logic 
        }
    }

    IEnumerator FiringPattern()
    {
        GameObject bullet = GetPooled();
        bullet.transform.SetPositionAndRotation(PlayerMuzzle.position, PlayerMuzzle.rotation);
        bullet.SetActive(true);

        for (int i = 0; i < angleNumber; i++)
        {
            int angleUp = (i + 1) * 5;
            int angleDown = (i + 1) * -5;
            float playerXrotation = PlayerMuzzle.rotation.eulerAngles.x;

            bullet = GetPooled();
            bullet.transform.SetPositionAndRotation(PlayerMuzzle.position, Quaternion.Euler(playerXrotation + angleUp, 90, 0));
            bullet.SetActive(true);

            bullet = GetPooled();
            bullet.transform.SetPositionAndRotation(PlayerMuzzle.position, Quaternion.Euler(playerXrotation + angleDown, 90, 0));
            bullet.SetActive(true);
        }

        yield return new WaitForSeconds(3.0f);

        for (int i = 0; i < barrelNumber; i++)
            {
                float displacementUp = (i + 1) * 0.15f; // changethis : changethis  for a larger/smaller offset between bullets
                float displacementDown = (i + 1) * -0.15f;


                bullet = GetPooled();
                Vector3 spawnPos = new Vector3(PlayerMuzzle.position.x, PlayerMuzzle.position.y + displacementUp, PlayerMuzzle.position.z);
                bullet.transform.SetPositionAndRotation(spawnPos, PlayerMuzzle.rotation);
                bullet.SetActive(true);

                bullet = GetPooled();
                spawnPos = new Vector3(PlayerMuzzle.position.x, PlayerMuzzle.position.y + displacementDown, PlayerMuzzle.position.z);
                bullet.transform.SetPositionAndRotation(spawnPos, PlayerMuzzle.rotation);
                bullet.SetActive(true);
            }
            
        
        yield return new WaitForSeconds(0.01f);
    }
}
