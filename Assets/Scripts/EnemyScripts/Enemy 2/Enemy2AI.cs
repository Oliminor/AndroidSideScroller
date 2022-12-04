using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2AI : BaseEnemy
{
    //Movement speed
    [SerializeField] private float horizontalSpeed;
    [SerializeField] Transform thruster;
    [SerializeField] float stayTime;

    private bool isChargeing = false;
    private bool startFollow = false;
    private float lerpY;

    // Update is called once per frame
    void Update()
    {
        //move forward
        if (!startFollow) transform.Translate(Vector3.left * horizontalSpeed * Time.deltaTime);

        if (startFollow && !isChargeing)
        {
            lerpY = Mathf.Lerp(transform.position.y, GameManager.instance.GetPlayer().transform.position.y - 0.3f, 0.02f);

            transform.position = new Vector3(transform.position.x, lerpY, transform.position.z);
        }

        if (isChargeing) transform.Translate(Vector3.left * horizontalSpeed * 5 * Time.deltaTime);

        StopAtSide();
    }

    IEnumerator Delay()
    {
        while (true)
        {
            yield return new WaitForSeconds(stayTime);
            isChargeing = true;
            health = 1;
        }
    }

    private void StopAtSide()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

        if (pos.x < 0.9 && !startFollow)
        {
            startFollow = true;
            StartCoroutine(Delay());
        }
    }
}


