using System.Collections.Generic;
using TsingPigSDK;

public class InspectionManager : Singleton<InspectionManager>
{
    private Inspection_SO _inspectionData;

    private List<InspectionInfo> _inspectionInfos;
    public List<InspectionInfo> InspectionInfos => _inspectionInfos;

    private void Init()
    {
        _inspectionData = Res.Load<Inspection_SO>(Str_Def.INSPECTION_DATA_PATH);
        _inspectionInfos = _inspectionData.inspectionInfos;
    }

    private new void Awake()
    {
        base.Awake();
        Init();

    }
}

