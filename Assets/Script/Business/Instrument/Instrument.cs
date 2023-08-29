using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instrument : MonoBehaviour
{

    public Action<Transform> InspectionStart_Event;

    public Action<Transform> InspectionEnd_Event;

    public Action<Transform> AddQueueingPatients_Event;

    public Transform Target => transform.GetChild(0);

    public Transform AddMovingPatients(Patient patient)
    {
        InspectionStart_Event += patient.MoveTarget;
        AddQueueingPatients_Event += patient.MoveTarget;
        InspectionEnd_Event += patient.FollowPrePatient;
        return Target;
    }

    public void EnQueue(Patient patient)
    {
        InspectionStart_Event -= patient.MoveTarget;

        AddQueueingPatients_Event -= patient.MoveTarget;

        AddQueueingPatients_Event?.Invoke(patient.transform.GetChild(0));
    }

    public IEnumerator Inspection(Patient patient)
    {
        InspectionStart_Event -= patient.MoveTarget;

        AddQueueingPatients_Event-= patient.MoveTarget;

        InspectionEnd_Event-= patient.FollowPrePatient;

        InspectionStart_Event?.Invoke(patient.transform.GetChild(0));

        yield return new WaitForSeconds(3f);

        Log.Info($"{patient.name} ÷Œ¡∆Ω· ¯");

        InspectionEnd_Event?.Invoke(patient.transform);


    }
}
