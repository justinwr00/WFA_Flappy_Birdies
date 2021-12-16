using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingButton : MonoBehaviour {

    private Rigidbody2D rb;
    private Vector3 startPos;
   // private float phi = 0f; // value/angle used in the sine function

   // public float Speed;       //speed of oscillation
   // public float Scalar;    //scalar parameter for oscillator.. determines "height" of bounce

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        startPos = rb.transform.position;
        //phi = 0f;
	}
	
	// Update is called once per frame
	void Update () {
       // phi += Speed * Time.deltaTime;
       // if (phi > 360f)
       //     phi = 0f;
        // rb.transform.position = new Vector3(rb.transform.position.x,startPos.y + (Mathf.Sin(Time.fixedTime))*Scalar,rb.transform.position.z);
        rb.transform.position = new Vector3(rb.transform.position.x, startPos.y + GameControllerFlappyBird.Oscillation, rb.transform.position.z);
        
    }
}
