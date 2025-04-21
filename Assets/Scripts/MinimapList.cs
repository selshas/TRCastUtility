using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapList : MonoBehaviour
{
    UnityEngine.UI.RawImage rawImg;
    public List<Texture2D> list_tex_minimaps = new List<Texture2D> ();

    void Awake()
    {
        rawImg = GetComponent<UnityEngine.UI.RawImage> ();
    }
    public void ChangeMap(int i)
    {
        Debug.Log(i);
        rawImg.texture = list_tex_minimaps[i];
    }

}
