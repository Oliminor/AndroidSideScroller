using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
   
    //Movement speed
    public float horizontalSpeed;
   
    //rotation variables
    public float rotationSpeed;
    public Vector3 rotation;

    //Sinus wave variables 
    public float sinusAmplitude;
    public float frequency;

    float startY;

   [SerializeField]
    private GameObject enemyExplosion;

    private void Start()
    {
        startY = transform.position.y;
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

        //shot



        //respawn
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        Vector3 respawnPos = Camera.main.ViewportToWorldPoint(new Vector3(1.1f, transform.position.y, GameManager.instance.GetZPosition()));

        if (pos.x < -0.1)
        {
            transform.position = respawnPos;
        }

    }

    //die
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enemy destroyed");

        if (other.tag == "Shot" || other.tag == "Player")
        {
            Destroy(other.gameObject);
            Instantiate(enemyExplosion, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

}
