using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Banner : MonoBehaviour
{
    private SourceDB srcDB;
    private RawImage rawImage;
    public eFaction faction
    {
        set => ChangeFaction(value);
        get => _faction;
    }
    private eFaction _faction = eFaction.None;

    // Start is called before the first frame update
    void ChangeFaction(eFaction faction)
    {
        _faction = faction;
        switch (faction)
        {
            case eFaction.GlobalRisk:
                {
                    rawImage.texture = srcDB.globalRisk.GetComponent<SourceDB_Faction>().tex2D_banner;
                    break;
                }
            case eFaction.BlackList:
                {
                    rawImage.texture = srcDB.blackList.GetComponent<SourceDB_Faction>().tex2D_banner;
                    break;
                }
            case eFaction.NewHorizon:
                {
                    rawImage.texture = srcDB.newHorizon.GetComponent<SourceDB_Faction>().tex2D_banner;
                    break;
                }
            default :
                {
                    rawImage.texture = null;
                    break;
                }
        }
    }

    // Update is called once per frame
    void Start()
    {
        rawImage = GetComponent<RawImage>();
        srcDB = GameObject.Find("SourceDB").GetComponent<SourceDB>();
    }
}
