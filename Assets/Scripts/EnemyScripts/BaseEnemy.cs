using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    [SerializeField] protected int health;
    [SerializeField] int score;
    [SerializeField] private GameObject enemyExplosion;
    [SerializeField] bool isPresistent;
    // Start is called before the first frame update

    public int GetEnemyScore() { return score; }

    private void Update()
    {
        Respawn();
    }
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
            GameManager.instance.AddScoreFromEnemy(score);
            GameManager.instance.SetEnemyKilled();
            Destroy(gameObject);
        }
    }

    private void powerUpDrop()
    {
        int randomChance = Random.Range(0, 2);
        if(randomChance==1)
        {
            GameObject targetPower = PowerUpManager.instance.availiblePowerups[0];
            PowerUpManager.instance.availiblePowerups.Remove(targetPower);
            GameObject go = Instantiate(targetPower, transform.position, Quaternion.identity);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enemy destroyed by player collide");

        if (other.gameObject == GameManager.instance.GetPlayer().gameObject)
        {
            Instantiate(enemyExplosion, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
        if (other.TryGetComponent(out Projectiles projectiles))
        {
            if (health > 0)
            {
                TakeDamage();
                other.gameObject.SetActive(false);
                projectiles.InstantiateDestroyEffect();
            }
            
        }

    }
}
