using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	
	public float walkSpeed = 800f;
	private bool isGrounded = true;
    bool isMoving = false;
	bool allowHorizontal = true;
	


	Animator animator;
	public Rigidbody2D rgd;
	public float jumpSpeed = 500000f;

	//animation states - the values in the animator conditions
	const int STATE_IDLE = 0;
	const int STATE_WALK = 1;
	const int STATE_IDLE_JUMP = 2;
	const int STATE_WALK_JUMP = 3;
	const int STATE_CROUCH = 4;

	int currentAnimationState = STATE_IDLE;
	string currentDirection = "left";


	// Use this for initialization
	void Start () {
		animator = this.GetComponent<Animator>();
		rgd = GetComponent <Rigidbody2D>();
		animator.SetInteger ("state", STATE_IDLE_JUMP);
	}

	// Update is called once per frame
	void Update () {
		

	}


	void FixedUpdate() {
		//allow horizontal movement when walking/sprinting > jump/ crouching
		if (this.animator.GetCurrentAnimatorStateInfo (0).IsName("Idle_Jump") || 
			this.animator.GetCurrentAnimatorStateInfo (0).IsName("crouch_Idle") || 
			this.animator.GetCurrentAnimatorStateInfo (0).IsName("crouch_Out") || 
			this.animator.GetCurrentAnimatorStateInfo (0).IsName("crouch_In")) {allowHorizontal = false;} 
		else {allowHorizontal = true;}

		//idle animation if no movement keys pressed
		if (!Input.GetKeyDown("left") && !Input.GetKeyDown("right") && !Input.GetKeyDown("up")&& !Input.GetKeyDown("down") && isGrounded == true){  
            animator.SetInteger("state", STATE_IDLE);
            currentAnimationState = STATE_IDLE;
            isMoving = false;
        }

		//jump
		if (Input.GetKeyDown("up") && isGrounded) {	
			rgd.AddForce (Vector3.up * jumpSpeed);
			animator.ResetTrigger ("land");

			isGrounded = false;
			isMoving = true;
			//walk>jump , horizontal movemente allowed
			if (Input.GetKey ("right") || Input.GetKey ("left")) {
				animator.SetTrigger ("WJump");
				currentAnimationState = STATE_WALK_JUMP;
				
			} else {
				//idle jump, no horzontal movement allowed (for now)
				allowHorizontal = false;
				animator.SetInteger ("state", STATE_IDLE_JUMP);
				
			}
		}
		if (Input.GetKey("down") && isGrounded) {
			currentAnimationState = STATE_CROUCH;
			animator.SetInteger ("state", STATE_CROUCH);
			allowHorizontal = false;
		}
		//horizontal movement animations
		else if ((Input.GetKey("right") || Input.GetKey("left")) && (allowHorizontal) )
		{
			
			animator.SetInteger("state", STATE_WALK);
            isMoving = true;
			if (currentAnimationState == STATE_WALK_JUMP) {
				transform.Translate (Vector2.left * (walkSpeed * 1.9f) * Time.deltaTime);
			} 
			else {transform.Translate (Vector2.left * walkSpeed * Time.deltaTime);
			}


            
        }
        if (Input.GetKey("left") && !Input.GetKey("right")){changeDirection("left");}		//change direction
        else if (Input.GetKey("right") && !Input.GetKey("left")) changeDirection("right");

	
	}

// Check if player has collided with the floor
	void OnCollisionEnter2D(Collision2D coll)		//collision detection
	{
		if (coll.gameObject.name == "Floor")
		{
			animator.SetTrigger ("land");
			isGrounded = true;
            if (currentAnimationState != STATE_IDLE){
                animator.SetInteger("state", STATE_IDLE);
                currentAnimationState = STATE_IDLE;
            }
        }

	}

	void changeDirection(string direction)
	{

		if (currentDirection != direction)
		{
			if (direction == "right")
			{
				transform.Rotate(0, 180, 0);
				currentDirection = "right";
			}
			else if (direction == "left")
			{
				transform.Rotate(0, -180, 0);
				currentDirection = "left";
			}
		}

	}


}
