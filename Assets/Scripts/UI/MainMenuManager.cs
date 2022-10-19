using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] RectTransform optionsMenu;
    [SerializeField] RectTransform levelSelector;
    [SerializeField] RectTransform menuButtons;
    [SerializeField] Button optionsButton;
    [SerializeField] Button backButton;
    [SerializeField] Canvas mainMenuCanvas;

    float optionsStartY;
    float optionsLerpY;
    float optionsPosY;

    float startGameStartX;
    float startGameLerpX;
    float startGamePosX;

    float selectorStartX;
    float selectorLerpX;
    float selectorPosX;

    bool optionsBool = false;
    bool startGameBool = false;
    private void Awake()
    {
        mainMenuCanvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(Screen.width, Screen.height);
    }
    void Start()
    {
        SetupOptionsMenu();
        SetupStartGameOptionsMenu();
    }

    void Update()
    {
        LerpOptions();
        LerpStartGame();
    }

    public void SelectLevel(int _index)
    {
        StartCoroutine(LoadLevel(_index));
    }

    IEnumerator LoadLevel(int _index)
    {
        Transition.instance.CloseTransition();

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(_index);
    }

    public void StartGame()
    {
        if (startGameBool)
        {
            startGameBool = false;
            startGameLerpX = startGameStartX;
            backButton.gameObject.SetActive(false);

            selectorLerpX = Screen.width + 1500;
        }
        else
        {
            startGameBool = true;
            optionsButton.interactable = false;
            optionsButton.interactable = true;
            startGameLerpX = mainMenuCanvas.GetComponent<RectTransform>().rect.xMin - 500;
            backButton.gameObject.SetActive(true);

            selectorLerpX = selectorStartX;

            if (optionsBool)
            {
                optionsBool = false;
                optionsButton.interactable = false;
                optionsButton.interactable = true;
                optionsLerpY = Screen.height + 1000;
            }
        }
    }
    public void Options()
    {
        if (!optionsBool) 
        {
            optionsBool = true;
            optionsLerpY = optionsStartY;
        }
        else
        {
            optionsBool = false;
            optionsButton.interactable = false;
            optionsButton.interactable = true;
            optionsLerpY = Screen.height + 1000;
        }
    }

    private void SetupOptionsMenu()
    {
        optionsMenu.gameObject.SetActive(true);
        optionsStartY = optionsMenu.position.y;
        Vector2 defaultPos = optionsMenu.position;
        optionsLerpY = Screen.height + 1000;
        optionsPosY = optionsLerpY;
        optionsMenu.position = new Vector2(defaultPos.x, optionsLerpY);
    }

    private void SetupStartGameOptionsMenu()
    {
        backButton.gameObject.SetActive(false);
        levelSelector.gameObject.SetActive(true);

        Vector2 defaultPos = menuButtons.position;
        startGameStartX = defaultPos.x;
        startGameLerpX = startGameStartX;
        startGamePosX = startGameStartX;
        menuButtons.position = defaultPos;

        Vector2 defaultPosSelector = levelSelector.position;
        selectorStartX = defaultPosSelector.x;
        selectorLerpX = Screen.width + 1500;
        selectorPosX = selectorLerpX;
        levelSelector.position = new Vector2(selectorPosX, defaultPosSelector.y);
    }

    private void LerpOptions()
    {
        Vector2 defaultPos = optionsMenu.position;
        optionsPosY = Mathf.Lerp(optionsPosY, optionsLerpY, 0.2f);
        optionsMenu.position = new Vector3(defaultPos.x, optionsPosY);
    }

    private void LerpStartGame()
    {
        Vector2 defaultPos = menuButtons.position;
        startGamePosX = Mathf.Lerp(startGamePosX, startGameLerpX, 0.2f);
        menuButtons.position = new Vector3(startGamePosX, defaultPos.y);

        Vector2 defaultPosSelector = levelSelector.position;
        selectorPosX = Mathf.Lerp(selectorPosX, selectorLerpX, 0.2f);
        levelSelector.position = new Vector3(selectorPosX, defaultPosSelector.y);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
