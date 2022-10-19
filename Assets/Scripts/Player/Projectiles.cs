using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectiles : MonoBehaviour
{
    float speed = 7.5f;
    [SerializeField] GameObject explodeProjectile;
    // Start is called before the first frame update

    // Update is called once per frame
    private void Update()
    {
        Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);

        screenPos.x = Mathf.Clamp01(screenPos.x);
        screenPos.y = Mathf.Clamp01(screenPos.y);
        if ( screenPos.x == 1)
            gameObject.SetActive(false);
        if (screenPos.y == 0 || screenPos.y == 1)
            gameObject.SetActive(false);
        transform.position = Camera.main.ViewportToWorldPoint(screenPos);

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    public void ProjectileExplode()
    {
        gameObject.SetActive(false);
        GameObject go = Instantiate(explodeProjectile, transform.position, Quaternion.identity);
        Destroy(go, 2);
    }
}
