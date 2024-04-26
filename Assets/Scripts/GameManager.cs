using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    [SerializeField] private int _maxLives =3;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private Transform _playerSpawn;

    private int _score;
    private int _remainingLives;
    private GameObject _player;
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        } else
        {
            _instance = this;
        }
        _remainingLives = _maxLives;
        RespawnPlayer();
    }

    private void OnDestroy()
    {
        if(_instance == this)
        {
            _instance = null;
        }
    }

    public int GetScore()  { return _score; }

    public void PlayerDeath()
    {
        _remainingLives -= 1;
        if (_remainingLives <= 0)
        {
            //Do game over
        }else
        {
            RespawnPlayer();
        }
    }

    private void RespawnPlayer()
    {
        if(_player != null ) 
        {
            Destroy(_player);
        }        
        _player = Instantiate(_playerPrefab, _playerSpawn.position,_playerSpawn.rotation);
    }
}
