using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    [SerializeField] int health;
    [SerializeField] private GameObject enemyExplosion;
    [SerializeField] bool isPresistent;
    // Start is called before the first frame update

    protected void Respawn()
    {
        if (!isPresistent) return;

        // Respawn the enemy to the other side based on the CAMERA view
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        Vector3 respawnPos = Camera.main.ViewportToWorldPoint(new Vector3(1.1f, transform.position.y, GameManager.instance.GetZPosition()));

        if (pos.x < -0.1)
        {
            transform.position = respawnPos;
        }
    }

    virtual protected void TakeDamage()
    {
        health--;

        if (health <= 0)
        {
            Instantiate(enemyExplosion, transform.position, Quaternion.identity);
            powerUpDrop();
            Destroy(gameObject);
        }
    }

    private void powerUpDrop()
    {
        //Not yet finalized how is it going to be
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enemy destroyed by player collide");

        if (other.gameObject == GameManager.instance.GetPlayer())
        {
            Instantiate(enemyExplosion, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }

        if (other.TryGetComponent<Projectiles>(out Projectiles _projectile))
        {
            TakeDamage();
            _projectile.ProjectileExplode();
        }
    }
}
