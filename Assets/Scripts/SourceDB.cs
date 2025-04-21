using System.Collections;
using UnityEngine;


[System.Serializable]
public class SourceDB : MonoBehaviour
{
    public static SourceDB instance = null;

    public SourceDB_Faction globalRisk;
    public SourceDB_Faction blackList;
    public SourceDB_Faction newHorizon;

    // Start is called before the first frame update
    void Start()
    {
        instance ??= this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}