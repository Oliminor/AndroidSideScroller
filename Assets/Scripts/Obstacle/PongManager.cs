using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongManager : MonoBehaviour
{
    [SerializeField] private Transform pongBall;
    [SerializeField] private Transform pongLeft;
    [SerializeField] private Transform pongRight;

    [SerializeField] private Transform topCollider;
    [SerializeField] private Transform bottomCollider;
    [SerializeField] private Transform leftCollider;
    [SerializeField] private Transform rightCollider;

    float zPosition;
    void Start()
    {
        GenerateColldiersAroundWindow();
        Initalize();
    }

    private void Update()
    {
        PongMovement();
    }

    private void Initalize()
    {
        Vector3 spawnPos = new Vector3(0.06f, 0.5f, GameManager.instance.GetZPosition());
        Vector3 spwanPosWorldToPoint = Camera.main.ViewportToWorldPoint(spawnPos);

        pongLeft.position = spwanPosWorldToPoint;

        spawnPos = new Vector3(0.94f, 0.5f, GameManager.instance.GetZPosition());
        spwanPosWorldToPoint = Camera.main.ViewportToWorldPoint(spawnPos);

        pongRight.position = spwanPosWorldToPoint;

        spawnPos = new Vector3(0.5f, 0.5f, GameManager.instance.GetZPosition());
        spwanPosWorldToPoint = Camera.main.ViewportToWorldPoint(spawnPos);

        pongBall.position = spwanPosWorldToPoint;
    }

    private void PongMovement()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(pongBall.transform.position);

        if (pos.x < 0.5f && pongBall.transform.position.x > pongLeft.transform.position.x)
        {
            Vector3 leftPos = pongLeft.transform.position;
            pongLeft.transform.position = Vector3.Lerp(pongLeft.transform.position, new Vector3(leftPos.x, pongBall.transform.position.y, leftPos.z), 0.15f);
        }
        else if (pos.x >= 0.5f && pongBall.transform.position.x < pongRight.transform.position.x)
        {
            Vector3 rightPos = pongRight.transform.position;
            pongRight.transform.position = Vector3.Lerp(pongRight.transform.position, new Vector3(rightPos.x, pongBall.transform.position.y, rightPos.z), 0.15f);
        }
    }

    private void GenerateColldiersAroundWindow()
    {
        zPosition = -GameManager.instance.GetZPosition();

        Vector2 cameraPos = Camera.main.transform.position;
        Vector2 screenSize = Vector2.zero;
        screenSize.x = Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector3(0, 0, zPosition)), Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, zPosition))) * 0.5f;
        screenSize.y = Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector3(0, 0, zPosition)), Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, zPosition))) * 0.5f;

        rightCollider.localScale = new Vector3(transform.localScale.x, screenSize.y * 2, transform.localScale.z);
        rightCollider.position = new Vector3(cameraPos.x + screenSize.x + (rightCollider.localScale.x * 0.5f), cameraPos.y, zPosition);

        leftCollider.localScale = new Vector3(transform.localScale.x, screenSize.y * 2, transform.localScale.z);
        leftCollider.position = new Vector3(cameraPos.x - screenSize.x - (rightCollider.localScale.x * 0.5f), cameraPos.y, zPosition);
        
        topCollider.localScale = new Vector3(screenSize.x * 2, transform.localScale.y, transform.localScale.z);
        topCollider.position = new Vector3(cameraPos.x, cameraPos.y + screenSize.y + (topCollider.localScale.y * 0.5f), zPosition)
            ;
        bottomCollider.localScale = new Vector3(screenSize.x * 2, transform.localScale.y, transform.localScale.z);
        bottomCollider.position = new Vector3(cameraPos.x, cameraPos.y - screenSize.y - (bottomCollider.localScale.y * 0.5f), zPosition);
        

    }
}
