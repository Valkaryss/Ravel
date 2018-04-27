using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monitor : MonoBehaviour {

    [Tooltip("An empty RenderTexture that will be filled by this script.")]
    public RenderTexture dummy;
    public Texture2D background;
    public Texture2D tags;
    public Texture2D text1;
    public Texture2D text2;
    public Texture2D text3;
    public Texture2D text4;
    public Texture2D text5;

    // Use this for initialization
    void Start () {
        Graphics.Blit(background, dummy);
        Graphics.Blit(tags, dummy);
	}
	
	// Update is called once per frame
	void Update () {

	    if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            Graphics.Blit(text1, dummy);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            Graphics.Blit(text2, dummy);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            Graphics.Blit(text3, dummy);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            Graphics.Blit(text4, dummy);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            Graphics.Blit(text5, dummy);
        }
    }
}
