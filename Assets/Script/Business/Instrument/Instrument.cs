using System;
using System.Collections;
using System.Collections.Generic;
using TsingPigSDK;
using UnityEngine;

public class Instrument : MonoBehaviour
{
    private InstrumentInfo _instrumentInfo;

    private List<Patient> _patients = new List<Patient>();

    public Action<Transform> InspectionStart_Event;

    public Action<Transform> InspectionEnd_Event;

    public Action<Transform> AddQueueingPatients_Event;
    public Transform Target => transform.GetChild(0);
    public List<Patient> Patients { get => _patients; }

    private void Start()
    {
        _instrumentInfo = InstrumentManager.Instance.GetInfo(this);
    }

    /// <summary>
    /// 获得某项检查在该机器中的检查时间。
    /// </summary>
    /// <param name="inspectionInfo">检查项目的ID</param>
    /// <returns></returns>
    private float GetTime(InspectionInfo inspectionInfo)
    {
        float periodCountPercent = _instrumentInfo.InspectionIDs.Find(info => info.inspectionID == inspectionInfo.instrumentID).periodCountPercent;
        float periodDuration = PeriodManager.DAY_PERIOD_DURATION;
        return periodCountPercent * periodDuration;
    }

    public int PatientCount => Patients.Count;
    public float WaitingTime
    {
        get
        {
            float waitingTIme = 0f;
            foreach (var patient in Patients)
            {
                waitingTIme += GetTime(patient.Inspection.CurInspectionInfo);
            }
            return waitingTIme;
        }
    }

    public Transform AddMovingPatients(Patient patient)
    {
        InspectionStart_Event += patient.SetPrePatient;
        AddQueueingPatients_Event += patient.UpdatePrePatient;
        InspectionEnd_Event += patient.FollowPrePatient;
        return Target;
    }

    public void EnQueue(Patient patient)
    {
        Patients.Add(patient);

        InspectionStart_Event -= patient.SetPrePatient;

        AddQueueingPatients_Event -= patient.UpdatePrePatient;

        AddQueueingPatients_Event?.Invoke(patient.transform.GetChild(0));
    }

    public IEnumerator StartInspection(Patient patient)
    {
        InspectionStart_Event -= patient.SetPrePatient;

        AddQueueingPatients_Event -= patient.UpdatePrePatient;

        InspectionEnd_Event -= patient.FollowPrePatient;

        InspectionStart_Event?.Invoke(patient.transform.GetChild(0));

        float time = GetTime(patient.Inspection.CurInspectionInfo);

        yield return new WaitForSeconds(time);

        Log.Info($"{patient.name} 治疗结束");

        InspectionEnd_Event?.Invoke(patient.transform);

        Patients.Remove(patient);
    }
}
