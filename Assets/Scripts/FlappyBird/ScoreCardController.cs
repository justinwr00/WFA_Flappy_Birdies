using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCardController : MonoBehaviour {

    private RectTransform rT;
    public float speed;
    public float lifetime; // how long the prefab should wait before self-destruct

	// Use this for initialization
	void Start () {
        rT = GetComponent<RectTransform>();
        StartCoroutine(SelfDestruct());
	}
	
	// Update is called once per frame
	void Update () {
        rT.position = new Vector3(rT.position.x,rT.position.y + (speed*Time.deltaTime), rT.position.z);
	}

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(this.gameObject);
    }
}
