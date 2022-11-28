using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserObstacle : MonoBehaviour
{
    private LineRenderer lr;
    [SerializeField] private float switchTime;
    [SerializeField] private LayerMask whatIsSolid;
    [SerializeField] private Transform laserEffect;

    bool laserIsOn = true;

    private void Start()
    {
        lr = GetComponent<LineRenderer>();
        StartCoroutine(LaserSwitchOnOff());
    }

    void Update()
    {
        PongBallLaser();
    }
    private void PongBallLaser()
    {
        if (!laserIsOn)
        {
            Vector3[] positions = new Vector3[2] { transform.position, transform.position };
            lr.SetPositions(positions);
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 20, whatIsSolid))
        {
            Vector3[] positions = new Vector3[2] { transform.position, hit.point };

            laserEffect.transform.position = hit.point;

            lr.positionCount = 2;
            lr.SetPositions(positions);

            GameManager.instance.GetPlayer().GetComponent<PlayerController>().TakeDamage();
        }
        else
        {
            Vector3[] positions = new Vector3[2] { transform.position, transform.position + Vector3.down * 20 };
            laserEffect.transform.position = positions[1];
            lr.SetPositions(positions);
        }
    }

    IEnumerator LaserSwitchOnOff()
    {
        while (true)
        {
            yield return new WaitForSeconds(switchTime);
            laserIsOn = !laserIsOn;
        }
    }

}
