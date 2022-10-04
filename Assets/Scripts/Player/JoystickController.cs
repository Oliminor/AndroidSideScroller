using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public float maxPivot = 1; //max movement from centrepoint
    public float deadzone = 0.1f; //min movement to trigger input 

    public Canvas canvas;

    RectTransform rectTransform; //image rectangle
    Vector2 initialPosition; //centrepoint of joystick
    Vector2 tapPosition; //where the player interacts

    bool isHolding = false;

    Vector2 inputPos = Vector2.zero; //init input axis as (0,0), no input


    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialPosition = rectTransform.anchoredPosition3D; //initial position is current position in realtion to anchor
    }

    void Update()
    {
      
    }

    //INPUT EVENT FUNCTIONS
    public void OnPointerDown(PointerEventData eventData) //when the player touches the button
    {
        tapPosition = eventData.pressPosition;
        isHolding = true;
      
    }

    public void OnPointerUp(PointerEventData eventData) //when the player releases
    {

        isHolding = false;
        //joystick reset
        rectTransform.anchoredPosition = initialPosition;
        inputPos = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData) //when the player drags 
    {
        Vector2 movementV = Vector2.ClampMagnitude((eventData.position - tapPosition) / canvas.scaleFactor,
                                                 (rectTransform.sizeDelta.x * maxPivot) + (rectTransform.sizeDelta.x * deadzone));

        Vector2 movePosition = initialPosition + movementV;
        rectTransform.anchoredPosition = movePosition;

        //input axis
        float axisX = 0, axisY = 0;

        if (Mathf.Abs(movementV.x) > rectTransform.sizeDelta.x * deadzone)
        {
            axisX = (movementV.x - (rectTransform.sizeDelta.x * deadzone * (movementV.x > 0 ? 1 : -1))) / (rectTransform.sizeDelta.x * maxPivot);
        }
        if (Mathf.Abs(movementV.y) > rectTransform.sizeDelta.x * deadzone)
        {
            axisY = (movementV.y - (rectTransform.sizeDelta.y * deadzone * (movementV.y > 0 ? 1 : -1))) / (rectTransform.sizeDelta.x * maxPivot);
        }
        inputPos = new Vector2(axisX, axisY);


    }

    public Vector2 GetInputAxis()
    {
        return inputPos;
    }
    public bool ReturnHoldStatus()
    {
        return isHolding;
    }
}
