using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public string name;

    public eFaction lastPlayedFaction = eFaction.GlobalRisk;

    Dictionary<eFaction, int[]> dict_loadouts = new Dictionary<eFaction, int[]>();
    int[] loadout = new int[16];

    public PlayerData()
    {
        dict_loadouts[eFaction.GlobalRisk] = new int[16];
        dict_loadouts[eFaction.BlackList]  = new int[16];
        dict_loadouts[eFaction.NewHorizon] = new int[16];
    }

    public int[] GetLoadoutForFaction(eFaction faction)
    {
        return dict_loadouts[faction];
    }
}
