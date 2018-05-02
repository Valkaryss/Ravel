using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Display : MonoBehaviour {

    public Material blending;
    public RenderTexture target;
    public Texture2D background;
    public Texture2D layerOn;

	// Use this for initialization
	void Start () {
        blending = new Material(Shader.Find("Alpha Effect/Alpha Blending"));
        Graphics.Blit(background, target);
	}
	
	// Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.Return)){
            blending.SetTexture("_BaseTex", target);
            blending.SetTexture("_ApplyTex", layerOn);
            Graphics.Blit(layerOn, target, blending);
        }
	}
}
