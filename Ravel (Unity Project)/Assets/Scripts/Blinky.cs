using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Target {
    public float time;
    public bool isOff;
    public List<int> triangles;
    public int[] indices;

    public Target(){
         time = 0;
         isOff = true;
         triangles = new List<int>();
         indices = new int[30];
    }
}


public class Blinky : MonoBehaviour {

    [Tooltip("Material index for lights off material")]
    public int lightsOff;
    [Tooltip("Material index for lights on material")]
    public int lightsOn;

    [Tooltip("Probability for a light to turn on if it is off, per second."
           + " Range[0,1]")]
    public float p;

    [Tooltip("Time that a light remains on")]
    public float onTime;

    List<Target> targets;
    Mesh mesh;

    // First the algorithm assumes that everything is supposed to start off.
    // Then, it computes the targets, assuming that if faces touch eachother
    // then they're meant to turn on and off in unison.
	void Start () {
        mesh = GetComponent<MeshFilter>().mesh;
        Debug.Log("Submeshes: " + mesh.subMeshCount);

        turnAllLightsOff(mesh);
        // Debug.Break();
		targets = getTargets(mesh);
	}
	
    // Remove everything from the "on" lists
    void turnAllLightsOff(Mesh m){
        List<int> onTris = new List<int>();
        List<int> offTris = new List<int>();

        m.GetTriangles(onTris, lightsOn);
        m.GetTriangles(offTris, lightsOff);

        onTris.ForEach(el => {offTris.Add(el);});

        m.SetTriangles(offTris, lightsOff);
        m.SetTriangles(new List<int>(), lightsOn);
    }

    // Remove everything from the "on" lists
    void turnAllLightsOn(Mesh m){
        List<int> onTris = new List<int>();
        List<int> offTris = new List<int>();

        m.GetTriangles(onTris, lightsOn);
        m.GetTriangles(offTris, lightsOff);

        offTris.ForEach(el => {onTris.Add(el);});

        m.SetTriangles(new List<int>(), lightsOff);
        m.SetTriangles(onTris, lightsOn);
    }
    // Get a list of all of the unique face-sections we care about
    List<Target> getTargets(Mesh m){
        List<int> tris = new List<int>();
        List<Target> result = new List<Target>();
        m.GetTriangles(tris, lightsOff);
        int count = 0;
        Target t = new Target();
        tris.ForEach(el => {
                count += 1;
                if (count <= 30){
                    t.triangles.Add(el);
                    t.indices[count-1] = t.triangles.Count;
                }
                else{
                    result.Add(t);
                    count = 1;
                    t = new Target();
                    t.triangles.Add(el);
                    t.indices[count-1] = t.triangles.Count;
                }
            });
        result.Add(t);
        return result;
    }

    // Turn on the lights that are off and win a coin toss, turn off the 
    // lights that have been on too long, have yourself a grand old time.
	void Update () {
		targets.ForEach(target => {
                if (target.isOff && coinFlip(p * Time.deltaTime)){
                    turnOn(mesh, target);
                }
                else if (!target.isOff){
                    target.time += Time.deltaTime;
                    if (target.time > onTime) {
                        turnOff(mesh, target);
                    }
                }
            }
	   );
    }

    // Turn the selected target off
    void turnOff(Mesh m, Target t){
        List<int> onTris = new List<int>();
        List<int> offTris = new List<int>();
        m.GetTriangles(onTris,  lightsOn);
        m.GetTriangles(offTris, lightsOff);

        // Remove the triangles from the submesh of "on"
        t.triangles.ForEach(el => onTris.Remove(el));

        // Add the triangles to the submesh of the "off" material
        int i = 0;
        t.triangles.ForEach(el => { offTris.Add(el); 
                                    t.indices[i] = offTris.Count; 
                                    i ++;});

        // Change the marker
        t.isOff = true;

        // Update the mesh
        m.SetTriangles(onTris, lightsOn);
        m.SetTriangles(offTris, lightsOff);
    }

    // Turn the selected target on and starts the timer to turn it off again
    void turnOn(Mesh m, Target t){
        List<int> onTris = new List<int>();
        List<int> offTris = new List<int>();
        m.GetTriangles(onTris,  lightsOn);
        m.GetTriangles(offTris, lightsOff);

        // Remove the triangles from the submesh of "off"
        t.triangles.ForEach(el => offTris.Remove(el));

        // Add the triangles to the submesh of the "on" material
        int i = 0;
        t.triangles.ForEach(el => { onTris.Add(el); 
                                    t.indices[i] = onTris.Count; 
                                    i ++;});

        // Change the marker and time the light
        t.time = 0;
        t.isOff = false;

        // Update the mesh
        m.SetTriangles(onTris, lightsOn);
        m.SetTriangles(offTris, lightsOff);
    }

    // A fair coin flip with probability p
    bool coinFlip(float p){
        return (Random.value < p);
    }
}
