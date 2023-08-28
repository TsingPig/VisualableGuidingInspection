using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instrument : MonoBehaviour
{
    private List<Patient> patients = new List<Patient>();
    public List<Patient> Patients { get => patients; set => patients = value; }

    public Action<Transform> PatientChangeTarget_Event;

    public Transform Target => transform.GetChild(0);
    public Transform AddPatient(Patient patient)
    {
        Patients.Add(patient);
        PatientChangeTarget_Event += patient.MoveTarget;
        return Target;
    }
 
    public IEnumerator Inspection(Patient patient)
    {
        PatientChangeTarget_Event -= patient.MoveTarget;

        PatientChangeTarget_Event?.Invoke(patient.transform.GetChild(0));

        yield return new WaitForSeconds(3f);

        Log.Info($"{patient.name} ÷Œ¡∆Ω· ¯");

        Patients.Remove(patient);

        PatientChangeTarget_Event?.Invoke(Target);
     

    }
}
