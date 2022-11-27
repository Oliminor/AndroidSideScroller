using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    // Start is called before the first frame update
    PlayerController player;
    float speed = 10f;
    [SerializeField]private enum PowerupType { SHIELD, FIRERATE, ANGLE, BARREL, LIFE, CONESHOOT, BULLETTIME, HOMINGPROJECTILE, BOMB, LASER }
    [SerializeField] PowerupType powerupType;
    [SerializeField] string powerUpName;
    [SerializeField] Color powerUpColor;
    
    void Start()
    {
        player = GameManager.instance.GetPlayer();
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
        if (other.gameObject == player.gameObject)
        {
            switch (powerupType)
            {
                case PowerupType.SHIELD:
                    player.ActivateShield();
                    break;
                case PowerupType.FIRERATE:
                    player.ShootFaster();
                    break;
                case PowerupType.ANGLE:
                    player.AddAngle();
                    break;
                case PowerupType.BARREL:
                    player.AddBarrel();
                    break;
                case PowerupType.LIFE:
                    player.AddLife();
                    break;
                case PowerupType.BULLETTIME:
                    StartCoroutine(player.BulletTime());
                    break;
                case PowerupType.CONESHOOT:
                    player.ConeShoot();
                    break;
                case PowerupType.HOMINGPROJECTILE:
                    player.ActivateHoming();
                    break;
                case PowerupType.BOMB:
                    break;
                case PowerupType.LASER:
                    player.ActivatePlayerLaser();
                    break;
            }
            PowerUpTextPopUp.instance.InstantiatePopUpText(powerUpName, powerUpColor, transform.position);
            Destroy(gameObject);
        }
    }
}
