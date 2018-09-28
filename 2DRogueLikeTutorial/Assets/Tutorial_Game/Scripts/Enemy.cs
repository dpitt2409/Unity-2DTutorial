using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject {

    public int playerDamage;

    private Animator animator;
    private Transform target;
    //Enemy is set up to move every other turn
    private bool skipMove;
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;

    protected override void Start()
    {
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        //Only move every other turn
        if (skipMove)
        {
            skipMove = false;
            return;
        }
        base.AttemptMove<T>(xDir, yDir);
        skipMove = true;
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        //If the player and the enemy are in the same column, compare their y coordinate values
        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
            //If the player has a higher y coordinate value, set yDir to 1, else -1
            yDir = target.position.y > transform.position.y ? 1 : -1;
        else
            //If the player and the enemy are not in the same column, the enemy moves in the x direction
            xDir = target.position.x > transform.position.x ? 1 : -1;

        //After setting the xDir and yDir values, call the AttemptMove function
        //The passed variable type is Player, because that is the type of object enemies can interact with
        AttemptMove<Player>(xDir, yDir);
    }

    //When the enemy cannot move, it means that a player is in the way and the enemy should perform an attack
    protected override void OnCantMove<T>(T component)
    {
        //Cast component parameter to Player type
        Player hitPlayer = component as Player;
        //Set the hit animation for the enemy
        animator.SetTrigger("enemyAttack");
        //Call LoseFood function for the hit Player
        hitPlayer.LoseFood(playerDamage);
        //Play enemy attack sound effect
        SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
    }


}
