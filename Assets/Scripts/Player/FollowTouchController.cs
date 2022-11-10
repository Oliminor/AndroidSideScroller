using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FollowTouchController : MonoBehaviour
{
    public static FollowTouchController instance;

    Vector3 previousPosition;
    Vector3 currentPosition;

    bool isFirstTouch = false;

    public Vector3 ArrowPosition { get { return transform.position; } private set { } }
    private void Awake()
    {
        instance = this;
        previousPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.touchCount < 1)
        {
            isFirstTouch = false;
            GetComponent<Image>().enabled = false;
            return;
        }

        Touch touch = Input.GetTouch(0);

        currentPosition = touch.position;

        if (!isFirstTouch)
        {
            isFirstTouch = true;
            previousPosition = currentPosition;
        }

        Vector3 direction = currentPosition - previousPosition;
        float distance = Vector3.Distance(previousPosition, currentPosition);

        GetComponent<Image>().enabled = true;

        transform.position += direction.normalized * distance;

        previousPosition = currentPosition;
    }

}