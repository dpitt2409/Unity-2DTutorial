using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MovingObject {

    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
    public Text foodText;
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;

    private Animator animator;
    private int food;

	// Use this for initialization
	protected override void Start ()
    {
        animator = GetComponent<Animator>();
        food = GameManager.instance.playerFoodPoints;
        //Display food value
        foodText.text = "Food: " + food;
        base.Start();
	}


    private void OnDisable()
     {
            //Used to store value of food as level is changed
            GameManager.instance.playerFoodPoints = food;
     }
   

    // Update is called once per frame
    void Update () {
        //stop if it is not the players turn
        if (!GameManager.instance.playersTurn) return;

        int horizontal = 0;
        int vertical = 0;

        //Assign horizontal and vertical variables based on current user input
        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        //Prevent Diagonal motion
        if (horizontal != 0)
            vertical = 0;

        //If there is a current input, call the AttemptMove function
        if (horizontal != 0 || vertical != 0)
            AttemptMove<Wall>(horizontal, vertical);
	}

    protected override void AttemptMove <T> (int xDir, int yDir)
    {
        //Everytime the player moves, they lose one food point
        food--;
        foodText.text = "Food: " + food;
        base.AttemptMove<T>(xDir, yDir);
        RaycastHit2D hit;
        //Use hit object to check if move was made and play sound effect
        if (Move(xDir, yDir, out hit))
        {
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }


        CheckIfGameOver();
        GameManager.instance.playersTurn = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //If the colliding object is the exit, restart the level after 1s
        if (other.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        //If the colliding object is food or soda, increase the food by the appropriate amount and delete the object
        else if (other.tag == "Food")
        {
            food += pointsPerFood;
            foodText.text = "+" + pointsPerFood +  " Food: " + food;
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Soda")
        {
            food += pointsPerSoda;
            foodText.text = "+" + pointsPerSoda + " Food: " + food;
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
            other.gameObject.SetActive(false);
        }
    }

    protected override void OnCantMove<T>(T component)
    {
        //Cast parameter component as a Wall
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        //Set the player animation for the playerChop
        animator.SetTrigger("playerChop");
    }

    private void Restart()
    {
        //Application.LoadLevel(Application.loadedLevel);
        //SceneManager.LoadScene(0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        foodText.text = "-" + loss + " Food: " + food;
        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if (food <= 0)
        {
            //Play gameover music and stop background music
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            GameManager.instance.GameOver();
        }
    }

}
