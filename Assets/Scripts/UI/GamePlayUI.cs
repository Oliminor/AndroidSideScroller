using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GamePlayUI : MonoBehaviour
{
    public static GamePlayUI instance;

    [SerializeField] private TextMeshProUGUI scoreText;
    //[SerializeField] private TextMeshProUGUI scoreMultiplierText;
    //[SerializeField] private List<Image> multiplierGraphicList;
    [SerializeField] private RectTransform playerHealth;
    [SerializeField] private RectTransform gameOverScene;
    [SerializeField] private RectTransform pauseMenuUI;
    [SerializeField] private RectTransform endLevelScreen;
    [SerializeField] private RectTransform gameHUB;
    [SerializeField] private Image playerLivesImage;
    [SerializeField] private Animator gameOverAnimator;
    [SerializeField] private Animator endLevelAnimator;


    [SerializeField] private TextMeshProUGUI starsText;
    [SerializeField] private TextMeshProUGUI totalScoreText;
    [SerializeField] private TextMeshProUGUI timeScoreText;
    [SerializeField] private TextMeshProUGUI enemyKilledText;
    [SerializeField] private TextMeshProUGUI livesMultiplierText;
    [SerializeField] private TextMeshProUGUI multiplier1Text;
    [SerializeField] private TextMeshProUGUI multiplier15Text;
    [SerializeField] private TextMeshProUGUI multiplier2Text;

    [SerializeField] private RectTransform stars;

    [SerializeField] private float alphaMax;
    [SerializeField] private float alphaLerp;
    [SerializeField] private float lerpMaxTextSize;
    [SerializeField] private float lerpTime;

    private float scoreTextSize;
    private List<Image> playerHealthList = new();
    // Start is called before the first frame update

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        OInitialized();
    }

    // Update is called once per frame
    void Update()
    {
        ScoreText();
        LerpScoreSize();
        LerpScoreMultiplierSize();
        //ScoreMultiplierText();
        //MultiplierGraphicLerp();
    }

    private void OInitialized()
    {
        scoreTextSize = scoreText.fontSize;
        /*
        for (int i = 0; i < multiplierGraphicList.Count; i++)
        {
            Color _color = multiplierGraphicList[i].color;
            multiplierGraphicList[i].color = new Color(_color.r, _color.g, _color.b, 0);
        }
        */
        for (int i = 0; i < GameManager.instance.GetPlayer().GetLives(); i++)
        {
            Image go =  Instantiate(playerLivesImage, playerHealth);
            playerHealthList.Add(go);
        }

        gameHUB.gameObject.SetActive(true);
        pauseMenuUI.gameObject.SetActive(false);
        gameOverScene.gameObject.SetActive(true);
        endLevelScreen.gameObject.SetActive(true);
        if (Transition.instance) Transition.instance.OpenTransition();
    }

    public void EndLevelSceneTrigger()
    {
        StartCoroutine(EndLevelEnumerator());
    }

    IEnumerator EndLevelEnumerator()
    {
        if (Transition.instance) Transition.instance.CloseTransition();

        yield return new WaitForSeconds(1);

        endLevelAnimator.SetTrigger("endLevel");
        gameHUB.gameObject.SetActive(false);
        EndLevelText();
    }

    public void EndLevelText()
    {
        GameManager gM = GameManager.instance;

        float percent = (float)gM.GetTotalScore() / gM.GetMaximumScore() * 100.0f;

        int star = 1;

        if (percent > 60) star = 2;
        if (percent > 80) star = 3;

        for (int i = 0; i < star; i++)
        {
            stars.GetChild(i).GetChild(0).gameObject.SetActive(true);
        }

        starsText.text = star.ToString();
        totalScoreText.text = gM.GetTotalScore().ToString();
        timeScoreText.text = gM.GetTimeScore().ToString();
        enemyKilledText.text = gM.GetEnemyKilledNumber().ToString() + "/" + gM.GetEnemyNumber().ToString();
        livesMultiplierText.text = gM.GetPlayer().GetLives().ToString();
        multiplier1Text.text = gM.GetMultiplierTick()[0].ToString();
        multiplier15Text.text = gM.GetMultiplierTick()[1].ToString();
        multiplier2Text.text = gM.GetMultiplierTick()[2].ToString();

        int index = SceneManager.GetActiveScene().buildIndex - 1;

        if (PlayerSettings.instance.GetHighScore()[index] < gM.GetTotalScore())
        {
            PlayerSettings.instance.SetHightScore(index, gM.GetTotalScore());
        }
    }

    public void RestartLevel()
    {
        AudioManager.instance.Play("SelectSound");
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RestartFromPause()
    {
        AudioManager.instance.Play("SelectSound");
        Time.timeScale = 1;
        StartCoroutine(RestartLevelFromPause());
    }
    IEnumerator RestartLevelFromPause()
    {
        Time.timeScale = 1;
        if (Transition.instance) Transition.instance.CloseTransition();

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMainMenu()
    {
        AudioManager.instance.Play("SelectSound");
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
        if (Transition.instance) Transition.instance.OpenTransition();
        gameOverAnimator.SetTrigger("idle");
    }

    public void BackToMainFromPause()
    {
        AudioManager.instance.Play("SelectSound");
        StartCoroutine(BackToMainMenuFromPause());
    }

    IEnumerator BackToMainMenuFromPause()
    {
        Time.timeScale = 1;
        if (Transition.instance) Transition.instance.CloseTransition();

        yield return new WaitForSeconds(1);

        if (Transition.instance) Transition.instance.OpenTransition();
        SceneManager.LoadScene(0);
    }

    public void NextLevel()
    {
        AudioManager.instance.Play("SelectSound");
        int nextLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextLevelIndex >= SceneManager.sceneCountInBuildSettings)
        {
            StartCoroutine(BackToMainMenuFromPause());
            return;
        }

        SceneManager.LoadScene(nextLevelIndex);
    }

    public void PauseResumeGame()
    {
        AudioManager.instance.Play("SelectSound");
        if (!GameManager.instance.GetIsPaused())
        {
            Time.timeScale = 0;
            GameManager.instance.SetIsPaused(true);
            pauseMenuUI.gameObject.SetActive(true);
            gameHUB.gameObject.SetActive(false);
        }
        else
        {
            Time.timeScale = 1;
            GameManager.instance.SetIsPaused(false);
            pauseMenuUI.gameObject.SetActive(false);
            gameHUB.gameObject.SetActive(true);
        }
    }

    public void HideGameOver()
    {
        gameOverAnimator.SetTrigger("idle");
    }

    public void TriggerGameOver()
    {
        StartCoroutine(GameOver());
        GameManager.instance.GameOver();
    }

    IEnumerator GameOver()
    {
        if (Transition.instance) Transition.instance.CloseTransition();
        gameHUB.gameObject.SetActive(false);

        yield return new WaitForSeconds(1);

        gameOverAnimator.SetTrigger("gameOver");
    }

    public void AddHealth()
    {
        Image go = Instantiate(playerLivesImage, playerHealth);
        playerHealthList.Add(go);
    }

    public void RemoveHealth()
    {
        Destroy(playerHealthList[0].gameObject);
        playerHealthList.RemoveAt(0);
    }

    private void ScoreText()
    {
        int score = GameManager.instance.GetScore();
        scoreText.text = score.ToString("00000");
    }

    private void ScoreMultiplierText()
    {
        //float multiplier = GameManager.instance.GetScoreMultiplier();
        //scoreMultiplierText.text = "x" + multiplier;
    }

    private void LerpScoreSize()
    {
        scoreText.fontSize = Mathf.Lerp(scoreText.fontSize, scoreTextSize, lerpTime);
    }

    private void LerpScoreMultiplierSize()
    {
        //scoreMultiplierText.fontSize = Mathf.Lerp(scoreMultiplierText.fontSize, scoreTextSize, lerpTime);
    }

    public void SetScoreTextSize()
    {
        scoreText.fontSize = lerpMaxTextSize;
    }
    /*
    public void SetScoreMultiplierTextSize(int index)
    {
        scoreMultiplierText.fontSize = lerpMaxTextSize;

        Color _color = multiplierGraphicList[index].color;
        multiplierGraphicList[index].color = new Color(_color.r, _color.g, _color.b, alphaMax);
    }

    private void MultiplierGraphicLerp()
    {
        for (int i = 0; i < multiplierGraphicList.Count; i++)
        {
            if (multiplierGraphicList[i].color.a > 0)
            {
                Color _color = multiplierGraphicList[i].color;
                float alpha = multiplierGraphicList[i].color.a - alphaLerp;
                multiplierGraphicList[i].color = new Color(_color.r, _color.g, _color.b, alpha);
            }
        }
    }
    */
}
