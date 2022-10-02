using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform PlayerMuzzle;
    
    public float speed;
    public float acceleration;

    float timerReset;

    public float timer=5.0f;

    float rotationSpeed = 30.0f;

    Rigidbody body;

    public JoystickController inputManager;

    private void Start()
    {
        body = GetComponent<Rigidbody>();
        body.useGravity=false;
        timerReset = timer;

    }

    private void FixedUpdate()
    {
        Vector2 moveDir = inputManager.GetInputAxis();


        if (inputManager.GetInputAxis().x > 0)
        {
            speed = Mathf.Lerp(speed, acceleration, Time.deltaTime);
        }
        if (inputManager.GetInputAxis().x < 0)
        {
            speed = Mathf.Lerp(acceleration, speed, Time.deltaTime);
        }

        body.AddForce(moveDir * speed);
        
        if(moveDir.y!=0)
            transform.Rotate(0, 0, moveDir.y*rotationSpeed * Time.deltaTime);
        
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            StartCoroutine(AutoFire());
            timer = timerReset;
        }
    }
    IEnumerator AutoFire()
    {
        GameObject bullet = BulletPool.singleton.GetPooled();
        bullet.transform.SetPositionAndRotation(PlayerMuzzle.position, PlayerMuzzle.rotation);
        //bullet.transform.SetParent(PlayerMuzzle);
        bullet.SetActive(true);
        yield return new WaitForSeconds(1);

    }
   
}
