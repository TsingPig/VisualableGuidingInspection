using System;
using System.Collections.Generic;

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
