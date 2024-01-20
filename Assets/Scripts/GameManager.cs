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

    public GameObject spawner;
    
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
    }

    public void EndGame(){
        gameLost = true;
        gamePaused = false;
        player = null;

        spawner.GetComponent<SpawnerScript>().SpawnEnemies = false;
        //TODO: display basic controls
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
        spawner.GetComponent<SpawnerScript>().SpawnEnemies = true;
    }

    void ToggleUpgradeMenu(){
        if(gameLost) return;
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
        if(!gameStarted && Input.GetButtonDown("Jump")){
            StartGame();
        }
        if(!gameStarted) return;

        if(Input.GetButtonDown("Jump")){
            ToggleUpgradeMenu();
        }

        if (Input.GetButtonDown("Cancel")){ 
            gamePaused = !gamePaused;
            //Show pause text
        }
    }
}
