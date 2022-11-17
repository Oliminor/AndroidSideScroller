using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectiles : MonoBehaviour
{
    [SerializeField] float speed = 7.5f;
    [SerializeField] GameObject destroyParticle;
    [SerializeField] LayerMask whatIsSolid;

    private Vector3 previousPosition;

    private void Start()
    {
        previousPosition = transform.position;
        destroyParticle = Instantiate(destroyParticle, transform.position, Quaternion.identity);
        destroyParticle.GetComponent<ParticleSystem>().Stop();
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

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
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
        if (Physics.Raycast(previousPosition, transform.forward, out hit, distance, whatIsSolid))
        {
            if (hit.transform.TryGetComponent(out IDamageable damageTarget))
            {
                damageTarget.TakeDamage();
            }

            InstantiateDestroyEffect(hit.point);
            gameObject.SetActive(false);
        }
        previousPosition = transform.position;
    }

}
