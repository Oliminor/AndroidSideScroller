using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextPopUpAnim : MonoBehaviour
{
    private Color defaultColor;
    float alpha = 1;
    private void Start()
    {
        defaultColor = GetComponent<TextMeshProUGUI>().color;
    }
    void Update()
    {
        transform.Translate(Vector3.up * Time.deltaTime * 40, Space.World);

        alpha = Mathf.Lerp(alpha, 0, Time.deltaTime);
        GetComponent<TextMeshProUGUI>().color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, alpha);
    }
}
