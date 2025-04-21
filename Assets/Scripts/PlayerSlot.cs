using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSlot : MonoBehaviour
{
    private Banner banner;

    // Start is called before the first frame update
    void Start()
    {
        banner = GetComponent<Banner>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OpenFactionSlot()
    {

    }
    void SelectFaction()
    {
        banner.faction = eFaction.BlackList;
    }

    void OpenCmdrSlot()
    {

    }

    void SelectCmdr(int cmdrNo)
    {
        
    }

    void OpenUnitSlot()
    {

    }
    void SelectUnit()
    {

    }

    bool LoadRememberedPlayerPreset(string name)
    {


        return false;
    }
}
