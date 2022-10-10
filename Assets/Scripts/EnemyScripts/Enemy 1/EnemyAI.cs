using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : BaseEnemy
{
   
    //Movement speed
    public float horizontalSpeed;
   
    //rotation variables
    public float rotationSpeed;
    public Vector3 rotation;

    //Sinus wave variables 
    public float sinusAmplitude;
    public float frequency;

    //store Y position
    float startY;     
    public GameObject projectile;

    void Start()
    {
        startY = transform.position.y;
        StartCoroutine(enemyShootRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        //move forward
        transform.Translate(Vector3.forward * horizontalSpeed * Time.deltaTime);

        //Z rotation animation
        transform.Rotate(rotation * rotationSpeed * Time.deltaTime);

        //Sinus wave animation
        float x = transform.position.x;
        float y = startY + Mathf.Sin(Time.time) * sinusAmplitude;
        float z = transform.position.z;
        transform.position = new Vector3(x, y, z);


        Respawn();
    }

    //shoot
    IEnumerator enemyShootRoutine()
    {
        while (true)
        {
            float randomTime = Random.Range(3, 5);
            Vector3 rotation = new Vector3(0, 0, 90);
            Instantiate(projectile, transform.position, Quaternion.Euler(rotation));
            yield return new WaitForSeconds(randomTime);
        }
    }

}
