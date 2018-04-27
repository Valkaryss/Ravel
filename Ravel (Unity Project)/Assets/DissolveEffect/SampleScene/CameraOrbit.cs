using UnityEngine;
using System.Collections;

public class CameraOrbit : MonoBehaviour {

    public GameObject target;//the target object
    public float speedMod = 10.0f;//a speed modifier
    public float distance = 20f;
    public GameObject EthanFires;

    public GameObject Ethan, Sphere, Monument;

    private Vector3 point;//the coord to the point where the camera looks at
    private bool rotate = true;

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 30), "Ethan on fire!"))
        {
            target = Ethan;
            distance = 11;
            Ethan.GetComponent<CustomDissolve>().Dissolve();
        }
        if (GUI.Button(new Rect(200, 10, 150, 30), "Reverse Ethan"))
        {
            target = Ethan;
            distance = 11;
            Ethan.GetComponent<CustomDissolve>().Undissolve();
        }

        if (GUI.Button(new Rect(10, 50, 150, 30), "Teleport Sphere"))
        {
            target = Sphere;
            distance = 15;
            Sphere.GetComponent<CustomDissolve>().Dissolve();
        }
        if (GUI.Button(new Rect(200, 50, 150, 30), "Reverse Sphere"))
        {
            target = Sphere;
            distance = 15;
            Sphere.GetComponent<CustomDissolve>().Undissolve();
        }

        if (GUI.Button(new Rect(10, 90, 150, 30), "Corrosion on Monument"))
        {
            target = Monument;
            distance = 20;
            Monument.GetComponent<CustomDissolve>().Dissolve();
        }
        if (GUI.Button(new Rect(200, 90, 150, 30), "Reverse Monument"))
        {
            target = Monument;
            distance = 20;
            Monument.GetComponent<CustomDissolve>().Undissolve();
        }

        rotate = GUI.Toggle(new Rect(10, 130, 200, 30), rotate, "Rotate camera");
    }

    void Start()
    {
        //Get Ethan event, just for light on fire!
        Ethan.GetComponent<CustomDissolve>().CallBackFunction = EventManager;

        point = target.transform.position;//get target's coords
        transform.LookAt(point);//makes the camera look to it
    }

    void EventManager(CustomDissolve.EventInfo eventInfo)
    {
        if (eventInfo.messageInfo == CustomDissolve.EventType.StartDissolve)
        {
            EthanFires.SetActive(true);
        }

        if (eventInfo.messageInfo == CustomDissolve.EventType.EndDissolve)
        {
            EthanFires.SetActive(false);
        }
    }

    void Update()
    {
        Vector3 destPos = target.transform.position - transform.forward * distance;
        transform.position = Vector3.Lerp(transform.position, destPos, Time.deltaTime * 20f);

        if (!rotate)
            return;
        point = target.transform.position;
        transform.RotateAround(point, new Vector3(0.0f, 1.0f, 0.0f), 20 * Time.deltaTime * speedMod);
    }
}
