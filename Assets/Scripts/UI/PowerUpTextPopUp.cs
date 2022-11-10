using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PowerUpTextPopUp : MonoBehaviour
{
    public static PowerUpTextPopUp instance;

    [SerializeField] Transform textPopUp;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    public void InstantiatePopUpText(string _powerUpName, Color _textColor, Vector3 _position)
    {
        Vector3 spawnPos = Camera.main.WorldToScreenPoint(_position);

        GameObject go = Instantiate(textPopUp.gameObject, spawnPos + Vector3.up * 50, Quaternion.identity, transform);
        go.GetComponent<TextMeshProUGUI>().text = _powerUpName;
        go.GetComponent<TextMeshProUGUI>().color = _textColor;
        go.AddComponent<TextPopUpAnim>();
        Destroy(go, 3);
    }
}
