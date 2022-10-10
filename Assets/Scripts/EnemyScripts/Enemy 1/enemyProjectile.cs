using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyProjectile : MonoBehaviour
{

    public float speed;


    // Update is called once per frame
    void Update()
    {

        //move forward
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        //destroy if out of the screen
        if (transform.position.x < -10)
        {
            Destroy(this.gameObject);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }
    }
}
