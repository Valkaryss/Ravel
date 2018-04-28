using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartbeatRunner : MonoBehaviour {

  public float effectiveDistance = 100f;
  private GameObject pc;
  private Runner runner;
  private AudioSource main;
  private AudioSource heartbeat;

	// Use this for initialization
	void Start () {
    pc = GameObject.Find("OVRPlayerController");
    runner = pc.GetComponent<Runner>();
    main = runner.mainMusic;
    heartbeat = runner.heartbeatMusic;
	}
	
	// Update is called once per frame
	void Update () {
    float distance = Vector3.Magnitude(pc.transform.position - transform.position);

    if (distance < effectiveDistance)
    {
      float ratio = distance / effectiveDistance;
      heartbeat.volume = 1 - ratio;
      main.volume = ratio;
    }
	}
}
