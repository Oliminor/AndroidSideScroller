using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] bool isGodModeOn = false;

    [Header("Health and Body")]//health and body 
    [SerializeField] Transform projectileParent;
    [SerializeField] GameObject explodeParticle;
    [SerializeField] int maxLives = 3;
    [SerializeField] float shieldTime;
    Rigidbody body;

    
    [Header("Movement speed")]//speed stuff
    [SerializeField] float reverseMultiplier;
    [SerializeField] float topSpeed=100f;
    [SerializeField] float TopAcceleration;
    float acceleration;

    //weapon stuff
    [SerializeField] Transform PlayerMuzzle;

    [Header("Object pools")] //bullet pool
    [SerializeField] List<GameObject> bulletInPool = new();
    [SerializeField] GameObject poolThis;
    [SerializeField] int poolSize;

    //cone bullet pool
    [SerializeField] List<GameObject> coneBulletInPool = new();
    [SerializeField] GameObject conePoolBullet;
    [SerializeField] int conePoolSize;

    [SerializeField] List<GameObject> homingBulletInPool = new();
    [SerializeField] GameObject homingPoolBullet;
    [SerializeField] int homingPoolSize;

    [Header("Laser")]
    [SerializeField] GameObject LaserProjectile;
    [SerializeField] float laserActiveTime;

    [Header("Powerup Modes")]//powerup modes
    [SerializeField] float initialFireRate = 2;
    [SerializeField] int barrelNumber;
    [SerializeField] int angleNumber;
    int startBarrels = 1;
    int startAngles = 0;
    [SerializeField] private bool isProtected;
    [SerializeField] MeshRenderer playerMat;
    [SerializeField] MeshRenderer shieldMat;
    private bool bulletTimeisOn;
    [SerializeField] private bool coneShot;
    [SerializeField] int coneShotAngle = 5;
    
    private enum ShootingPatternSwitch { STANDARDPATTERN, CONEPATTERN, HOMINGPATTERN, LASERPATTERN};
    private ShootingPatternSwitch shootingPatternSwitch;

    [Header("Powerup Time and number restrictions")]
    [SerializeField] float anglePowerupTime;
    [SerializeField] float barrelPowerupTime;
    [SerializeField] float coneShootingTime;
    [SerializeField] int maxBarrels;
    [SerializeField] int maxAngles;

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
        shootingPatternSwitch = ShootingPatternSwitch.STANDARDPATTERN;
    }

    private void Start()
    {
        LaserProjectile.SetActive(false);

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

        //instatiate bullets and add to pool
        coneBulletInPool = new List<GameObject>();
        GameObject coneBullet;
        for (int i = 0; i < conePoolSize; i++)
        {
            coneBullet = Instantiate(conePoolBullet);
            coneBullet.SetActive(false);
            coneBulletInPool.Add(coneBullet);
            coneBullet.transform.parent = projectileParent;
        }

        homingBulletInPool = new List<GameObject>();
        GameObject homingBullet;
        for (int i = 0; i < poolSize; i++)
        {
            homingBullet = Instantiate(homingPoolBullet);
            homingBullet.SetActive(false);
            homingBulletInPool.Add(homingBullet);
            homingBullet.transform.parent = projectileParent;
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
           
            yield return new WaitForSeconds(coneShot?currentFireRate/4:currentFireRate); //less delay if the cone shooting is active, otherwise normal fire rate delay (/2 just for testing ofc needs nicer numners) 
            
            
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

    private GameObject GetPooledConeBullet()
    {
        for (int i = 0; i < conePoolSize; i++)
        {
            if (!coneBulletInPool[i].activeInHierarchy)
            {
                return coneBulletInPool[i];
            }
        }
        return null;
    }

    private GameObject GetPooledHomingBullet()
    {
        for (int i = 0; i < homingPoolSize; i++)
        {
            if (!homingBulletInPool[i].activeInHierarchy)
            {
                return homingBulletInPool[i];
            }
        }
        return null;
    }

    public void TakeDamage()
    {
        if (GameManager.instance.GetIsLevelEnded() || GameManager.instance.GetIsLevelEnded()) return;

        if (isGodModeOn) return;

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
            FollowTouchController.instance.SetPosition();

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
        GameObject bullet;
        GameObject coneBullet;
        GameObject homingBullet;

        switch (shootingPatternSwitch)
        {
            case ShootingPatternSwitch.STANDARDPATTERN:                      // BARRELS AND ANGLES CASE- STANDARD SHOOTING PATTERNS 
                for (int i = startAngles; i < angleNumber; i++)
                {
                    int angleUp = (i + 1) * 5;
                    int angleDown = (i + 1) * -5;
                    float playerXrotation = PlayerMuzzle.rotation.eulerAngles.x;

                    bullet = GetPooledConeBullet();
                    bullet.transform.SetPositionAndRotation(PlayerMuzzle.position, Quaternion.Euler(playerXrotation + angleUp, 90, 0));
                    bullet.SetActive(true);

                    bullet = GetPooledConeBullet();
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
                break;
            case ShootingPatternSwitch.CONEPATTERN:                      // NON STANDARD PATTERN- 3 BULLETS IN A TRIANGULAR SHAPE 
                {
                    float playerXrotation = PlayerMuzzle.rotation.eulerAngles.x;

                    coneBullet = GetPooledConeBullet();
                    coneBullet.transform.SetPositionAndRotation(PlayerMuzzle.position, Quaternion.Euler(playerXrotation + coneShotAngle, 90, 0));
                    coneBullet.SetActive(true);

                    coneBullet = GetPooledConeBullet();
                    coneBullet.transform.SetPositionAndRotation(PlayerMuzzle.position, Quaternion.Euler(playerXrotation - coneShotAngle, 90, 0));
                    coneBullet.SetActive(true);

                    bullet = GetPooled();
                    Vector3 spawnPos = new Vector3(PlayerMuzzle.position.x, PlayerMuzzle.position.y, PlayerMuzzle.position.z);
                    bullet.transform.SetPositionAndRotation(spawnPos, PlayerMuzzle.rotation);
                    bullet.SetActive(true);

                    yield break;
                }

            case ShootingPatternSwitch.HOMINGPATTERN:
                {
                    homingBullet = GetPooledHomingBullet();
                    homingBullet.transform.SetPositionAndRotation(PlayerMuzzle.position, PlayerMuzzle.rotation);
                    homingBullet.SetActive(true);
                }
                break;
            case ShootingPatternSwitch.LASERPATTERN:
                break;
        }

    }

    public void AddBarrel() { StartCoroutine(AddRemoveBarrel()); }

    public void AddAngle() { StartCoroutine(AddRemoveAngle()); }

    public void ActivatePlayerLaser() { StartCoroutine(ActicateLaser()); }

    private IEnumerator ActicateLaser()
    {
        LaserProjectile.SetActive(true);

        yield return new WaitForSeconds(laserActiveTime);

        LaserProjectile.SetActive(false);
    }

    private IEnumerator AddRemoveAngle()
    {
        if(angleNumber<maxAngles)
        {
            angleNumber++;
        }
        else
        {
            yield break;
        }
        
        yield return new WaitForSeconds(anglePowerupTime);

        if(angleNumber>startAngles)
        {
            angleNumber--;
        }
        
    }
    private IEnumerator AddRemoveBarrel()
    {
        if(barrelNumber<maxBarrels)
        {
            barrelNumber ++;
        }
        else
        {
            yield break;
        }

        yield return new WaitForSeconds(barrelPowerupTime);

        if(barrelNumber>startBarrels)
        {
            barrelNumber--;        
        }
    }

    public void AddLife() 
    {
        if (lives >= maxLives || lives <= 0) return;
        lives += 1;
        GamePlayUI.instance.AddHealth();
    }

    public void ShootFaster() { currentFireRate /= 2; }

    public void ConeShoot() { StartCoroutine(ConeShootingTimer(coneShootingTime)); } //know you will probably look at this ben i tried to mimic what you did for the shield

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

    public void ActivateHoming()
    {
        StartCoroutine(HomingMissiles(coneShootingTime));
    }
    private IEnumerator HomingMissiles(float _shootingTime) // assuming we want this pattern specifically to add to the current time rather than run in parallel
    {
        float time = _shootingTime;

        while (time > 0)
        {
            shootingPatternSwitch = ShootingPatternSwitch.HOMINGPATTERN;
            time -= Time.deltaTime;
            yield return null;
        }
        shootingPatternSwitch = ShootingPatternSwitch.STANDARDPATTERN;

    }

    private IEnumerator ConeShootingTimer(float _shootingTime) // assuming we want this pattern specifically to add to the current time rather than run in parallel
    {
        float time = _shootingTime;

        while (time > 0)
        {
            shootingPatternSwitch = ShootingPatternSwitch.CONEPATTERN;
            time -= Time.deltaTime;
            yield return null;
        }
        shootingPatternSwitch = ShootingPatternSwitch.STANDARDPATTERN;

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
