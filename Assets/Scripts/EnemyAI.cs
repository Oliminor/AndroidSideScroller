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

    //Other Game Objects
   [SerializeField]
    private GameObject enemyExplosion;
    [SerializeField]
    private GameObject drop;
    
    public GameObject projectile;

    void Start()
    {
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
        float y = Mathf.Sin(Time.time) * sinusAmplitude;
        float z = transform.position.z;
        transform.position = new Vector3(x, y, z);
        

        //respawn if out of the screen
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


            //random drop
            int randomNumber = Random.Range(1, 6);
            if (randomNumber == 3) 
            {
                //Instantiate(drop, transform.position, Quaternion.Euler(rotation));
            }
        }
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
