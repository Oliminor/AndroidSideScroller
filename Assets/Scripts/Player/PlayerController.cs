using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    //health and body 
    [SerializeField] GameObject explodeParticle;
    [SerializeField] int maxLives = 3;
    Rigidbody body;

    //speed stuff
    [SerializeField] float reverseMultiplier;
    [SerializeField] float topSpeed=100f;
    [SerializeField] float TopAcceleration;
    float acceleration;

    //weapon stuff
    [SerializeField] Transform PlayerMuzzle;

    //bullet pool
    [SerializeField] List<GameObject> bulletInPool = new();
    [SerializeField] GameObject poolThis;
    [SerializeField] int poolSize;

    //powerup modes
    [SerializeField] float initialFireRate = 2;
    [SerializeField] float currentFireRate;
    [SerializeField] int barrelNumber;
    [SerializeField] int angleNumber;
    int startBarrels = 1;
    int startAngles = 0;
    [SerializeField] private bool isProtected;
    [SerializeField] MeshRenderer playerMat;
    [SerializeField] MeshRenderer shieldMat;
    [SerializeField] float shieldTime;
    private bool bulletTimeisOn;


    int lives;

    //player input
    bool isHolding;
    public JoystickController inputManager;

    //rotation management
    private float xRot=0;
    private float minAngle = -15;
    private float maxAngle = 15;
    private float rotationSpeed = 75.0f;
    Quaternion initialRotation;
    bool isBulletTimeOn = false;
    Vector3 startPosition;

    private float shieldLerp = 1;
    private float shieldDiss = 1;

    public int GetLives() { return lives; }

    private void Awake()
    {
        GameManager.instance.SetPlayer(this);
    }

    private void Start()
    {
        lives = maxLives;
        startPosition = transform.position;
        body = GetComponent<Rigidbody>();
        body.useGravity = false;
        
        initialRotation = transform.rotation;
        
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) StartCoroutine(Invulnerability(true, shieldTime));

    }

    private void FixedUpdate()
    {
        ShieldLerp();

        if (bulletTimeisOn)
        {
            Time.timeScale = 0.5f;
            acceleration *= 4;
        }
        else
        {
            Time.timeScale = 1.0f;
        }


        if (lives <= 0)
        {
            body.velocity = Vector3.zero;
            return;
        }

        if (GameManager.instance.GetIsPaused()) return;

        Vector2 moveDir = inputManager.GetInputAxis();
        isHolding = inputManager.ReturnHoldStatus();

        Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);

        screenPos.x = Mathf.Clamp01(screenPos.x);
        screenPos.y = Mathf.Clamp01(screenPos.y);

        Vector3 speedTemp = body.velocity;

        float bumbValue = 2;

        if (screenPos.x == 0)
            speedTemp.x = bumbValue;
        if (screenPos.x == 1)
            speedTemp.x = -bumbValue;
        if (screenPos.y == 0)
            speedTemp.y = bumbValue;
        if (screenPos.y == 1)
            speedTemp.y = -bumbValue;

        body.velocity = speedTemp;

        transform.position = Camera.main.ViewportToWorldPoint(screenPos);

        if (isBulletTimeOn) acceleration *= 4;

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
            GameObject go = Instantiate(explodeParticle, transform.position, Quaternion.identity);
            Destroy(go, 1.5f);
            barrelNumber = startBarrels;
            angleNumber = startAngles;
            currentFireRate = initialFireRate;
            GamePlayUI.instance.RemoveHealth();
            transform.position = startPosition;
            StartCoroutine(Invulnerability(false, 3));
            StartCoroutine(RespawnEffect());
        }
        else return;

        if (lives <= 0)
        {
            GamePlayUI.instance.TriggerGameOver();
            //player death logic 
        }
    }

    IEnumerator RespawnEffect()
    {
        while (isProtected)
        {
            playerMat.material.SetFloat("_Alpha", 1);
            yield return new WaitForSeconds(0.25f);
            playerMat.material.SetFloat("_Alpha", 0);
            yield return new WaitForSeconds(0.25f);
        }
    }


    IEnumerator FiringPattern()
    {
        GameObject bullet = GetPooled();

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

        for (int i = 0; i < barrelNumber; i++)
        {
            float gapsize = 0.12f;
            float displacementUp = i * gapsize; // changethis : changethis  for a larger/smaller offset between bullets

            float pushDown = barrelNumber * gapsize / 2;

            bullet = GetPooled();
            Vector3 spawnPos = new Vector3(PlayerMuzzle.position.x, PlayerMuzzle.position.y + displacementUp - pushDown, PlayerMuzzle.position.z);
            bullet.transform.SetPositionAndRotation(spawnPos, PlayerMuzzle.rotation);
            bullet.SetActive(true);
        }


        yield return new WaitForSeconds(0.01f);
    }

    public void AddBarrel() { barrelNumber += 1; }

    public void AddAngle() { angleNumber += 1; }

    public void AddLife() 
    {
        if (lives >= maxLives) return;
        lives += 1;
        GamePlayUI.instance.AddHealth();
    }

    public void ShootFaster() { currentFireRate /= 2; }

    public IEnumerator Invulnerability(bool isShield, float _shieldTime)
    {

        float time = _shieldTime;

        while (time > 0)
        {
            isProtected = true;
            if (isShield) shieldLerp = 0;
            time -= Time.deltaTime;
            yield return null;
        }

        isProtected = false;
        if (isShield) shieldLerp = 1;
    }

    void ShieldLerp()
    {
        shieldDiss = Mathf.Lerp(shieldDiss, shieldLerp, 0.1f);

        shieldMat.material.SetFloat("_dissAmount", shieldDiss);
    }

    public IEnumerator BulletTime()
    {
        bulletTimeisOn = true;
        yield return new WaitForSeconds(0.3f);
        bulletTimeisOn = false;
    }
}
