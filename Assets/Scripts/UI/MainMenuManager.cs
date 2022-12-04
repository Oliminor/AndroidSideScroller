using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager instance;

    [SerializeField] RectTransform levelSelector;
    [SerializeField] RectTransform optionsMenu;
    [SerializeField] RectTransform levelSelectorScrollView;
    [SerializeField] RectTransform menuButtons;
    [SerializeField] Button optionsButton;
    [SerializeField] Button backButton;
    [SerializeField] RectTransform selectALevelText;
    [SerializeField] Canvas mainMenuCanvas;
    [SerializeField] TMPro.TMP_Dropdown dropDownMenu;
    [SerializeField] Volume postProcessVolume;
    [SerializeField] Toggle bloomToggleButton;
    [SerializeField] Toggle hDRToggleButton;
    [SerializeField] Slider renderScaleSlider;
    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider effectVolumeSlider;

    [SerializeField] List<TextMeshProUGUI> highScoreText = new();

    public List<TextMeshProUGUI> GetHighScoreText() { return highScoreText; }

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
        instance = this;
    }

    void Start()
    {
        SetupOptionsMenu();
        SetupStartGameOptionsMenu();

        Initialize();
    }

    void Update()
    {
        LerpOptions();
        LerpStartGame();
        MusicVolume();
    }

    private void Initialize()
    {
        dropDownMenu.SetValueWithoutNotify(PlayerSettings.instance.ResolutionIndex);
        bloomToggleButton.isOn = PlayerSettings.instance.IsBloomOn;
        hDRToggleButton.isOn = PlayerSettings.instance.IsHDROn;
        renderScaleSlider.value = PlayerSettings.instance.RenderScale;
        if (AudioManager.instance) masterVolumeSlider.value = AudioManager.instance.GetMasterVolume();
        if (AudioManager.instance) musicVolumeSlider.value = AudioManager.instance.GetMusicVolume();
        if (AudioManager.instance) effectVolumeSlider.value = AudioManager.instance.GetEffectVolume();

        if (PlayerSettings.instance.IsOptionIsOn)
        {
            optionsMenu.gameObject.SetActive(true);
            optionsBool = true;
            optionsLerpY = optionsStartY;
        }

        for (int i = 0; i < levelSelector.transform.childCount; i++)
        {
            highScoreText.Add(levelSelector.transform.GetChild(i).GetChild(2).GetComponent<TextMeshProUGUI>());
            PlayerSettings.instance.AddScore();
        }

    }

    private void MusicVolume()
    {
        AudioManager.instance.SetMasterVolume(masterVolumeSlider.value);
        AudioManager.instance.SetMusicVolume(musicVolumeSlider.value);
        AudioManager.instance.SetEffectVolume(effectVolumeSlider.value);
    }

    public void SelectLevel(int _index)
    {
        StartCoroutine(LoadLevel(_index));
    }

    IEnumerator LoadLevel(int _index)
    {
        AudioManager.instance.Play("SelectSound");

        Transition.instance.CloseTransition();

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(_index);
    }

    public void StartGame()
    {
        AudioManager.instance.Play("SelectSound");
        if (startGameBool)
        {
            startGameBool = false;
            startGameLerpX = startGameStartX;
            backButton.gameObject.SetActive(false);
            selectALevelText.gameObject.SetActive(false);

            selectorLerpX = Screen.width * 2.5f;
        }
        else
        {
            PlayerSettings.instance.RefreshHighScore();

            startGameBool = true;
            optionsButton.interactable = false;
            optionsButton.interactable = true;
            startGameLerpX = mainMenuCanvas.GetComponent<RectTransform>().rect.xMin - 500;
            backButton.gameObject.SetActive(true);
            selectALevelText.gameObject.SetActive(true);

            selectorLerpX = selectorStartX;

            if (optionsBool)
            {
                optionsBool = false;
                optionsButton.interactable = false;
                optionsButton.interactable = true;
                optionsLerpY = Screen.height * 2.5f;
            }
        }
    }
    public void Options()
    {
        AudioManager.instance.Play("SelectSound");
        if (!optionsBool) 
        {
            PlayerSettings.instance.IsOptionIsOn = true;
            optionsBool = true;
            optionsLerpY = optionsStartY;
        }
        else
        {
            PlayerSettings.instance.IsOptionIsOn = false;
            optionsBool = false;
            optionsButton.interactable = false;
            optionsButton.interactable = true;
            optionsLerpY = Screen.height * 2.5f;
        }
    }

    private void SetupOptionsMenu()
    {
        optionsMenu.gameObject.SetActive(true);
        optionsStartY = optionsMenu.position.y;
        Vector2 defaultPos = optionsMenu.position;
        optionsLerpY = Screen.height * 2.5f;
        optionsPosY = optionsLerpY;
        optionsMenu.position = new Vector2(defaultPos.x, optionsLerpY);
    }

    private void SetupStartGameOptionsMenu()
    {
        backButton.gameObject.SetActive(false);
        selectALevelText.gameObject.SetActive(false);
        levelSelectorScrollView.gameObject.SetActive(true);

        Vector2 defaultPos = menuButtons.position;
        startGameStartX = defaultPos.x;
        startGameLerpX = startGameStartX;
        startGamePosX = startGameStartX;
        menuButtons.position = defaultPos;

        Vector2 defaultPosSelector = levelSelectorScrollView.position;
        selectorStartX = defaultPosSelector.x;
        selectorLerpX = Screen.width * 2.5f;
        selectorPosX = selectorLerpX;
        levelSelectorScrollView.position = new Vector2(selectorPosX, defaultPosSelector.y);
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

        Vector2 defaultPosSelector = levelSelectorScrollView.position;
        selectorPosX = Mathf.Lerp(selectorPosX, selectorLerpX, 0.2f);
        levelSelectorScrollView.position = new Vector3(selectorPosX, defaultPosSelector.y);
    }

    public void SetResolution()
    {
        int index = dropDownMenu.value;
        int width = 1280;
        int height = 720;

        switch (index)
        {
            case 0:
                width = (int)PlayerSettings.instance.GetNativeResolution.x;
                height =(int)PlayerSettings.instance.GetNativeResolution.y;
                break;
            case 1:
                width = 3840;
                height = 2160;
                break;
            case 2:
                width = 3200;
                height = 1800;
                break;
            case 3:
                width = 2560;
                height = 1440;
                break;
            case 4:
                width = 1920;
                height = 1080;
                break;
            case 5:
                width = 1600;
                height = 900;
                break;
            case 6:
                width = 1366;
                height = 768;
                break;
            case 7:
                width = 1280;
                height = 720;
                break;
            case 8:
                width = 1024;
                height = 576;
                break;
            case 9:
                width = 960;
                height = 540;
                break;
            case 10:
                width = 848;
                height = 480;
                break;
            case 11:
                width = 640;
                height = 360;
                break;
        }
        Screen.SetResolution(width, height, true);

        PlayerSettings.instance.ResolutionIndex = index;
        SceneManager.LoadScene(0);
    }

    public void ToggleBloom()
    {
        PlayerSettings.instance.IsBloomOn = bloomToggleButton.isOn;

        Bloom _bloom;
        postProcessVolume.profile.TryGet(out _bloom);
        _bloom.active = bloomToggleButton.isOn;
    }

    public void ToggleHDR()
    {
        PlayerSettings.instance.IsHDROn = hDRToggleButton.isOn;

        UniversalRenderPipelineAsset _pipleine = (UniversalRenderPipelineAsset)QualitySettings.renderPipeline;
        _pipleine.supportsHDR = hDRToggleButton.isOn;
    }

    public void RenderScaleSLider()
    {
        UniversalRenderPipelineAsset _pipleine = (UniversalRenderPipelineAsset)QualitySettings.renderPipeline;
        _pipleine.renderScale = renderScaleSlider.value;
        PlayerSettings.instance.RenderScale = renderScaleSlider.value;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
