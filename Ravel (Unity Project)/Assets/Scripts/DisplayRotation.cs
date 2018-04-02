using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayRotation : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
    void Update(){
        transform.Rotate(new Vector3(0, 3 * Time.deltaTime, 0),  Space.World);
    }
}
