using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaFade : MonoBehaviour
{
    private MeshRenderer mesh;
    private Material mat;

    float alpha = 1;
    void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
        mat = mesh.material;
    }

    // Update is called once per frame
    void Update()
    {
        alpha = Mathf.Lerp(alpha, 0, Time.deltaTime * 15);

        mat.SetFloat("_Alpha", alpha);

        if (alpha <= 0.1f) Destroy(gameObject);
    }
}
