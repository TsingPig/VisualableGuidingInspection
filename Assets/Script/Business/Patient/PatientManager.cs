using System;
using System.Collections;
using System.Collections.Generic;
using TsingPigSDK;
using UnityEngine;

public class PatientManager : Singleton<PatientManager>
{
    private int _totalPatientCount = 120;

    private float _genDurationPeriodPercent = 0.1f;

    public Action AllPatientFinish_Event;
    public int TotalPatientCount  => _totalPatientCount; 

    private int _curDestroyCount = 0;
    private int CurDestroyCount
    {
        get => _curDestroyCount;
        set
        {
            _curDestroyCount = (value <= _totalPatientCount) ? value : _totalPatientCount;
            if (_curDestroyCount >= _totalPatientCount)
            {
                AllPatientFinish_Event?.Invoke();
                Log.CallInfo("所有病人完成检查");

            }
        }
    }





    private List<GameObject> _patientPrefabs = new List<GameObject>();

    private Transform _parent;

    private new void Awake()
    {
        base.Awake();
        GameObject prefab = Res.Load<GameObject>(Str_Def.PATIENT_1_DATA_PATH);
        _patientPrefabs.Add(prefab);
    }
    private void Start()
    {
        _parent = GameObject.Find("Patient").transform;
        if (_parent == null)
        {
            GameObject parent = new GameObject();
            _parent = parent.transform;
        }
        StartCoroutine(GenPatient(0, _genDurationPeriodPercent, _parent));
    }

    IEnumerator GenPatient(int idx, float genDurationPeriodPercent, Transform parent)
    {
        int patientCount = 0;
        float genDuration = genDurationPeriodPercent * PeriodManager.DAY_PERIOD_DURATION;
        WaitForSeconds duration = new WaitForSeconds(genDuration);
        yield return new WaitForSeconds(2f);
        while (patientCount < _totalPatientCount)
        {
            yield return duration;
            Log.Info($"生成病人{_patientPrefabs[idx].name}");
            var patient = Instantiate(_patientPrefabs[idx], parent).GetComponent<Patient>();
            patient.FinishInspection_Event += FinishInspection;
            patient.MoveNextInspection();
            patientCount++;
        }
    }

    private void FinishInspection(Transform patient)
    {
        Destroy(patient.gameObject, 1f);
        CurDestroyCount++;
        Log.Info($"{patient.name} 回收");
    }
}
