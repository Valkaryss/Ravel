using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class RyanNonLinear : MonoBehaviour {

    public int rightLackeyNum;
    public int leftLackeyNum;

    public float focalDist;
    public float rotation;

    Vector3 focalPoint;

    public int barSize;
    public int blendSize;

    public Camera original;

    int width;
    int height;

    Material blending;

    Camera[] rightLackeys;
    RenderTexture[] rightTargets;
    Texture2D[] rightMasks;

    Camera[] leftLackeys;
    RenderTexture[] leftTargets;
    Texture2D[] leftMasks;

	// Use this for initialization
	void Start () {
        width = Screen.width;
        height = Screen.height;

        blending = new Material(Shader.Find("Alpha Mask/Blending"));
        focalPoint = transform.position + transform.forward * focalDist;

		rightMasks = new Texture2D[rightLackeyNum];
        rightTargets = new RenderTexture[rightLackeyNum];
        rightLackeys = new Camera[rightLackeyNum];

        leftMasks = new Texture2D[leftLackeyNum];
        leftTargets = new RenderTexture[leftLackeyNum];
        leftLackeys = new Camera[leftLackeyNum];

        int rWidth = blendSize;

        // Initialize the right side cameras and rendertextures. 
        for (int i = 0; i < rightLackeyNum; i ++){
            rightMasks[i] = getRightMask(width/2 + barSize * (i + 1), rWidth);
            rightTargets[i] = new RenderTexture(width, height, 24);
            rightLackeys[i] = (Camera) Object.Instantiate(original);
            rightLackeys[i].targetTexture = rightTargets[i];
            rightLackeys[i].transform.RotateAround(focalPoint, Vector3.up, 
                                                -rotation * (i+1));
            rightLackeys[i].depth = i;
            rightLackeys[i].transform.SetParent(transform);
        }

        // Initialize the left side
        for (int i = 0; i < leftLackeyNum; i ++){
            leftMasks[i] = getLeftMask(width/2 - barSize * (i + 1), rWidth);
            leftTargets[i] = new RenderTexture(width, height, 24);
            leftLackeys[i] = (Camera) Object.Instantiate(original);
            leftLackeys[i].targetTexture = leftTargets[i];
            leftLackeys[i].transform.RotateAround(focalPoint, Vector3.up, 
                                                rotation * (i+1));
            leftLackeys[i].depth = i + rightLackeyNum;
            leftLackeys[i].transform.SetParent(transform);
        }
        Debug.Log("Init complete");
	}


  // Get a mask for the right side of the screen, with the ramp on the
  // left side.
  Texture2D getRightMask(int start, int rampWidth){
        Color[] maskVal = new Color[height * width];

        for (int i = 0; i < height; i ++){
            for (int j = 0; j < width; j ++){
                if (j > start + rampWidth){ 
                    maskVal[i * width + j] = new Color(0,0,0);
                }
                else if (j > start){
                    float through = 1 - (j - start)/(float)rampWidth;
                    float val = Mathf.SmoothStep(0,1, through);
                    maskVal[i * width + j] = new Color(val,val,val);
                }
                else { 
                    maskVal[i * width + j] = new Color(1,1,1);
                }
            }
        }
        Texture2D result = new Texture2D(width,height,
                               TextureFormat.RGBA32, false);
        result.SetPixels(maskVal);
        result.Apply();
        return result;
    }

    // Get a mask for the left side of the screen, with the ramp on the 
    // right side.
    Texture2D getLeftMask(int start, int rampWidth){
        Color[] maskVal = new Color[height * width];

        for (int i = 0; i < height; i ++){
            for (int j = 0; j < width; j ++){
                if (j < start - rampWidth){ 
                    maskVal[i * width + j] = new Color(0,0,0);
                }
                else if (j < start){
                    float through = 1 - (start - j)/(float)rampWidth;
                    float val = Mathf.SmoothStep(0,1, through);
                    maskVal[i * width + j] = new Color(val,val,val);
                }
                else { 
                    maskVal[i * width + j] = new Color(1,1,1);
                }
            }
        }
        Texture2D result = new Texture2D(width,height,
                               TextureFormat.RGBA32, false);
        result.SetPixels(maskVal);
        result.Apply();
        return result;
    }


    // Happens before this camera renders. I was hoping this would avoid the
    // GFX pipeline stall, but it doesn't seem to.
    void OnPreRender(){
        for (int i = 0; i < rightLackeyNum; i ++){
            rightLackeys[i].Render();
        }
        for (int j = 0; j < leftLackeyNum; j ++){
            leftLackeys[j].Render();
        }
    }

    // Happens during the render step of the pipeline. I'm ripping control
    // away from the system to do my own blending things.
    void OnRenderImage(RenderTexture src, RenderTexture dest) {
        RenderTexture mixer = new RenderTexture(width, height, 24);

        // Mix all of the right-side cameras.
        for (int i = 0; i < rightLackeyNum; i ++){
            blending.SetTexture("_BaseTex", src);
            blending.SetTexture("_MaskTex", rightMasks[i]);
            blending.SetTexture("_ApplyTex", rightTargets[i]);
            Graphics.Blit(mixer, src, blending);
        }
        mixer = new RenderTexture(width, height, 24);
        // Mix all of the left-side cameras. 
        for (int i = 0; i < leftLackeyNum; i++)
        {
          blending.SetTexture("_BaseTex", src);
          blending.SetTexture("_MaskTex", leftMasks[i]);
          blending.SetTexture("_ApplyTex", leftTargets[i]);
          Graphics.Blit(mixer, src, blending);
        }

    // Send all the stuff I just built up onto the screen.
    Graphics.Blit(src, dest);
  }
}