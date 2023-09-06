using System.Collections;
using System.Collections.Generic;
using TsingPigSDK;
using UnityEngine;

public class PatientManager : Singleton<PatientManager>
{
    public int TotalPatientCount = 40;

    public float GenDurationPeriodPercent = 0.12f;

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
        StartCoroutine(GenPatient(0, GenDurationPeriodPercent, _parent));
    }

    IEnumerator GenPatient(int idx, float genDurationPeriodPercent, Transform parent)
    {
        int patientCount = 0;
        float genDuration = genDurationPeriodPercent * PeriodManager.DAY_PERIOD_DURATION;
        WaitForSeconds duration = new WaitForSeconds(genDuration);
        yield return new WaitForSeconds(2f);
        while (patientCount < TotalPatientCount)
        {
            yield return duration;
            Log.Info($"生成病人{_patientPrefabs[idx].name}");
            var patient = Instantiate(_patientPrefabs[idx], parent).GetComponent<Patient>();
            patient.FinishAllInspection_Event += FinishAllInspection;
            patient.MoveNextInspection();
            patientCount++;
        }
    }

    private void FinishAllInspection(Transform patient)
    {
        Destroy(patient, 1f);
        Log.Info($"{patient.name} 回收");

    }
}
