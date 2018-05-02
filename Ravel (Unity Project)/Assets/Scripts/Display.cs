using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Display : MonoBehaviour {

    public Runner runner;
    public Material blending;
    public RenderTexture target;

    public Texture2D background;
    public Texture2D tags;
    public Texture2D text1;
    public Texture2D text2;
    public Texture2D text3;
    public Texture2D text4;
    public Texture2D text5;

    // Use this for initialization
    void Start()
    {
        blending = new Material(Shader.Find("Alpha Effect/Alpha Blending"));
        Graphics.Blit(background, target);
        restart();
    }

    void restart()
    {
        apply(tags);
        StartCoroutine(applyText1());
        StartCoroutine(applyText2());
        StartCoroutine(applyText3());
        StartCoroutine(applyText4());
        StartCoroutine(applyText5());
        StartCoroutine(listenToRestart());
    }

    IEnumerator listenToRestart()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.R));
        restart();
    }

    void apply(Texture2D t)
    {
        print("Applying");
        blending.SetTexture("_BaseTex", target);
        blending.SetTexture("_ApplyTex", tags);
        Graphics.Blit(tags, target, blending);
    }

    IEnumerator applyText1()
    {
        yield return new WaitUntil(() => runner.drawText1);
        apply(text1);
    }

    IEnumerator applyText2()
    {
        yield return new WaitUntil(() => runner.drawText2);
        apply(text2);
    }
    IEnumerator applyText3()
    {
        yield return new WaitUntil(() => runner.drawText3);
        apply(text3);
    }
    IEnumerator applyText4()
    {
        yield return new WaitUntil(() => runner.drawText4);
        apply(text4);
    }
    IEnumerator applyText5()
    {
        yield return new WaitUntil(() => runner.drawText5);
        apply(text5);
    }
}
