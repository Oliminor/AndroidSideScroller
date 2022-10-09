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
    [SerializeField] List<GameObject> bulletInPool;
    public GameObject poolThis;
    [SerializeField] int poolSize;

    //powerup modes
    [SerializeField] float initialFireRate = 2;
    [SerializeField] float currentFireRate;
    [SerializeField] int barrelNumber;
    [SerializeField] int angleNumber;
    int startBarrels = 1;
    int startAngles = 0;
    [SerializeField] private bool isProtected;



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

    private void Start()
    {
        GameManager.instance.SetPlayer(this.gameObject);
        
        body = GetComponent<Rigidbody>();
        body.useGravity = false;
        

        initialRotation = transform.rotation;
        
        lives = 3;
        barrelNumber = startBarrels;
        angleNumber = startAngles;
        currentFireRate = initialFireRate;

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


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag=="Enemy")//ill change this just testing things 
        {
            TakeDamage();
        }
        Debug.Log("test");
    }

    IEnumerator AutoFire()
    {
        while (true)
        {

            StartCoroutine(FiringPattern());
            
            yield return new WaitForSeconds(currentFireRate);
        }

    }
    private GameObject GetPooled()
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
        if (isProtected == false) //if no shield is up, take damage and reset barrel count
        {
            lives--;
            barrelNumber = startBarrels;
            angleNumber = startAngles;
            currentFireRate = initialFireRate;
        }
        else
            return;

        if (lives <= 0)
        {
            //player death logic 
        }
    }


    IEnumerator FiringPattern()
    {
        GameObject bullet = GetPooled();
        bullet.transform.SetPositionAndRotation(PlayerMuzzle.position, PlayerMuzzle.rotation);
        bullet.SetActive(true);

        for (int i = startAngles; i < angleNumber; i++)
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

        for (int i = startBarrels; i < barrelNumber; i++)
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

    public void AddBarrel() { barrelNumber += 1; }

    public void AddAngle() { angleNumber += 1; }

    public void AddLife() { lives += 1; }

    public void ShootFaster() { currentFireRate /= 2; }

    public IEnumerator Invulnerability()
    {
        isProtected = true;

        yield return new WaitForSeconds(3);

        isProtected = false;
    }

}
