using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{

    private bool gameLost;

    private bool gamePaused;

    private bool gameStarted;

    public GameObject player;

    public GameObject spawner;

    public GameObject TutorialText;
    public GameObject GameOverText;
    public GameObject GamePausedText;

    public int AggroedEnemies;

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
        gamePaused = true;
        gameStarted = false;
        AggroedEnemies = 0;
        TutorialText.GetComponent<Text>().enabled = true;
        GameOverText.GetComponent<Text>().enabled = false;
        GamePausedText.GetComponent<Text>().enabled = false;
    }

    public void EndGame(){
        gameLost = true;
        gamePaused = false;
        player = null;

        spawner.GetComponent<SpawnerScript>().SpawnEnemies = false;
        
        GameOverText.GetComponent<Text>().enabled = true;
    }

    public void ResetGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void EnemyDied(EnemyController enemyController){
        player.GetComponent<PlayerController>().IncreaseUpgradePoints(enemyController.PointsReward);
    }

    void StartGame(){
        gameStarted = true;
        gamePaused = false;
        gameLost = false;
        TutorialText.GetComponent<Text>().enabled = false;
        spawner.GetComponent<SpawnerScript>().SpawnEnemies = true;
    }

    void ToggleUpgradeMenu(){
        if(gameLost) return;
        if(AggroedEnemies > 0) return;

        PlayerController playerController = player.GetComponent<PlayerController>();

        if(gamePaused && !playerController.CanBuildPlayer()) return;
        gamePaused = !gamePaused;

        if(gamePaused)
            playerController.ResetPlayer();

        PlayerConstructManager.Instance.ToggleInventory();

        if(!gamePaused)
            playerController.BuildPlayer(true);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        if(!gameStarted && Input.GetButtonDown("Vertical")){
            StartGame();
        }
        if(!gameStarted) return;
        
        if(Input.GetButtonDown("Jump")){
            ToggleUpgradeMenu();
        }

        if(Input.GetButtonDown("Cancel") && gameLost){
            ResetGame();
        }

        if (Input.GetButtonDown("Cancel") && !gameLost){ 
            gamePaused = !gamePaused;
            GamePausedText.GetComponent<Text>().enabled = gamePaused;

        }
    }
}
