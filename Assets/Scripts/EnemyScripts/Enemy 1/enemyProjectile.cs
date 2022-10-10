using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyProjectile : MonoBehaviour
{

    [SerializeField] float speed;
    [SerializeField] float life;
    [SerializeField] GameManager particleEffect;


    private void Start()
    {
        Destroy(this.gameObject, life);
    }

    void Update()
    {
        //move forward
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameManager.instance.GetPlayer())
        {
            if (particleEffect != null) Instantiate(particleEffect, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
