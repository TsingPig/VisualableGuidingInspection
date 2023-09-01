using System.Collections.Generic;
using System.Linq;
using TsingPigSDK;
using UnityEngine.AI;

public class Inspection
{
    private List<InspectionInfo> _inspectionInfos;

    private bool[] _visited;

    private bool[,] _matrix;
    private int Len => _inspectionInfos.Count;

    private InspectionInfo _curInspectionInfo;
    public InspectionInfo CurInspectionInfo { get => _curInspectionInfo; set => _curInspectionInfo = value; }

    /// <summary>
    /// 根据检查项目ID获得对应列表中的索引。
    /// </summary>
    /// <param name="inspectionID">检查项目ID</param>
    /// <returns></returns>
    private int GetIndex(string inspectionID)
    {
        int idx = _inspectionInfos.FindIndex(info => info.inspectionID == inspectionID);
        if (idx == -1)
        {
            Log.Error($"{inspectionID} 不存在");
            return 0;
        }
        return idx;
    }

    /// <summary>
    /// 根据要前往的设备反向匹配检查项目。
    /// </summary>
    /// <param name="indexs">当前可以检查的项目ID</param>
    /// <param name="instrument">当前最优吸引设备</param>
    /// <returns>设备中匹配到的应检项目</returns>
    private int GetIndex(List<int> indexs, Instrument instrument)
    {
        InstrumentInfo info = instrument.InstrumentInfo;
        //Log.Info($"{info.InspectionIDs.Count}");
        foreach (var idx in indexs)
        {
            foreach (var idItem in instrument.InstrumentInfo.InspectionIDs)
            {
                if (_inspectionInfos[idx].inspectionID == idItem.inspectionID)
                {
                    Log.Info($"当前要前往{instrument.InstrumentInfo.instrumentName} 检查 {_inspectionInfos[idx].inspectionName}");
                    return idx;
                }
            }
        }
        Log.Error($" {instrument.InstrumentInfo.instrumentName} 匹配失败");
        return -1;
    }

    /// <summary>
    /// 返回入度为0的所有节点的索引。
    /// </summary>
    private List<int> GetIndexs => Enumerable.Range(0, Len).
        Where(j => Enumerable.Range(0, Len).
        All(i => !_matrix[i, j] && !_visited[j])).
        ToList();

    private void LogMatrix()
    {
        //for (int i = 0; i < Len; i++)
        //{
        //    for (int j = 0; j < Len; j++)
        //    {
        //        if (_matrix[i, j])
        //        {
        //         Log.Info($"{_inspectionInfos[i].inspectionName}({_inspectionInfos[i].instrumentID})-->{_inspectionInfos[j].inspectionName}({_inspectionInfos[j].instrumentID})");
        //        }
        //    }
        //}
        foreach (var i in GetIndexs)
        {
            Log.Info($"{_inspectionInfos[i].inspectionName}({_inspectionInfos[i].instrumentID})");
        }
    }
    private void Init()
    {
        _inspectionInfos = InspectionManager.Instance.InspectionInfos;
        _visited = new bool[Len];
        _matrix = new bool[Len, Len];
        for (int i = 0; i < Len; i++)
        {
            _visited[i] = false;
            for (int j = 0; j < Len; j++)
            {
                _matrix[i, j] = false;
            }
        }
        for (int v = 0; v < Len; v++)
        {
            var preInspectionIDs = _inspectionInfos[v].preInspectionIDs;
            foreach (var preInspectionID in preInspectionIDs)
            {
                int u = GetIndex(preInspectionID);
                _matrix[u, v] = true;
            }
        }
        //LogMatrix();
    }
    public Inspection()
    {
        Init();
    }

    /// <summary>
    ///  根据智能体和检查表，寻找最优检查设备、匹配项目。
    /// </summary>
    /// <param name="agent">病人智能体</param>
    /// <returns>下一个前往的设备</returns>
    public Instrument GetNext(NavMeshAgent agent)
    {
        List<int> indexs = GetIndexs;
        if (indexs.Count == 0)
        {
            Log.Info("所有检查已完成");
            return null;
        }
        int curInspectionIdx = 0;
        List<InspectionInfo> infos = new List<InspectionInfo>();
        foreach (var idx in indexs)
        {
            infos.Add(_inspectionInfos[idx]);
        }
        Instrument nextInstrument = InstrumentManager.Instance.Recommend(infos, agent);

        curInspectionIdx = GetIndex(indexs, nextInstrument);
        _visited[curInspectionIdx] = true;
        for (int j = 0; j < Len; j++)
            _matrix[curInspectionIdx, j] = false;
        _curInspectionInfo = _inspectionInfos[curInspectionIdx];
        Log.Info($"当前选择{_curInspectionInfo.inspectionName}");
        //LogMatrix();
        return nextInstrument;
    }
}

