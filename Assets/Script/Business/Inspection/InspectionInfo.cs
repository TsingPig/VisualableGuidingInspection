using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inspection", menuName = "Data/Inspection")]
public class Inspection_SO : ScriptableObject
{
    public List<InspectionInfo> inspectionInfos;
}


[Serializable]
public class InspectionInfo 
{
    public string inspectionID;
    public string inspectionName;
    public bool emptyStomach;
    public string instrumentID;
    public List<string> preInspectionIDs;
}
