using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomDissolve : MonoBehaviour {

    public float dissolveDuration = 5f;
    public Texture2D noiseTexture;
    public Color dissolveColor = Color.white;
    [Range(0, 10)]
    public float dissolveThickness = 3f;
    public float tilingX = 1;
    public float tilingY = 1;

    private List<Shader> originalObjetsShader;
    private Shader dissolveShader = null;
    private bool dissolve = false;
    private float t = 0;
    private float startValue, endValue;
    private Component[] renderers;
    

    public enum EventType
    {
        StartDissolve,
        EndDissolve,
        StartUndissolve,
        EndUnsidissolve
    }

    public class EventInfo
    {
        public EventType messageInfo;
        public CustomDissolve sender;
    }
    public delegate void CallbackEventHandler(EventInfo eventInfo);
    public CallbackEventHandler CallBackFunction;
    private EventType nextType;


    // Use this for initialization
    void Start () {
        dissolveShader = Shader.Find("3y3net/CustomDissolve");

        
        renderers = GetComponentsInChildren<Renderer>();
        originalObjetsShader = new List<Shader>();

        if (dissolveShader == null)
            Debug.Log("No dissolve shader found!");
        ReplaceShaders();
    }

    void Update()
    {
        if (!dissolve)
            return;
        foreach (Renderer singleRenderer in renderers)
            foreach (Material singleMaterial in singleRenderer.materials)
            {
                singleMaterial.SetFloat("_DissolveAmount", Mathf.Lerp(startValue, endValue, t));
            }

        if (t < 1)
        {
            t += Time.deltaTime / dissolveDuration;
        }
        else
        {
            dissolve = false;
            if (CallBackFunction != null)
            {
                EventInfo myEvent = new EventInfo();
                myEvent.sender = this;
                myEvent.messageInfo = nextType;
                CallBackFunction(myEvent);
            }
        }
    }

    public void Dissolve()
    {
        t = 0f;
        startValue = 1f;
        endValue = 0f;
        dissolve = true;
        if (CallBackFunction != null)
        {
            EventInfo myEvent = new EventInfo();
            myEvent.sender = this;
            myEvent.messageInfo = EventType.StartDissolve;
            CallBackFunction(myEvent);
        }
        nextType = EventType.EndDissolve;
    }

    public void Undissolve()
    {
        t = 0f;
        startValue = 0;
        endValue = 1f;
        dissolve = true;
        if (CallBackFunction != null)
        {
            EventInfo myEvent = new EventInfo();
            myEvent.sender = this;
            myEvent.messageInfo = EventType.StartUndissolve;
            CallBackFunction(myEvent);
        }
        nextType = EventType.EndUnsidissolve;
    }
	
	public void ReplaceShaders()
    {
        foreach (Renderer singleRenderer in renderers)
            foreach (Material singleMaterial in singleRenderer.materials)
            {
                originalObjetsShader.Add(singleMaterial.shader);
                singleMaterial.shader = dissolveShader;
                singleMaterial.SetTexture("_Noise", noiseTexture);
                singleMaterial.SetColor("_DissolveColor", dissolveColor);
                singleMaterial.SetFloat("_GlowThickness", dissolveThickness);
                singleMaterial.SetFloat("_GlowThickness", dissolveThickness);
                singleMaterial.SetTextureScale("_Noise", new Vector2(tilingX, tilingY));
            }
    }

    public void RestoreShaders()
    {
        int i = 0;
        Component[] renderers;
        renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer singleRenderer in renderers)
            foreach (Material singleMaterial in singleRenderer.materials)
            {
                singleMaterial.shader = originalObjetsShader[i++];
            }
    }
}
