using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Instrument", menuName = "Data/Instrument")]
public class Instrument_SO : ScriptableObject
{
    public List<InstrumentInfo> instrumentInfos;
}


[Serializable]
public class InstrumentInfo
{

    [Serializable]
    public struct InspectionID
    {
        public string inspectionID;
        public float periodCountPercent;
    }

    public string instrumentID;
    public string instrumentName;
    public int instrumentCount;

    public List<InspectionID> inspectionIDs;


}
