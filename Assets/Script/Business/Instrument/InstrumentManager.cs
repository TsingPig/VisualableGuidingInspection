using System.Collections.Generic;
using System.Linq;
using TsingPigSDK;
using UnityEngine.AI;

public class InstrumentManager : Singleton<InstrumentManager>
{
    private const float w1 = 0.2f;

    private const float w2 = 0.8f;

    private List<InstrumentInfo> _instrumentInfos;

    private Dictionary<string, List<Instrument>> _dicInstruments = new Dictionary<string, List<Instrument>>();

    /// <summary>
    /// 设备初始化：双向注册（Info绑定到设备、设备绑定到管理器）
    /// </summary>
    /// <param name="instrument">设备脚本</param>
    /// <returns></returns>
    public InstrumentInfo GetInfo(Instrument instrument)
    {
        string instrumentID = instrument.name;
        instrumentID = instrumentID.Substring(0, instrumentID.IndexOf(" ") == -1 ? instrumentID.Length : instrumentID.IndexOf(" "));
        InstrumentInfo info = _instrumentInfos.Find(info => info.instrumentID == instrumentID);

        if (_dicInstruments.ContainsKey(info.instrumentID))
        {
            _dicInstruments[info.instrumentID].Add(instrument);
        }
        else
        {
            List<Instrument> lstInstrument = new List<Instrument>() { instrument };
            _dicInstruments.Add(info.instrumentID, lstInstrument);
        }
        Log.Info($"ID : {instrumentID}注册{info.instrumentName}");
        return info;
    }

    /// <summary>
    /// 推荐算法：根据治疗信息，推断处最合适的设备以及治疗项目。
    /// </summary>
    /// <param name="inspectionInfos">治疗信息</param>
    /// <param name="agent">当前病人的智能体</param>
    /// <returns></returns>
    public Instrument Recommend(List<InspectionInfo> inspectionInfos, NavMeshAgent agent)
    {
        List<InstrumentInfo> instrumentInfos = new List<InstrumentInfo>();
        foreach (var inspcInfo in inspectionInfos)
        {
            instrumentInfos.Add(GetInfo(inspcInfo.instrumentID));
        }
        List<Instrument> instruments = new List<Instrument>();

        foreach (var instrInfo in instrumentInfos)
        {
            List<Instrument> instrs = _dicInstruments[instrInfo.instrumentID];
            instruments.AddRange(instrs);
        }
        instruments.OrderBy(instr => GetAttraction(instr, agent));
        Log.Info($"推荐 {instruments[0].InstrumentInfo.instrumentName}");
        return instruments[0];
    }

    private new void Awake()
    {
        base.Awake();
        Init();
    }
    private void Init()
    {
        Instrument_SO instrumentData = Res.Load<Instrument_SO>(Str_Def.INSTRUMENT_DATA_PATH);
        _instrumentInfos = instrumentData.instrumentInfos;

    }

    /// <summary>
    /// 根据设备ID，获得管理器中Info
    /// </summary>
    /// <param name="instrumentID"></param>
    /// <returns></returns>
    private InstrumentInfo GetInfo(string instrumentID)
    {
        return _instrumentInfos.Find(info => info.instrumentID == instrumentID);
    }

    /// <summary>
    /// 计算某台设备对每个病人的吸引程度
    /// </summary>
    /// <param name="instrument">设备脚本</param>
    /// <param name="agent">病人智能体</param>
    /// <returns></returns>
    private float GetAttraction(Instrument instrument, NavMeshAgent agent)
    {
        float waitingTime = instrument.WaitingTime;
        float pathLength = MyExtensions.CalculatePathLength(agent.transform, instrument.transform);
        float pathTime = pathLength / agent.velocity.magnitude;
        return w1 * waitingTime + w2 * pathTime;
    }

}
