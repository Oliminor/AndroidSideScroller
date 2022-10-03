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
    public int sinusAmplitude;
    public float frequency;

   [SerializeField]
    private GameObject enemyExplosion;


    // Update is called once per frame
    void Update()
    {
        //move forward
        transform.Translate(Vector3.forward * horizontalSpeed * Time.deltaTime);

        //Z rotation animation
        transform.Rotate(rotation * rotationSpeed * Time.deltaTime);

        //Sinus wave animation
        float x = transform.position.x;
        float y = Mathf.Sin(Time.time) * sinusAmplitude;
        float z = transform.position.z;
        transform.position = new Vector3(x, y, z);

        //shot

        

        //respawn
        if (transform.position.x < -11) 
        {
            //Assuming camera position is (0, 0, 0)
            float randomY = Random.Range(-5, 3);
            transform.position = new Vector3(14.0f, randomY, 2.5f);
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
