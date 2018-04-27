using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dissolveController : MonoBehaviour {
    public bool dissolved;
    public bool destroyOnDissolve;

    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        //Replace these triggers vvv. Right now you press space to dissolve/undissolve
        if (Input.GetKeyDown("space") && !dissolved)
        {
            gameObject.GetComponent<CustomDissolve>().Dissolve();
            dissolved = true;
        }
        //                      vvv
        else if (Input.GetKeyDown("space") && dissolved){
            gameObject.GetComponent<CustomDissolve>().Undissolve();
            dissolved = false;
            if (destroyOnDissolve) Destroy(gameObject, gameObject.GetComponent<CustomDissolve>().dissolveDuration);
        }
    }
}
