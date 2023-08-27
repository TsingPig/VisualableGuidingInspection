using System;
using System.Collections.Generic;



[Serializable]
public class InspectionInfo
{
    public string inspectionID;
    public string inspectionName;
    public bool emptyStomach;
    public string instrumentID;
    public List<string> preInspectionIDs;
}
