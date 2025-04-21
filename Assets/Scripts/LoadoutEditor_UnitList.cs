using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class LoadoutEditor_UnitList : MonoBehaviour
{
    public LoadoutFrame loadoutFrame = null;
    public int slotNo = 0;

    List<GameObject> list_buttonPool = new List<GameObject>();
    List<Roster> list_unitDatas = new List<Roster> ();


    public void Open(int _slotNo)
    {
        Close();
        list_unitDatas = SourceDB.instance.globalRisk.slots[_slotNo].units;
        slotNo = _slotNo;

        // Create unit list
        for (int i = 0; i < list_unitDatas.Count; i++)
        {
            Roster unitData = list_unitDatas[i];

            GameObject obj;
            Button btn;
            RawImage rawImg;

            if (list_buttonPool.Count <= i)
            {
                list_buttonPool.Add(new GameObject());
                obj = list_buttonPool[i];
                obj.transform.SetParent(transform);
                btn = obj.AddComponent<Button>();
                rawImg = obj.AddComponent<RawImage>();
            }
            else
            {
                obj = list_buttonPool[i];
                obj.SetActive(true);

                btn = obj.GetComponent<Button>();
                rawImg = obj.GetComponent<RawImage>();
            }

            obj.transform.position = transform.position;
            btn.onClick.RemoveAllListeners();

            int index = i;
            btn.onClick.AddListener(() => Select(index));
            rawImg.texture = unitData.tex_icon;
        }
    }

    public void Select(int unitIndex)
    {
        Debug.Log($"Sizeof list_unitDatas: {list_unitDatas.Count}");
        Debug.Log($"{unitIndex} Selected");
        Roster unitData = list_unitDatas[unitIndex];
        loadoutFrame.SelectRoster(slotNo, unitIndex);

        Close();
    }

    public void Close()
    {
        for (int i = 0; i < list_buttonPool.Count; i++)
        {
            list_buttonPool[i].SetActive(false);
        }
    }
}
