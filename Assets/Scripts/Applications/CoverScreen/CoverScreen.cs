using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.RawImage))]
public class CoverScreen : UtilityAppBase
{
    public UnityEngine.UI.RawImage rawImage;

    public Material mat_multiplyOpacity;

    private RenderTexture renderTex;
    private RenderTexture renderTex_prev;
    public Texture2D tex_titleImage;

    public bool isCycling = true;
    public float cyclingTime = 2.0f;

    private Coroutine coroutine_imageSwapFading = null;

    public List<Texture2D> images = new List<Texture2D>();

    private int currentImageIndex = -1;

    // Start is called before the first frame update

    public void Shuffle()
    {
        Select(Random.Range(0, images.Count-2));
    }

    public void Select(int imgNo)
    {
        if (imgNo < 0)
            currentImageIndex = images.Count + imgNo;
        else
            currentImageIndex = imgNo;
    }

    // Start is called before the first frame update
    void Awake()
    {
        images.Add(tex_titleImage);
        images.Add(new Texture2D(1, 1, TextureFormat.RGBA32, false));
        images.Add(new Texture2D(1, 1, TextureFormat.RGBA32, false));

        images[images.Count - 1].SetPixel(0, 0, new Color(0, 0, 0, 1));
        images[images.Count - 1].Apply();
        images[images.Count - 2].SetPixel(0, 0, new Color(1, 1, 1, 1));
        images[images.Count - 2].Apply();

        renderTex = new RenderTexture(1920, 1080, 1, RenderTextureFormat.ARGB32, 0);
        renderTex_prev = new RenderTexture(1920, 1080, 1, RenderTextureFormat.ARGB32, 0);

        mat_multiplyOpacity.SetTexture("_DestTex", renderTex_prev);

        rawImage = GetComponent<UnityEngine.UI.RawImage>();
        rawImage.texture = renderTex;
        rawImage.color = new Color(1, 1, 1, 0);

        Shuffle();
    }

    public void ShowCyclicImages()
    {
        gameObject.SetActive(true);
        if (coroutine_imageSwapFading != null) StopCoroutine(coroutine_imageSwapFading);

        isCycling = true;
        Shuffle();

        Graphics.CopyTexture(renderTex, renderTex_prev);

        coroutine_imageSwapFading = StartCoroutine(Coroutine_FadeIn());
    }
    public void ShowBlackScreen()
    {
        gameObject.SetActive(true);
        if (coroutine_imageSwapFading != null) StopCoroutine(coroutine_imageSwapFading);

        isCycling = false;
        Select(-1);

        Graphics.CopyTexture(renderTex, renderTex_prev);

        coroutine_imageSwapFading = StartCoroutine(Coroutine_FadeIn());
    }
    public void ShowWhiteScreen()
    {
        gameObject.SetActive(true);
        if (coroutine_imageSwapFading != null) StopCoroutine(coroutine_imageSwapFading);

        isCycling = false;
        Select(-2);

        Graphics.CopyTexture(renderTex, renderTex_prev);

        coroutine_imageSwapFading = StartCoroutine(Coroutine_FadeIn());
    }
    public void ShowTitleScreen()
    {
        gameObject.SetActive(true);
        if (coroutine_imageSwapFading != null) StopCoroutine(coroutine_imageSwapFading);

        isCycling = false;
        Select(-3);

        Graphics.CopyTexture(renderTex, renderTex_prev);

        coroutine_imageSwapFading = StartCoroutine(Coroutine_FadeIn());
    }
    public void Disappear()
    {
        coroutine_imageSwapFading = StartCoroutine(Coroutine_FadeOut());
    }

    IEnumerator Coroutine_FadeIn()
    {
        while (true)
        {
            for (float alpha = 0; alpha <= 1.0f; alpha += Time.deltaTime)
            {
                mat_multiplyOpacity.SetFloat("_Opacity", alpha);
                if (rawImage.color.a < 1.0f) rawImage.color = new Color(1, 1, 1, alpha);
                Graphics.Blit(images[currentImageIndex], renderTex, mat_multiplyOpacity);
                yield return new WaitForEndOfFrame();
            }
            mat_multiplyOpacity.SetFloat("_Opacity", 1.0f);
            rawImage.color = new Color(1, 1, 1, 1);
            Graphics.Blit(images[currentImageIndex], renderTex, mat_multiplyOpacity);

            if (isCycling)
            {
                yield return new WaitForSecondsRealtime(cyclingTime);
                Graphics.CopyTexture(renderTex, renderTex_prev);
                Shuffle();
            }
            else break;
        }

        yield return null;
    }
    IEnumerator Coroutine_FadeOut()
    {
        if (coroutine_imageSwapFading != null) StopCoroutine(Coroutine_FadeIn());

        for (float alpha = 1.0f; alpha >= 0.0f; alpha -= Time.deltaTime)
        {
            rawImage.color = new Color(1, 1, 1, alpha);
            yield return new WaitForEndOfFrame();
        }
        rawImage.color = new Color(1, 1, 1, 0);

        gameObject.SetActive(false);
        yield return null;
    }
}
