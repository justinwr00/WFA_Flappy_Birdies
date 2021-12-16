using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour {

    // Class description:
    // apply this class to any PAIR of boackground images, and reference eachother's Gameobject
    // in the inspector field labeled: partner
    // apply the SAME scroll spead to each image in the pair
    // make sure each image has a Rigidbody2D component with gravity multiplier set to 0
    // images should be set with the leftmost image in full-frame of the camer, with the second one offset to the right

    public GameObject partner;
    public float scrollSpeed;

    private Rigidbody2D rbSelf;
    private Rigidbody2D rbPartner;
    private float offset;
    private Vector2 posOffset;
    private Vector3 startingPos;
    private Vector3 warpPosition;

	// Use this for initialization
	void Start () {
        rbSelf = GetComponent<Rigidbody2D>();

        //get offset, the difference between the two paired images
        posOffset = partner.transform.position - rbSelf.transform.position;
        offset = Mathf.Abs(posOffset.x);

        // set startiing pos to self, or partner, whichever is more to the left
        startingPos = rbSelf.transform.position;
        rbPartner = partner.GetComponent<Rigidbody2D>();
        if (rbPartner.transform.position.x < rbSelf.transform.position.x)
        {
            startingPos = rbPartner.transform.position;
        }
        startingPos = new Vector3(startingPos.x - offset, startingPos.y, startingPos.z);
	}
	
	// Update is called once per frame
	void Update () {

        //scroll the background image
        rbSelf.transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);

        //check if we need to warp to back
        if (rbSelf.transform.position.x < startingPos.x)
        {
            warpPosition = new Vector3(rbPartner.transform.position.x + offset, rbSelf.transform.position.y, rbSelf.transform.position.z);
            rbSelf.transform.position = warpPosition;
        }

    }
}
