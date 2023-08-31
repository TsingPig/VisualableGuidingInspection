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
        _instrumentInfo = InstrumentManager.Instance.
    }
    private float GetTime(InspectionInfo inspectionInfo)
    {

    }
    public int PatientCount => Patients.Count;

    public float WaitingTime
    {
        get
        {

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

        yield return new WaitForSeconds();

        Log.Info($"{patient.name} ÷Œ¡∆Ω· ¯");

        InspectionEnd_Event?.Invoke(patient.transform);

        Patients.Remove(patient);
    }
}
