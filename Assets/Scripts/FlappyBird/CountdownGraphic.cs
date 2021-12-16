using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownGraphic : MonoBehaviour {
    
    public float Speed, Scalefactor;
    private Rigidbody2D rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(DestroySelf());
	}
	
	// Update is called once per frame
	void Update () {

        rb.transform.position = new Vector3(rb.transform.position.x, rb.transform.position.y + (Speed*Time.deltaTime),rb.transform.position.z);
        transform.localScale -= new Vector3(Scalefactor*Time.deltaTime, Scalefactor * Time.deltaTime, Scalefactor * Time.deltaTime);
	}

    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(0.9f);
        Destroy(this.gameObject);
    }
}
