using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public float levelStartDelay = 2f;
    public float turnDelay = 0.1f;
    public static GameManager instance = null;
    public BoardManager boardScript;
    public int playerFoodPoints = 100;
    [HideInInspector] public bool playersTurn = true;

    private Text levelText;
    private GameObject levelImage;
    private int level = 1;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doingSetup = true;
    
	// Use this for initialization
	void Awake ()
    {
        //Ensure that there can only be one instance of this class
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //In between scenes, all objects in the hierarchy are destroyed. 
        //This ensures that the GameManager does not get destroyed in between levels
        DontDestroyOnLoad(gameObject);

        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
        InitGame();
	}


    //this is called only once, and the paramter tell it to be called only after the scene was loaded
    //(otherwise, our Scene Load callback would be called the very first load, and we don't want that)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization()
    {
        //register the callback to be called everytime the scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //This is called each time a scene is loaded.
    static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        instance.level++;
        instance.InitGame();
    }




    void InitGame()
    { 
        doingSetup = true;

        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        //Create the new text based on the current level
        levelText.text = "Day " + level;
        //Enable black background
        levelImage.SetActive(true);
        //Display the title image for levelStartDelay amount of time
        Invoke("HideLevelImage", levelStartDelay);

        enemies.Clear();
        boardScript.SetupScene(level);
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    public void GameOver()
    {
        //Set game over text
        levelText.text = "After " + level + " days, you starved.";
        //Enable black background
        levelImage.SetActive(true);
        enabled = false;
    }

    // Update is called once per frame
    void Update() {
        //if it is neither the players turn nor the enemies turn, move the enemy
        if (playersTurn || enemiesMoving || doingSetup)
            return;

        StartCoroutine(MoveEnemies());
	}

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    IEnumerator MoveEnemies()
    {
        //Signifies enemies turn
        enemiesMoving = true;

        //Pause for turnDelay amount of time
        yield return new WaitForSeconds(turnDelay);

        //If no enemies have spawned, perform an additional pause
        if(enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }

        //Iterate through each enemy and call MoveEnemy() for each
        for(int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            //Pause for the time taken for this enemy to move
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        //Switch turns
        playersTurn = true;
        enemiesMoving = false;

    }


}
