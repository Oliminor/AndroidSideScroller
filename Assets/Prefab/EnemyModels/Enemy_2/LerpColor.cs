using UnityEngine;

public class LerpColor : MonoBehaviour
{
    MeshRenderer headMeshRenderer;
    [SerializeField] [Range(0f, 1f)] float lerpTime;

    [SerializeField] Color[] myColors;

    int index = 0;

    float t = 0.8f;

    int len;

    // Start is called before the first frame update
    void Start()
    {
        headMeshRenderer = GetComponent<MeshRenderer>();
        len = myColors.Length;
    }

    // Update is called once per frame
    void Update()
    {
        headMeshRenderer.material.color = Color.Lerp(headMeshRenderer.material.color, myColors[index], lerpTime * Time.deltaTime);

        t = Mathf.Lerp(t, 1f, lerpTime * Time.deltaTime);

        if (t > .9f) 
        {
            t = 0.85f;
            index++;
            index = (index >= len) ? 0 : index;

        }
    }
}
