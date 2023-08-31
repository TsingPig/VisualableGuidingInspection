using TsingPigSDK;

public class InspectionManager : Singleton<InspectionManager>
{
    private Inspection_SO _inspectionData;
    public Inspection_SO InspectionData => _inspectionData;

    private void Init()
    {
        _inspectionData = Res.Load<Inspection_SO>(Str_Def.INSPECTION_DATA_PATH);
    }

    private new void Awake()
    {
        base.Awake();
        Init();

    }
}

