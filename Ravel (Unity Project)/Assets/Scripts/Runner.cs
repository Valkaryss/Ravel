using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner : MonoBehaviour {
  [Tooltip("Player controller script, please!")]
  public OVRPlayerController pc; // PC culture gone too far
  [Tooltip("GameObject where the user will float to on house click")]
  public GameObject startTargetPosition; // Empty GameObject stair target
  [Tooltip("Please tell me where the collider for b2 is, I need to disable it.")]
  public GameObject bedroomCollider2;
  Vector3 bedroom2Location;

  public GameObject letterLocation;
  Vector3 letterLoc;


  enum State { start, searching };
  State state;
  Vector3 stairPosition; // Stair location
  Vector3 restartPosition; // Reset position in spaaaaace
  bool traveling = false;

  Hashtable locationTracker;

  // Use this for initialization
  void Start() {
    state = State.start;
    restartPosition = transform.position;
    stairPosition = startTargetPosition.transform.position;
    letterLoc = letterLocation.transform.position;
    pc.GravityModifier = 0;
    locationTracker = new Hashtable();
    locationTracker["Bedroom"] = false;
    locationTracker["Kitchen"] = false;
    locationTracker["Basement"] = false;
    locationTracker["Bathroom"] = false;
    disableBedroom2();
  }

  // Move the bedroom so far away no one will ever find it
  void disableBedroom2()
  {
    bedroom2Location = bedroomCollider2.transform.position;
    bedroomCollider2.transform.position = new Vector3(999999, 9999999, 999999);
  }

  // Move the bedroom2 collider back to a useful place
  void enableBedroom2()
  {
    bedroomCollider2.transform.position = bedroom2Location;
  }
  // Update is called once per frame
  void Update() {
    if (state == State.start)
    {
      startUpdate();
    }
    else
    {
      searchingUpdate();
    }

    // Rotation for testing
    float rSpeed = 30f;
    if (Input.GetKey(KeyCode.K))
    {
      transform.Rotate(new Vector3(0, -rSpeed * Time.deltaTime, 0));
    }
    if (Input.GetKey(KeyCode.L))
    {
      transform.Rotate(new Vector3(0, rSpeed * Time.deltaTime, 0));
    }
  }

 
  // ---------------------------------------------------------------------------------------
  // ------------------------------------- Start State -------------------------------------
  // ---------------------------------------------------------------------------------------
  [Tooltip ("The speed at which we fly from space to the stairs")]
  public float lerpSpeed = 1;
  // Update function in the "start" state
  void startUpdate()
  {
    // Dummy trigger for now, this should actually be when you pull the house towards you
    if (Input.GetKeyDown(KeyCode.Return))
    {
      traveling = true;
    }
    // Fly, my pretty!
    if (traveling) {
      transform.position = Vector3.Lerp(transform.position, stairPosition, lerpSpeed * Time.deltaTime);
    }

    // Are we there yet?
    if (almostEquals(transform.position, stairPosition))
    {
      traveling = false;
      pc.GravityModifier = 1;
      state = State.searching;
      drawText(letterLoc, "You see a letter", 0, 5);
      drawText(letterLoc, "You pick it up and open it", 5.1f, -1);
    }
  }

  // ---------------------------------------------------------------------------------------
  // ----------------------------------- Searching State -----------------------------------
  // ---------------------------------------------------------------------------------------

  // Update function for "searching" state
  void searchingUpdate()
  {
    checkReset();
    checkEnableBedroom2();
  }

  // Check to see if the player is resetting to starting position, and do it if they are.
  void checkReset()
  {
    if (Input.GetKeyDown(KeyCode.R))
    {
      traveling = true;
      pc.GravityModifier = 0;
    }
    // !ytterp ym, ylF
    if (traveling)
    {
      transform.position = Vector3.Lerp(transform.position, restartPosition, lerpSpeed * Time.deltaTime);
    }
    // Are we there yet?
    if (almostEquals(transform.position, restartPosition))
    {
      Start();
    }
  }

  // Check to see whether or not we've been to every room yet. We're only allowed
  // to access the second interaction in the bedroom if we've been everywhere else 
  // already.
  void checkEnableBedroom2()
  {
    foreach(string k in locationTracker.Keys)
    {
      if (!((bool) locationTracker[k])){ return; }
    }
    enableBedroom2();
  }

  // How to handle triggering events based on collisions with trigger zones.
  // How to use:
  // Create an empty game object. Name it whatever you like (henceforth, <name>)
  // In this function, create a new case statement that follows the following form

  //  case("<name>"):
  //    drawText(p, "Your Text Here", time to creation in seconds, lifespan of text in seconds)
  //    drawText(p, "Your Other Text Here", delay, life);
  //    . . . 
  //    break;
  private void OnTriggerEnter(Collider other)
  {
    Vector3 p = other.transform.position;
    switch (other.name)
    {
      case ("Test"):
        drawText(p, "Testing testing 123", 0, 5);
        break;
      case ("Bedroom1"):

        markVisited("Bedroom");
        break;
      default:
        drawText(p, "DEFAULTY TEXT", 0,-1);
        break;
    }
    Destroy(other.gameObject);
  }

  // Tell the location tracker that we've been to this room, with snarky data verification.
  void markVisited(string roomName)
  {
    if (!locationTracker.ContainsKey(roomName)){ print("You fucked up. Write better code."); Debug.Break();}
    locationTracker[roomName] = true;
  }

  // Create a thing that says a thing in a place
  void drawText(Vector3 p, string t, float delay, float life)
  {
    StartCoroutine(delayedDrawText(p, t, delay, life));
  }

  // Coroutine helper function to drawText. Created a text mesh after delay-many seconds,
  // then either waits life-many seconds or waits for the user to press 'r', depending
  // on whether a valid life is provided (to allow for persistent text)
  IEnumerator delayedDrawText(Vector3 p, string t, float delay, float life)
  {
    yield return new WaitForSeconds(delay);

    GameObject result = new GameObject();
    result.transform.position = p;
    result.AddComponent<TextMesh>();
    result.transform.LookAt(transform.position);
    result.transform.Rotate(new Vector3(0, 180, 0));
    TextMesh textMesh = result.GetComponent<TextMesh>();
    textMesh.text = t;
    textMesh.characterSize = 5f;
    textMesh.anchor = TextAnchor.MiddleCenter;

    if (life > 0){ yield return new WaitForSeconds(life);}
    else{ yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.R));}

    Destroy(result);
  }

  // ---------------------------------------------------------------------------------------
  // ---------------------------------- UTILITY FUNCTIONS ----------------------------------
  // ---------------------------------------------------------------------------------------

  [Tooltip("Sensitivity of \"close enough\" to the stairs")]
  public float e = 20f;
  // Returns true iff vector p is close enough to vector q for us to not care, as defined by epsilon
  bool almostEquals(Vector3 p, Vector3 q)
  {
    Vector3 epsilon = new Vector3(e, e, e);
    return lessThan(v3Abs(p - q), epsilon);
  }

  // Takes the component-wise absolute value of a vector
  Vector3 v3Abs(Vector3 v)
  {
    return new Vector3(Mathf.Abs(v.x),Mathf.Abs(v.y), Mathf.Abs(v.z));
  }
  // Returns true iff every value of p is less than every value of q
  bool lessThan(Vector3 p, Vector3 q)
  {
    return p.x < q.x && p.y < q.y && p.z < q.z;
  }
}
