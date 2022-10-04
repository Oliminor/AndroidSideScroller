using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2AI : MonoBehaviour
{
    //Movement speed
    public float horizontalSpeed;

    public int health;

    //Other Game Objects
    [SerializeField]
    private GameObject enemyExplosion;
    [SerializeField]
    private GameObject drop;

    // Update is called once per frame
    void Update()
    {
        //move forward
        transform.Translate(Vector3.left * horizontalSpeed * Time.deltaTime);

        //respawn if out of the screen
        if (transform.position.x < -11)
        {
            //Assuming camera position is (0, 0, 0)
            float randomY = Random.Range(-5, 4);
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
            health--;
            Debug.Log("current health: " + health);

            if (health <= 0) 
            {
                Instantiate(enemyExplosion, transform.position, Quaternion.identity);
                Destroy(this.gameObject);


                //random drop
                int randomNumber = Random.Range(1, 6);
                if (randomNumber == 3)
                {
                    Instantiate(drop, transform.position, Quaternion.identity);
                }

            }
        
        }
    }
}


