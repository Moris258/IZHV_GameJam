using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{

    private bool gameLost;

    private bool gamePaused;

    private bool gameStarted;

    public GameObject player;

    public List<GameObject> enemies;

    private static GameManager instance;

    public static GameManager Instance {get {return instance;}}

    public bool GameLost{get {return gameLost;}}

    public bool GamePaused{get {return gamePaused;}}

    public bool GameStarted{get {return gameStarted;}}

    private void Awake(){
        if(instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;

        GameSetup();
    }

    public void GameSetup(){
        gameLost = false;
        gamePaused = false;
        gameStarted = true;
    }

    public void EndGame(){
        gameLost = true;
        gamePaused = false;
        player = null;
        //Do other stuff
    }

    public void ResetGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void EnemyDied(){
        player.GetComponent<PlayerController>().IncreaseUpgradePoints(10);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
