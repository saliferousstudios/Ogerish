using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    [SerializeField] private LayerMask spikes;
    [SerializeField] private Text livesNumber;
    [SerializeField] private Scene EndGame;
    [SerializeField] private int lives = 1;
    [SerializeField] private Text laundryNumber;
    [SerializeField] private int laundry = 1;
    //[SerializeField] private LayerMask enemies;
    [SerializeField] private float hurtForce = 10f;
    private enum State { idle = 0, running = 1, jumping = 2, falling = 3, hurt = 4, climb1= 5, climb2 = 6, lookAround = 7};
    private State state = State.idle;
    private string[] animationState = { "OgreNormal_Idle", "OgreNormal_Run", "OgreNormal_Jump", "OgreNormal_Fall", "OgreNormal_Hurt", "Ogre_Climb1", "Ogre_Climb2", "OgreNormal_LookAround" };
    bool canDoubleJump = true;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "lives")
        {
            Destroy(collision.gameObject);
            lives++;
            livesNumber.text = lives.ToString();
        }
        else if (collision.tag == "laundry")
        {
            Destroy(collision.gameObject);
            laundry++;
            laundryNumber.text = laundry.ToString();
        }
        else if (collision.tag == "spikes")
        {
            state = State.hurt;
            if (collision.gameObject.transform.position.x > transform.position.x)
            {
                rigidBody.velocity = new Vector2(-hurtForce, hurtForce/2);
                print("to my left");
            }
            else
            {
                rigidBody.velocity = new Vector2(hurtForce, hurtForce / 2);
                print("to my right");
            }
            lives--;
            livesNumber.text = lives.ToString();
            StartCoroutine(Heal());
        }
    }

    void Start()
    {

        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        colider = GetComponent<Collider2D>();
    }


    private IEnumerator Heal()
    {
        yield return new WaitForSeconds(1f);
        state = State.idle;
    }

    // Update is called once per frame
    void Update()
    {
        if(lives <= 0)
        {
            print("end game");
            SceneManager.LoadScene("EndGame");
        }
        if (!(state == State.hurt))
       {
            PlayerInput();
            
        }
        PlayerAnimation();
    }

    public void PlayerAnimation()
    {
        animator.Play(animationState[(int)state]);
    }
    private IEnumerator Climb()
    {
        yield return new WaitForSeconds(.5f);
        if(state == State.climb1)
        {
            state = State.climb2;
        }
        else
        {
            state = State.climb1;
        }
    }

  
    int changeIdle = -1;
    int changeClimb = -1;

    public void PlayerInput()
    {
        if (state == State.climb1 || state == State.climb2)
        {
            rigidBody.gravityScale = 0;
        }
        else
        {
            rigidBody.gravityScale = 1;
        }
        if (state != State.idle && state != State.lookAround && changeIdle > -1)
        {
            changeIdle = -1;
        }

        if (state != State.climb1 && state != State.climb2 && changeClimb > -1)
        {
            changeClimb = -1;
        }

        // making sure that the sprite is facing the correct dirrection. If not, we want it to.
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftArrow))
        {
            spriteRenderer.flipX = true;
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.RightArrow))
        {
            spriteRenderer.flipX = false;
        }




        // Here we're splitting up what the animation can do. 
        if (colider.IsTouchingLayers(ground))
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
                rigidBody.velocity = new Vector2(speed, rigidBody.velocity.y);
                state = State.running;
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftArrow))
            {
                rigidBody.velocity = new Vector2(-speed, rigidBody.velocity.y);
                state = State.running;
            }

            else

            {
                if (changeIdle == -1)
                {
                    state = State.idle;
                }
                changeIdle++;

                if (changeIdle > 300)
                {
                    if (state == State.idle)
                    {
                        state = State.lookAround;
                    }
                    else if (state == State.lookAround)
                    {
                        state = State.idle;
                    }
                    changeIdle = 0;

                }
            }




        } else if (colider.IsTouchingLayers(ladder))
        {

            if (Input.GetKey(KeyCode.UpArrow))
            {
                
                if (changeClimb == -1)
                {
                    state = State.climb1;
                }
                changeClimb++;

                if (changeClimb > 50)
                {
                    if (state == State.climb1)
                    {
                        state = State.climb2;
                    }
                    else if (state == State.climb2)
                    {
                        state = State.climb1;
                    }
                    changeClimb = 0;
                }
                rigidBody.velocity = new Vector2(0, climbSpeed);

            }
            else if (Input.GetKeyUp(KeyCode.UpArrow) && (state == State.climb1 || state == State.climb2))
            {
               
                rigidBody.velocity = new Vector2(0, 0);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {

                if (changeClimb == -1)
                {
                    state = State.climb1;
                }
                changeClimb++;

                if (changeClimb > 50)
                {
                    if (state == State.climb1)
                    {
                        state = State.climb2;
                    }
                    else if (state == State.climb2)
                    {
                        state = State.climb1;
                    }
                    changeClimb = 0;
                }
                rigidBody.velocity = new Vector2(0, -climbSpeed);

            }
            else if (Input.GetKeyUp(KeyCode.DownArrow) && (state == State.climb1 || state == State.climb2))
            {

                rigidBody.velocity = new Vector2(0, 0);
            }
            else if (Input.GetKey(KeyCode.Space))
            {
               
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpSpeed);
                state = State.jumping;
            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.RightArrow))
            {
                rigidBody.velocity = new Vector2(speed / 4, rigidBody.velocity.y);
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftArrow))
            {
                rigidBody.velocity = new Vector2(-speed / 4, rigidBody.velocity.y);
            }
            else
            {
               
            }
        }
       else if( !colider.IsTouchingLayers(ladder))
        {
            if (state == State.climb1 || state == State.climb2)
            {
                state = State.falling;
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
            else if (rigidBody.velocity.y < 0 && state != State.climb1 && state != State.climb2)
            {
                state = State.falling;

            }
        }
        }




    }

