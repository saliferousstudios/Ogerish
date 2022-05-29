using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OgreNormalMovement : MonoBehaviour
{

    [SerializeField] private float speed = 1.2f;
    [SerializeField] private float jumpSpeed = 1.2f;
    [SerializeField] private float climbSpeed = 1.2f;
    private Rigidbody2D rigidBody;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Collider2D colider;
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask ladder;
    //[SerializeField] private LayerMask enemies;
    [SerializeField] private float hurtForce = 10f;
    private Vector2 velocity;
    private enum State { idle = 0, running = 1, jumping = 2, falling = 3, climb = 4};
    private State state = State.idle;
    private string[] animationState = { "OgreNormal_Idle", "OgreNormal_Run", "OgreNormal_Jump", "OgreNormal_Fall", "OgreNormal_Climb"};
    bool canDoubleJump = true;
   // bool canClimb = false;
/*
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "ladder")
        {
            canClimb = true;
        }
        
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "ladder")
        {
            canClimb = false;
        }
    }*/

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        colider = GetComponent<Collider2D>();
    }


    // Update is called once per frame
    void Update()
    {
       // if (!(state == State.hurt))
       // {
            PlayerInput();
            //  PlayerMove();
            //
      //  }
        PlayerAnimation();
    }

    public void PlayerAnimation()
    {
        animator.Play(animationState[(int)state]);
    }

    public void PlayerInput()
    {

        // making sure that the sprite is facing the correct dirrection. If not, we want it to.
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftArrow))
        {
            spriteRenderer.flipX = true;
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.RightArrow))
        {
            spriteRenderer.flipX = false;
        }

        if (colider.IsTouchingLayers(ladder))
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                state = State.climb;
                rigidBody.velocity = new Vector2(0, climbSpeed);
                rigidBody.gravityScale = 0;


            }
            else if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                rigidBody.velocity = new Vector2(0, 0);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                state = State.climb;
                rigidBody.velocity = new Vector2(0, -climbSpeed);
                rigidBody.gravityScale = 0;


            }
            else if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                rigidBody.velocity = new Vector2(0, 0);
            }
        }

        // Here we're splitting up what the animation can do. 
        else if (colider.IsTouchingLayers(ground))
        {
            if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) && colider.IsTouchingLayers(ground))
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpSpeed);
                state = State.jumping;
            }
            else if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && colider.IsTouchingLayers(ground))
            {
                // state = State.crouch;
                rigidBody.velocity = new Vector2(0, 0);
            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.RightArrow))
            {
                print("right");
                rigidBody.velocity = new Vector2(speed, rigidBody.velocity.y);
                state = State.running;
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftArrow))
            {
                print("left");
                rigidBody.velocity = new Vector2(-speed, rigidBody.velocity.y);
                state = State.running;
            }

            else

            {
                state = State.idle;
            }
        }
        else
        {
            if (canDoubleJump && (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) && colider.IsTouchingLayers(ground))
            {
                canDoubleJump = false;
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpSpeed);
                state = State.jumping;
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, -jumpSpeed * 2);
                // state = State.crouch;
            }
            else if (rigidBody.velocity.y < 0)
            {
                state = State.falling;

            }
        }
        }




    }

