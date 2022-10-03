using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] float speed;
    [SerializeField] int score;
    private GameObject player;
    // Start is called before the first frame update

    public void SetSpeed(float _speed) { speed = _speed; } 
    public float GetSpeed() { return speed; }
    public void SetPlayer(GameObject _player) { player = _player; }
    public GameObject GetPlayer() { return player; }
    public void SetScore(int _score) { score = _score; }
    public int GetScore() { return score; }

    void Awake()
    {
        instance = this;
    }
}
