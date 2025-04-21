using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutFrame : MonoBehaviour
{
    PlayerData playerData = null;

    RawImage rawImg_loadoutIcon_cmdr = null;
    RawImage[] rawImgs_loadoutIcon_unit = new RawImage[7];

    public eFaction faction {
        get => _faction;
        set => SelectFaction(value);
    }
    private eFaction _faction = eFaction.None;

    int[] loadout = new int[16];

    RawImage rawImage_front;
    public void LoadPlayerData(string playerName)
    {
        if (false)
        {
            loadout = playerData.GetLoadoutForFaction(playerData.lastPlayedFaction);
        }
        else
        {
            playerData = new PlayerData();
            loadout = new int[] { 0, };
        }


    }

    void SelectFaction(eFaction faction)
    {
        _faction = faction;//
        rawImage_front.texture = null;//
    }

    // Start is called before the first frame update
    void Start()
    {
        rawImg_loadoutIcon_cmdr = transform.Find("CommanderLoadout").GetComponent<RawImage>();
        rawImage_front = transform.Find("Front").GetComponent<RawImage>();
        Transform trans_unitLoadout = transform.Find("UnitLoadout");


        for (int i = 0; i < 7; i++)
        {
            rawImgs_loadoutIcon_unit[i] = trans_unitLoadout.GetChild(i).GetComponent<RawImage>();
        }
    }

    // Update is called once per frame
    void UpdateLoadout()
    {
        rawImg_loadoutIcon_cmdr.texture = SourceDB.instance.globalRisk.slots[0].units[0].tex_icon;

        for (int i = 0; i < 7; i++)
        {
            rawImgs_loadoutIcon_unit[i].texture = SourceDB.instance.globalRisk.slots[i+1].units[loadout[i]].tex_icon;
        }
    }

    public void SelectRoster(int slotNo, int itemNo)
    {
        loadout[slotNo] = itemNo;
        UpdateLoadout();
    }
}
