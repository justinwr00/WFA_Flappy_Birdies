using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControllerFlappyBird : MonoBehaviour {

    public float upForce;
    public float maxAngle;
    public float minAngle;
    public float upRotateSpeed;
    public float downRotateSpeed;
    public string jumpButton;
    public Text scoreCountText; // used for end-of-round scorecard
    private AudioSource Asource;
    public AudioClip flapSound;
    

    private Rigidbody2D rb;
    private Vector3 startingPos;  // player starting/idle position

    public GameObject ScoreParticle, DieParticle, ScoreCard;
    private Text scoreText;

    private Animator anim;

    // State management property
    private string playerState;
    public string PlayerState
        {
         get
           {
               return playerState;
           }

          set
            {
               playerState = value;
            }
         }

    // player score property
    private int score;
    public int Score
    {
        get
        {
            return score;
        }
        set
        {
            scoreCountText.text = value.ToString();
            score = value;
        }
    }

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        // initialize starting position
        startingPos = rb.transform.position;
        PlayerState = "Idle";
        // PlayerState = "InPlay";
        anim = GetComponentInChildren<Animator>();
        Asource = GetComponent<AudioSource>();
        Asource.clip = flapSound;
    }
	
	// Update is called once per frame
	void Update () {

        // update player based on state machine
        switch (PlayerState)
        {
            case "Idle":
                // pin player to starting position
                rb.transform.position = startingPos;
                rb.velocity = new Vector2(0f, 0f);
                break;

            case "InPlay":
                // listen for input and move player
                PlayerMovement();
                anim.SetFloat("ySpeed", rb.velocity.y);
                // check if grounded
                if (transform.position.y <= -3.5f)
                {
                    transform.position = new Vector3(transform.position.x, -3.5f, transform.position.z);
                    rb.rotation = 0f;
                    rb.velocity = new Vector2(0f, 0f);
                }

                break;

            default:
                break;
        }

	}

    private void PlayerMovement()
    {
        // handle the player jumps and rotation while in play
        // listen for input here
        // if jump button pressed, make the bird jump

        if (Input.GetButtonDown(jumpButton))
        {

            //make the bird jumpy jump
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(0, upForce));
            anim.SetTrigger("Flapping");
            
            Asource.Play();
        }

        // rotate player up if y velocity is positive, downward otherwise
        if (rb.velocity.y > 0f)
        {
            rb.rotation = rb.rotation + (upRotateSpeed * Time.deltaTime);
        }
        else
        {
            rb.rotation = rb.rotation - (downRotateSpeed * Time.deltaTime);
        }

        // keep player rotation in bounds
        if (rb.rotation > maxAngle)
        {
            rb.rotation = maxAngle;
        }
        if (rb.rotation < minAngle)
        {
            rb.rotation = minAngle;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Pipe" && PlayerState == "InPlay")
        {
            PipeAudio PC = collision.GetComponent<PipeAudio>();
            PC.PlayDieSound();
            KillPlayer();
        }

        if (collision.CompareTag("Gate"))
        {
            // this is when the player scores a point by passing between the top and bottom pipes
            // Debug.Log(this.name.ToString() + " scored a point");
            
            // Increment score
            Score++;
            Instantiate(ScoreParticle, new Vector3(transform.position.x,transform.position.y + 0.6f,transform.position.z),Quaternion.identity);
            // instantiate a scorecard here
            GameObject thisScore = Instantiate(ScoreCard, transform.position, Quaternion.identity) as GameObject;
            scoreText = thisScore.GetComponentInChildren<Text>();
            scoreText.text = Score.ToString();
            scoreCountText.text = Score.ToString();
            PipeGate PG = collision.GetComponent<PipeGate>();
            PG.PlayScoreSound();
        }
    }

    private void KillPlayer()
    {
        // feather explosion
        if (PlayerState == "InPlay") 
            {
                Instantiate(DieParticle, transform.position, Quaternion.identity);
            }
            
        // this function kills the player
        PlayerState = "Idle";
        
    }

}
