using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSwitcher : MonoBehaviour
{
    public List<GameObject> list_items = new List<GameObject> ();

    int current = -1;

    public void Switch(int index)
    { 
        for (int i = 0; i < list_items.Count; i++)
        {
            list_items[i].SetActive(false);
        }

        if (current != index)
        {
            list_items[index].SetActive(true);
            current = index;
        }
        else
            current = -1;
    }
    public void AllOff()
    {
        for (int i = 0; i < list_items.Count; i++)
        {
            list_items[i].SetActive(false);
        }
        current = -1;
    }
}
