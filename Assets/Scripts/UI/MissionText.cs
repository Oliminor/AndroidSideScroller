using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MissionText : MonoBehaviour
{
    private TextMeshProUGUI textMeshProGUI;
    private string missionText;
    void Start()
    {
        textMeshProGUI = GetComponent<TextMeshProUGUI>();
        missionText = textMeshProGUI.text;
        textMeshProGUI.text = "";

        StartCoroutine(TextPrint());
        StartCoroutine(RemoveText());
    }

    IEnumerator TextPrint()
    {
        int counter = 0;
        bool isDialogueFinished = false;

        while (!isDialogueFinished)
        {
            yield return new WaitForSeconds(0.05f);
            textMeshProGUI.text += missionText[counter];
            counter++;

            if (counter == missionText.Length) isDialogueFinished = true;
        }
    }
    IEnumerator RemoveText()
    {
        yield return new WaitForSeconds(EnemySpawner.instance.GetDelayStartTime());
        textMeshProGUI.text = "";
    }

}
