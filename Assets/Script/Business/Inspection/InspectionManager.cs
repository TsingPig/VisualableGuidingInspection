using System.Collections.Generic;
using TsingPigSDK;
using UnityEngine;

public class InspectionManager : Singleton<InspectionManager>
{
    public Transform InspectionExit;

    private Inspection_SO _inspectionData;

    private List<InspectionInfo> _inspectionInfos;

    private int _curFinishedCount;

    public List<InspectionInfo> InspectionInfos => _inspectionInfos;

    public int CurFinishedInspectionsCount { get => _curFinishedCount; set => _curFinishedCount = value; }

    private void Init()
    {
        _inspectionData = Res.Load<Inspection_SO>(Str_Def.INSPECTION_DATA_PATH);
        _inspectionInfos = _inspectionData.inspectionInfos;
        InspectionExit = GameObject.Find("Exit").transform.GetChild(0);
    }

    private new void Awake()
    {
        base.Awake();
        Init();
    }

}

