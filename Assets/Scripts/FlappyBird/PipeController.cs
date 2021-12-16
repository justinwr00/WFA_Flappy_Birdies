using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeController : MonoBehaviour {

    public float scrollSpeed = 5.0f;
    public GameObject PipeBottom;
    private Rigidbody2D rbPartner; // rigidbody of pipe bottom
    public int pipeNumber; // an ID number for which pipe object is, used for the random generator in Game Manager

    private Rigidbody2D rb;
    private Vector3 startingPos; // starting position in object pool

    

    // boolean property pipe prefab is in play or idle in object pool
    private bool isInPlay = false;
    public bool IsInPlay
    {
        get
        {
            return isInPlay;
        }
        set
        {
            isInPlay = value;
        }
    }

    // property for gap distance between top and bottom pipe
    private float pipeGap;
    public float PipeGap
    {
        get
        {
            return pipeGap;
        }
        set
        {
            pipeGap = value;
        }
    }


	// Use this for initialization
	void Start () {
        //grab rigidbody2d 
        rb = GetComponent<Rigidbody2D>();

        //grab rigidbody of pipe bottom that follows this pipe around
        rbPartner = PipeBottom.GetComponent<Rigidbody2D>();

        //save starting position in object pool
        startingPos = rb.transform.position;

       // Asource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {

        // scroll object if in play
        if (isInPlay)
        {
            rb.transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);
            rbPartner.transform.position = new Vector3(rb.transform.position.x, rb.transform.position.y - PipeGap, rb.transform.position.z);
        }
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // reset pipe to object pool when it reaches the finish line
        if (collision.name == "FinishLine")
        {
            IdleToPool();
        }
    }

    private void IdleToPool()
    {
        // reset position to starting position and go back to idle state
        IsInPlay = false;
        rb.transform.position = startingPos;
        rb.velocity = new Vector2(0f, 0f);
    }

    
}
