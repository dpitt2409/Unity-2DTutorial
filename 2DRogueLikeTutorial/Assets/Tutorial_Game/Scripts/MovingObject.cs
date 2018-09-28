using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour {

    public float moveTime = 0.1f;
    public LayerMask blockingLayer;

    private BoxCollider2D boxCollider;
    //Rigidbody will be the object that is moving
    private Rigidbody2D rb2D;
    private float inverseMoveTime;

	// Can be used or redefined by children classes
	protected virtual void Start ()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;
	}
	
    
    //out parameter used to return two different objects, one of type bool and one of typed RayCastHit2D
    //Returns whether or movement is possible, as well as a hit object
    protected bool Move (int xDir, int yDir, out RaycastHit2D hit)
    {
        //Determine the start and end positions
        //By casting the Vector3 transform position to a Vector2, the z axis is removed
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        //Make sure that when checking for other objects, this objects BoxCollider is not hit
        boxCollider.enabled = false;
        //Check for a collision on the blockingLayer between the start and end positions
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        //If no collision was detected
        if (hit.transform == null)
        {
            StartCoroutine(SmoothMovement(end));
            return true;
        }
        //Return true if movement was possible, false otherwise
        return false;
    }



    //Movement method
    protected IEnumerator SmoothMovement (Vector3 end)
    {
        //Determine the distance between the object to be moved and the destination
        //square magnitude is better for computation than regular magnitude
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        //float.Epsilon is just a very small number close to 0
        while(sqrRemainingDistance > float.Epsilon)
        {
            //Determine the next position for the object to go, based on the current position,
            //the destination, and the moveTime speed
            //The MoveTowards function creates the smooth movement towards the destination
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            //Move the object to the new position
            rb2D.MovePosition(newPosition);
            //Recalculate the remaining distance after the object has moved
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
           
            yield return null;
        }

    }

    protected virtual void AttemptMove <T> (int xDir, int yDir)
        where T : Component
    {
        //Call Move function to determine if movement is possible
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        
        if (hit.transform == null)
            return;

        T hitComponent = hit.transform.GetComponent<T>();

        if (!canMove && hitComponent != null)
            OnCantMove(hitComponent);
    }

    //Abstract method to be defined by inheriting classes
    //Variable type T also to be defined by inheriting class
    protected abstract void OnCantMove<T>(T Component)
        where T : Component;


}
