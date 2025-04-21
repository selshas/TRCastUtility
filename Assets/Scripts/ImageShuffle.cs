using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(UnityEngine.UI.RawImage))]
public class ImageShuffle : MonoBehaviour
{
    public List<Texture2D> images = new List<Texture2D>();

    private UnityEngine.UI.RawImage rawImg;

    // Start is called before the first frame update
    void Awake()
    {
        rawImg = GetComponent<UnityEngine.UI.RawImage>();
        Shuffle();
    }

    private void OnEnable()
    {
        Shuffle();
    }

    public void Shuffle()
    {
        int i = Random.Range(0, images.Count);
        Debug.Log($"{i}/{images.Count}");
        Select(i);
    }

    public void Select(int imgNo)
    {
        rawImg.texture = images[imgNo];
    }

    private void Update()
    {
        if (Keyboard.current.f5Key.ReadValue() == 1.0f)
        {
            Shuffle();
        }
    }
}
