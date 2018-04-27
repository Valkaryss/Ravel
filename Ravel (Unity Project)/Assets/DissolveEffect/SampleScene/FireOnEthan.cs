using UnityEngine;
using System.Collections;

public class FireOnEthan : MonoBehaviour {

    public GameObject targetFire;
	
	// Update is called once per frame
	void Update () {
        Vector3 pos = transform.position;
        pos.x = targetFire.transform.position.x;
        pos.z = targetFire.transform.position.z;
        transform.position = pos;
	}
}
