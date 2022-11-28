using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongBallBounce : MonoBehaviour
{
    [SerializeField] private float speed = 2;
    [SerializeField] private float speedLimit = 20;
    [SerializeField] private LayerMask whatIsSolid;

    private Rigidbody rb;
    private LineRenderer lr;
    private Vector3 lastFrameVelocity;
    private Vector3 direction;
    private Vector3 previousPosition;

    void OnEnable()
    {
        previousPosition = transform.position;
        float x = Random.Range(1.0f, -1.0f);
        float y = Random.Range(1.0f, -1.0f);
        direction = transform.TransformDirection(new Vector3(x, y, 0));
        rb = GetComponent<Rigidbody>();
        lr = GetComponent<LineRenderer>();
        rb.AddForce(direction.normalized * speed, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        lastFrameVelocity = rb.velocity;
        PongBallLaser();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Bounce(collision.contacts[0].normal, collision.transform.name);
    }

    private void Bounce(Vector3 collisionNormal, string _name)
    {
        speed += 0.25f;
        Vector3 _direction = Vector3.Reflect(lastFrameVelocity.normalized, collisionNormal);
        float y = Random.Range(1.0f, -1.0f);

        if (_name == "PongLeft" || _name == "PongRight") _direction = new Vector3(_direction.x, y, _direction.z);

        rb.velocity = _direction.normalized * speed;
        if (speed > speedLimit) speed = speedLimit;

        CameraShake.instance.TriggerShake(0.06f, 0.1f, 0.05f);

        direction = _direction;
    }

    private void PongBallLaser()
    {
        Vector3 direction = transform.position - previousPosition;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, 100, whatIsSolid))
        {
            Vector3[] positions = new Vector3[2] { transform.position, hit.point};

            lr.positionCount = 2;
            lr.SetPositions(positions);

        }

        previousPosition = transform.position;
    }
}
