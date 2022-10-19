using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject player;
    PlayerController playerScript;
    float speed = 10f;
    [SerializeField]private enum PowerupType { SHIELD, FIRERATE, ANGLE, BARREL, LIFE, BULLETTIME }
    [SerializeField] PowerupType powerupType;

    void Start()
    {
        player = GameManager.instance.GetPlayer();
        playerScript=player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);

        if (screenPos.x == 1||screenPos.x==0.5)
            Destroy(gameObject);
        if (screenPos.y == 0 || screenPos.y == 1)
            Destroy(gameObject);
        transform.position = Camera.main.ViewportToWorldPoint(screenPos);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            switch (powerupType)
            {
                case PowerupType.SHIELD:
                    StartCoroutine(playerScript.Invulnerability());
                    break;
                case PowerupType.FIRERATE:
                    playerScript.ShootFaster();
                    break;
                case PowerupType.ANGLE:
                    playerScript.AddAngle();
                    break;
                case PowerupType.BARREL:
                    playerScript.AddBarrel();
                    break;
                case PowerupType.LIFE:
                    playerScript.AddLife();
                    break;
                case PowerupType.BULLETTIME:
                    StartCoroutine(playerScript.BulletTime());
                    break;
            }
            Destroy(gameObject);
        }
    }
}
