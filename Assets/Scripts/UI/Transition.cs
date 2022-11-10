using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour
{
    public static Transition instance;
    Animator anim;
    void Awake()
    {
        anim = GetComponent<Animator>();

        DontDestroyOnLoad(transform.parent);

        if (instance == null) instance = this;
        else Destroy(transform.parent.gameObject);
    }

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void OpenTransition()
    {
        anim.SetTrigger("start");
    }

    public void CloseTransition()
    {
        anim.SetTrigger("close");
    }
}
