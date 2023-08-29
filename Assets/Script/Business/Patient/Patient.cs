using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Patient : MonoBehaviour
{
    private PatientInfo _patientInfo;

    private NavMeshAgent _agent;

    private NavMeshObstacle _obstacle;

    private bool walk_active = false;

    private List<Animator> _anims = new List<Animator>();

    private Transform _prePatient;

    [SerializeField] private List<Instrument> _instruments;

    public PatientInfo PatientInfo => _patientInfo;

    public CharacterCustomization CharacterCustomization;

    public bool Walk_Active
    {
        get { return walk_active; }
        set
        {
            walk_active = value;
            foreach (Animator a in _anims)
                a.SetBool("walk", walk_active);
        }
    }

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();

        _obstacle = GetComponent<NavMeshObstacle>();

        foreach (Animator a in CharacterCustomization.animators)
            _anims.Add(a);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            MoveNextInspection();
        }
    }

    public void MoveNextInspection()
    {
        if (_instruments.Count > 0)
        {
            MoveTarget(_instruments[0].AddMovingPatients(this));
        }
        else
        {
            Log.Info($"{gameObject.name} 所有治疗已完成");
        }

    }



    public void SetPrePatient(Transform patient)
    {
        if (_prePatient == null)
        {
            _prePatient = patient.parent;
            MoveTarget(patient);
        }
    }

    public void FollowPrePatient(Transform patient)
    {
        StopAllCoroutines();
        StartCoroutine(FollowPrePatientCoroutine(patient));
    }
    IEnumerator FollowPrePatientCoroutine(Transform patient)
    {
        if (_prePatient == null)
        {
            MoveTarget(_instruments[0].transform.GetChild(0));
        }
        else if (patient == _prePatient)
        {
            _prePatient = null;
            MoveTarget(_instruments[0].transform.GetChild(0));
        }
        else
        {
            Log.Info($"{transform.name}跟随{_prePatient.name}");
            if (_instruments[0].Patients.Contains(this))
            {
                yield return new WaitForSeconds(_instruments[0].Patients.IndexOf(this) * 0.3f);
            }
            MoveTarget(_prePatient.GetChild(0));
        }
        yield break;
    }


    public void UpdatePrePatient(Transform patient)
    {
        _prePatient = patient.parent;
        MoveTarget(patient);
    }

    private void MoveTarget(Transform target)
    {
        StopAllCoroutines();
        StartCoroutine(Move(target));
    }

    IEnumerator Move(Transform target)
    {
        if (target != null)
        {

            Walk_Active = true;
            _agent.SetDestination(target.position);
            Log.Info($"{gameObject.name} 开始寻路 {target.parent.name}");

            NavMeshHit hit;
            if (NavMesh.SamplePosition(target.position, out hit, 10f, NavMesh.AllAreas))
            {
                Vector3 reachablePosition = hit.position;
                _agent.SetDestination(reachablePosition);
                while (Vector3.Distance(transform.position, reachablePosition) > _agent.stoppingDistance)
                {

                    foreach (Animator a in _anims)
                        a.speed = (_agent.velocity.magnitude / _agent.speed) / 2f + 0.5f;
                    yield return null;
                }
            }

        }

        Log.Info($"{gameObject.name} 到达目的地 {target.name}");

        Walk_Active = false;

        if (target.parent.GetComponent<Patient>())
        {
            _instruments[0].EnQueue(this);
        }

        if (target.parent.TryGetComponent(out Instrument instrument))
        {

            Vector3 lookDirection = (target.position - transform.position).normalized;
            while (Vector3.Angle(lookDirection, transform.forward) > 45f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 4f);
                yield return null;
            }

            Log.Info($"{gameObject.name} 开始治疗");

            yield return StartCoroutine(instrument.Inspection(this));

            Log.Info($"{gameObject.name} 治疗结束");

            _instruments.RemoveAt(0);

            MoveNextInspection();
        }

    }
}


