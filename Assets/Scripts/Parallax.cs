using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] float cameraSpeed;
    [SerializeField] List<customClass> layers;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Camera.main.transform.Translate(cameraSpeed * new Vector3(1 * cameraSpeed * Time.deltaTime, 0, 0));
    }
}

[System.Serializable]
public class customClass
{
    public GameObject layer;
    public float speedPercent;
}