using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] float timeSinceLevelStarted;
    [SerializeField] float speed;
    [SerializeField] float score;
    [SerializeField] int scorePerSecond;
    [SerializeField] float scoreMultiplier = 1;
    [SerializeField] float globalZposition;
    [SerializeField] bool isTheLevelEnded = false;
    [SerializeField] bool isGameOver = false;

    private GameObject player;

    float lerpScore;
    float lerpTime;
    // Start is called before the first frame update

    public void SetSpeed(float _speed) { speed = _speed; } 
    public float GetSpeed() { return speed; }
    public void SetPlayer(GameObject _player) { player = _player; }
    public GameObject GetPlayer() { return player; }
    public void SetScore(int _score) { score = _score; }
    public int GetScore() { return (int)score; }
    public float GetZPosition() { return globalZposition; }
    public void SetScoreMultiplier(float _scoreMultiplier) { scoreMultiplier = _scoreMultiplier; }
    public float GetScoreMultiplier() { return scoreMultiplier; }

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartCoroutine(ScoreAdditionPerSecond(1));
    }

    private void Update()
    {
        timeSinceLevelStarted = Time.deltaTime;

        LerpScore();
        ScoreMultiplierLayer();
    }

    private void ScoreMultiplierLayer()
    {
        if (isTheLevelEnded || isGameOver) return; 

        Vector3 playerPos = Camera.main.WorldToViewportPoint(player.transform.position);

        if (playerPos.x < 0.8f) scoreMultiplier = 2.5f;
        if (playerPos.x < 0.6f) scoreMultiplier = 2;
        if (playerPos.x < 0.4f) scoreMultiplier = 1.5f;
        if (playerPos.x < 0.2f) scoreMultiplier = 1;

    }

    IEnumerator ScoreAdditionPerSecond(float _spawnrate)
    {
        while (true)
        {
            lerpScore += scorePerSecond * scoreMultiplier;
            ResetLerpTime();
            yield return new WaitForSeconds(_spawnrate);
        }
    }

    public void AdditionalScore(int _score)
    {
        lerpScore += _score * scoreMultiplier;
        ResetLerpTime();
    }

    private void LerpScore()
    {
        lerpTime += Time.deltaTime * 2;
        score = Mathf.Lerp(score, lerpScore, lerpTime);
    }

    private void ResetLerpTime()
    {
        lerpTime = 0;
    }

    public void LevelFinished()
    {
        isTheLevelEnded = true;
    }

    public void GameOver()
    {
        isGameOver = true;
    }
}
