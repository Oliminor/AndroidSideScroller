using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour, IDamageable
{
    [SerializeField] protected int health;
    [SerializeField] int score;
    [SerializeField] private GameObject enemyExplosion;
    [SerializeField] bool isPresistent;
    [SerializeField] List<MeshRenderer> damageMeshRendererList;
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

    public void WipeAllEnemyOnTheScreen()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        if (pos.x < 1.0 && pos.x > 0) EnemyDeath();
    }

    virtual protected void TakeDamage()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

        if (pos.x > 1.0) return;

        health--;
        StartCoroutine(DamageFlash());

        if (health <= 0)
        {
            EnemyDeath();
        }
    }

    IEnumerator DamageFlash()
    {
        for (int i = 0; i < damageMeshRendererList.Count; i++)
        {
            for (int j = 0; j < damageMeshRendererList[i].materials.Length; j++)
            {
                damageMeshRendererList[i].materials[j].SetFloat("_Lerp", 1);
            }
        }

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < damageMeshRendererList.Count; i++)
        {
            for (int j = 0; j < damageMeshRendererList[i].materials.Length; j++)
            {
                damageMeshRendererList[i].materials[j].SetFloat("_Lerp", 0);
            }
        }
    }

    private void EnemyDeath()
    {
        GameObject go = Instantiate(enemyExplosion, transform.position, Quaternion.identity);
        GameManager.instance.AddScoreFromEnemy(score);
        GameManager.instance.SetEnemyKilled();
        Destroy(go, 3);
        Destroy(gameObject);
        float _score = score; //* GameManager.instance.GetScoreMultiplier();
        int _popUpScore = (int)_score;
        CameraShake.instance.TriggerShake(0.1f, 0.1f, 0.05f);
        PowerUpTextPopUp.instance.InstantiatePopUpText("+" + _popUpScore.ToString(), Color.white, transform.position);
        powerUpDrop();
    }

    private void powerUpDrop()
    {
        if (PowerUpManager.instance.availiblePowerups.Count == 0) return;

        int randomChance = Random.Range(0, 2);
        if(randomChance==1)
        {
            GameObject targetPower = PowerUpManager.instance.availiblePowerups[0];
            PowerUpManager.instance.availiblePowerups.Remove(targetPower);
            Instantiate(targetPower, transform.position, Quaternion.identity);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameManager.instance.GetPlayer().gameObject)
        {
            GameObject go = Instantiate(enemyExplosion, transform.position, Quaternion.identity);
            Destroy(go, 2);
            Destroy(this.gameObject);
        }
    }

    void IDamageable.TakeDamage()
    {
        if (health > 0) TakeDamage();
    }
}

public interface IDamageable
{ 
    public void TakeDamage();
}
