using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    //health and body 
    [SerializeField] Transform projectileParent;
    [SerializeField] GameObject explodeParticle;
    [SerializeField] int maxLives = 3;
    [SerializeField] float shieldTime;
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
    [SerializeField] int barrelNumber;
    [SerializeField] int angleNumber;
    int startBarrels = 1;
    int startAngles = 0;
    [SerializeField] private bool isProtected;
    [SerializeField] MeshRenderer playerMat;
    [SerializeField] MeshRenderer shieldMat;
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
    float currentFireRate;
    Quaternion initialRotation;
    bool isBulletTimeOn = false;
    Vector3 startPosition;

    private float shieldLerp = 1;
    private float shieldDiss = 1;

    public int GetLives() { return lives; }

    private void Awake()
    {
        lives = maxLives;
        GameManager.instance.SetPlayer(this);
    }

    private void Start()
    {
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
            bullet.transform.parent = projectileParent;
        }

        //start shooting coroutine
        StartCoroutine(AutoFire());
    }

    private void Update()
    {
        FollowTouchControllerMovement();
    }

    private void FixedUpdate()
    {
        ShieldLerp();

        if (PlayerSettings.instance.ControllerType != 0) return;

        if (bulletTimeisOn)
        {
            Time.timeScale = 0.5f;
            acceleration *= 4;
        }
        else
        {
            Time.timeScale = 1.0f;
        }


        if (lives <= 0 || GameManager.instance.GetIsLevelEnded())
        {
            transform.position = Vector3.Lerp(transform.position, startPosition, 0.01f);
            transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
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

    private void FollowTouchControllerMovement()
    {
        if (PlayerSettings.instance.ControllerType != 1) return;

        Vector3 arrowPosition = new Vector3(FollowTouchController.instance.ArrowPosition.x, FollowTouchController.instance.ArrowPosition.y, GameManager.instance.GetZPosition());

        Vector3 arrowPos = Camera.main.ScreenToWorldPoint(arrowPosition);

        transform.position = Vector3.MoveTowards(transform.position, arrowPos, Time.deltaTime * topSpeed * 2);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag=="Enemy" || other.tag == "Obstacle")//ill change this just testing things 
        {
            TakeDamage();
        }
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
        if (GameManager.instance.GetIsLevelEnded() || GameManager.instance.GetIsLevelEnded()) return;

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
            transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
            StartCoroutine(Invulnerability(false, 3));
            StartCoroutine(RespawnEffect());

            if (lives <= 0)
            {
                GamePlayUI.instance.TriggerGameOver();
                StartCoroutine(Invulnerability(false, 1000));
            }
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

        yield return new WaitForSeconds(currentFireRate / 2);

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
    }

    public void AddBarrel() { barrelNumber += 1; }

    public void AddAngle() { angleNumber += 1; }

    public void AddLife() 
    {
        if (lives >= maxLives || lives <= 0) return;
        lives += 1;
        GamePlayUI.instance.AddHealth();
    }

    public void ShootFaster() { currentFireRate /= 2; }

    public void ActivateShield()
    {
        StartCoroutine(Invulnerability(true, shieldTime));
    }
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
