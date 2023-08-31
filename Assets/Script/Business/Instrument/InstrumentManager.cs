using System.Collections.Generic;
using TsingPigSDK;
public class InstrumentManager : Singleton<InstrumentManager>
{
    private Instrument_SO _instrumentData;

    private List<InstrumentInfo> _instrumentInfos;
    public List<InstrumentInfo> InstrumentInfos { get => _instrumentInfos; set => _instrumentInfos = value; }

    public InstrumentInfo GetInfo(string instrumentID)
    {
        return _instrumentInfos.Find(info => info.instrumentID == instrumentID);
    }

    private new void Awake()
    {
        base.Awake();
        Init();
    }
    private void Init()
    {
        _instrumentData = Res.Load<Instrument_SO>(Str_Def.INSTRUMENT_DATA_PATH);
        _instrumentInfos = _instrumentData.instrumentInfos;

    }
}
