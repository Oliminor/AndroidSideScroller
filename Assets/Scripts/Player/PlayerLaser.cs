using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaser : MonoBehaviour
{
    [SerializeField] private ParticleSystem laserParticle;
    [SerializeField] private LayerMask whatIsSolid;
    [SerializeField] private float damageRate;
    private LineRenderer lr;

    private float cooldown;
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        laserParticle.Play();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = GameManager.instance.GetPlayer().transform.position;
        DrawLaser();
    }

    private void DrawLaser()
    {
        cooldown -= Time.deltaTime;

        RaycastHit hit;
        if (Physics.SphereCast(transform.position, lr.startWidth / 2, Vector3.right, out hit, 100, whatIsSolid))
        {
            if (hit.transform.TryGetComponent(out IDamageable damageTarget))
            {
                if (cooldown < 0)
                {
                    damageTarget.TakeDamage();
                    cooldown = damageRate;
                }

            }
        }

        Vector3 startPos = transform.position;
        Vector3 endPos = new Vector3(hit.point.x, transform.position.y, hit.point.z);
        if (hit.point.x == 0) endPos = new Vector3(Vector3.right.x * 30, transform.position.y, -GameManager.instance.GetZPosition());

        Vector3[] positions = new Vector3[2] { startPos, endPos };

        laserParticle.transform.position = endPos;

        lr.positionCount = 2;
        lr.SetPositions(positions);
    }
}
