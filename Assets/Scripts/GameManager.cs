using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private float speed;
    [SerializeField] private float gameScore;
    [SerializeField] private List<float> multiplierValue;
    [SerializeField] private int[] multiplierTick;
    [SerializeField] private int scorePerSecond;
    [SerializeField] private float globalZposition;
    [SerializeField] private bool isTheLevelEnded = false;
    [SerializeField] private bool isGameOver = false;
    private bool isPaused = false;

    private PlayerController player;

    private float lerpScore;
    private float lerpTime;
    //private float scoreMultiplier = 1;
    //private float tempScoreMultiplier;
    private int multiplierIndex;
    private int gameTime;

    private float timeScore;
    private int enemyKilled;
    private int allEnemyNumber;
    private int enemyScore;
    // Start is called before the first frame update

    public List<float> GetMultiplierValue() { return multiplierValue; }
    public int[] GetMultiplierTick() { return multiplierTick; }
    public int GetGameTIme() { return gameTime; }
    public void SetSpeed(float _speed) { speed = _speed; } 
    public float GetSpeed() { return speed; }
    public void SetPlayer(PlayerController _player) { player = _player; }
    public PlayerController GetPlayer() { return player; }
    public void SetScore(int _score) { gameScore = _score; }
    public int GetScore() { return (int)gameScore; }
    public float GetZPosition() { return globalZposition; }
    //public void SetScoreMultiplier(float _scoreMultiplier) { scoreMultiplier = _scoreMultiplier; }
    //public float GetScoreMultiplier() { return scoreMultiplier; }
    public int GetMultiplierIndex() { return multiplierIndex; }
    public bool GetIsPaused() { return isPaused; }
    public void SetIsPaused(bool _isPaused) { isPaused = _isPaused; }
    public bool GetIsLevelEnded() { return isTheLevelEnded; }
    public bool GetIsGameOver() { return isGameOver; }
    public float GetTimeScore() { return (int)timeScore; }
    public int GetEnemyNumber() { return allEnemyNumber; }
    public void SetEnemyNumber() { allEnemyNumber++; }
    public void SetEnemyScore(int _score) { enemyScore += _score; }
    public int GetEnemyKilledNumber() { return enemyKilled; }
    public void SetEnemyKilled() { enemyKilled++; }
          

    void Awake()
    {
        multiplierTick = new int[multiplierValue.Count];
        //tempScoreMultiplier = scoreMultiplier;
        instance = this;
    }

    private void Update()
    {
        LerpScore();
        //ScoreMultiplierLayer();
    }

    public int GetTotalScore() 
    {
        float totalScore = gameScore * GetPlayer().GetLives();
        return (int)totalScore; 
    }

    public int GetMaximumScore()
    {
        int gameTime = EnemySpawner.instance.GetGameTime();

        int playerMaxLife = 3;

        float maximumScore = (gameTime * scorePerSecond + enemyScore) * playerMaxLife; //* multiplierValue[multiplierValue.Count - 1]

        return (int)maximumScore;
    }

    private void ScoreMultiplierLayer()
    {
        if (isTheLevelEnded) return;

        Vector3 playerPos = Camera.main.WorldToViewportPoint(player.transform.position);

        if (playerPos.x < 1.0f) multiplierIndex = 2;
        if (playerPos.x < 0.666f) multiplierIndex = 1;
        if (playerPos.x < 0.333f) multiplierIndex = 0;

        //scoreMultiplier = multiplierValue[multiplierIndex];


       // if (tempScoreMultiplier != multiplierIndex) GamePlayUI.instance.SetScoreMultiplierTextSize(multiplierIndex);

        //tempScoreMultiplier = multiplierIndex;

    }

    public IEnumerator ScoreAdditionPerSecond()
    {
        while (!isGameOver && !isTheLevelEnded)
        {
            lerpScore += scorePerSecond; //* scoreMultiplier;
            timeScore += scorePerSecond; //* scoreMultiplier;
            multiplierTick[multiplierIndex]++;
            gameTime++;
            ResetLerpTime();
            yield return new WaitForSeconds(1);
        }
    }

    private void LerpScore()
    {
        lerpTime += Time.deltaTime * 2;
        gameScore = Mathf.Lerp(gameScore, lerpScore, lerpTime);
    }

    private void ResetLerpTime()
    {
        lerpTime = 0;
    }

    public void LevelFinished()
    {
        if (isGameOver) return;
        isTheLevelEnded = true;
    }

    public void GameOver()
    {
        if (isTheLevelEnded) return;
        isGameOver = true;
    }

    public void AddScoreFromEnemy(int _score)
    {
        lerpScore += _score; // * scoreMultiplier;
        GamePlayUI.instance.SetScoreTextSize();
    }
}
