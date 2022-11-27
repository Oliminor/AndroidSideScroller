using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectiles : MonoBehaviour
{
    [SerializeField] float speed = 7.5f;
    [SerializeField] GameObject destroyParticle;
    [SerializeField] bool isHomingProjectile;
    bool wasTargeting=false;
    [SerializeField] Transform target;

    [SerializeField] LayerMask whatIsSolid;
    private Vector3 previousPosition;
    Rigidbody body;

    private void Start()
    {
        destroyParticle = Instantiate(destroyParticle, transform.position, Quaternion.identity);
        destroyParticle.GetComponent<ParticleSystem>().Stop();
    }
    private void OnEnable()
    {
        previousPosition = transform.localToWorldMatrix.GetPosition();

        if (isHomingProjectile)
        {
            if(EnemySpawner.instance.GetActiveEnemyList().Count>0)
            {
                body = gameObject.GetComponent<Rigidbody>();
                List<Transform> targetList = EnemySpawner.instance.GetActiveEnemyList(); // Here you go
                target = targetList[0];
            }
        }
    }
    void Update()
    {
        CheckBetweenTwoPositions();

        Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);

        screenPos.x = Mathf.Clamp01(screenPos.x);
        screenPos.y = Mathf.Clamp01(screenPos.y);
        if ( screenPos.x == 1)
            gameObject.SetActive(false);
        if (screenPos.y == 0 || screenPos.y == 1)
            gameObject.SetActive(false);
        transform.position = Camera.main.ViewportToWorldPoint(screenPos);

        if(Time.timeScale<1)
        {
            speed =15f;
        }
        else
        {
            speed = 7.5f;
        }

        if(isHomingProjectile)
        {
            if(target!=null)
            {
                float travel = (speed*2f) * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, target.position, travel);
                wasTargeting = true;
            }
            else
            {
                if(EnemySpawner.instance.GetActiveEnemyList().Count>0)
                {
                    target = EnemySpawner.instance.GetActiveEnemyList()[0];
                }
                else
                {
                    if (wasTargeting)
                    {
                        transform.Translate( transform.forward* speed * Time.deltaTime);
                    }
                    else
                    {
                        transform.Translate(Vector3.forward * speed * Time.deltaTime);
                    }
                }
                
            }
            
        }
        else
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
        
    }


    public void InstantiateDestroyEffect(Vector3 _explodePosition)
    {
        destroyParticle.transform.position = _explodePosition;
        destroyParticle.GetComponent<ParticleSystem>().Play();
    }

    private void CheckBetweenTwoPositions()
    {
        float distance = Vector3.Distance(previousPosition, transform.position);
        RaycastHit hit;
        if (Physics.SphereCast(previousPosition - transform.forward * distance * 10, 0.15f, transform.forward, out hit, distance * 10, whatIsSolid))
        {
            if (hit.transform.TryGetComponent(out IDamageable damageTarget))
            {
                damageTarget.TakeDamage();
            }
            InstantiateDestroyEffect(hit.point);
            gameObject.SetActive(false);
        }
        previousPosition = transform.localToWorldMatrix.GetPosition();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(isHomingProjectile)
        {
            if (other.tag == "Enemy")
            {
                if (other.TryGetComponent(out IDamageable damageTarget))
                {
                    damageTarget.TakeDamage();
                }
                InstantiateDestroyEffect(gameObject.transform.position);
                gameObject.SetActive(false);
            }
        }
    }
}
