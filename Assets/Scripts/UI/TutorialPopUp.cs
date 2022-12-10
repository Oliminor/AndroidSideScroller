using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPopUp : MonoBehaviour
{
    [SerializeField] private List<Tutorials> tutorialList;
    [SerializeField] private RectTransform blackBG;
    [SerializeField] private RectTransform continueText;

    bool isFirstTouch;
    bool tutorialReadyToExit = false;

    private int tutorialIndex;
    void Start()
    {
        blackBG.gameObject.SetActive(false);
        continueText.gameObject.SetActive(false);

        for (int i = 0; i < tutorialList.Count; i++)
        {
            StartCoroutine(ShowTutorial(tutorialList[i].tutorialSpawnTime, tutorialList[i].tutorialPicture));
        } 
    }

    void Update()
    {
        if (!tutorialReadyToExit) return;

#if !UNITY_EDITOR_WIN
           if (Input.touchCount < 1)
        {
            isFirstTouch = false;
            return;
        }

        if (!isFirstTouch)
        {
            isFirstTouch = true;
            DeactivateTutorial();
        }
#endif

#if UNITY_EDITOR_WIN
        if (Input.GetMouseButtonDown(0)) DeactivateTutorial();
#endif
    }

    private void DeactivateTutorial()
    {
        if (Time.timeScale != 0) return;

        Time.timeScale = 1;
        blackBG.gameObject.SetActive(false);
        continueText.gameObject.SetActive(false);
        tutorialList[tutorialIndex].tutorialPicture.gameObject.SetActive(false);
        tutorialIndex++;
    }

    IEnumerator ShowTutorial(float time, GameObject image)
    {
        image.SetActive(false);
        yield return new WaitForSeconds(time);
        tutorialReadyToExit = false;
        image.SetActive(true);
        blackBG.gameObject.SetActive(true);
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(1);
        continueText.gameObject.SetActive(true);
        tutorialReadyToExit = true;
    }
}

[System.Serializable]
public class Tutorials
{
    public GameObject tutorialPicture;
    public float tutorialSpawnTime;

    public Tutorials(GameObject _tutorialPicture, float _tutorialSpawnTime)
    {
        tutorialPicture = _tutorialPicture;
        tutorialSpawnTime = _tutorialSpawnTime;
    }
}

