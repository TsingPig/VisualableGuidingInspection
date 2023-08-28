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

    private bool walk_active = false;


    private List<Animator> _anims = new List<Animator>();

    [SerializeField] private List<Instrument> _instruments;

    public PatientInfo PatientInfo => _patientInfo;

    public CharacterCustomization CharacterCustomization;

    public bool Walk_Active
    {
        get { return walk_active; }
        set
        {
            walk_active = value;
            _agent.isStopped = !value;
            foreach (Animator a in _anims)
                a.SetBool("walk", walk_active);
        }
    }


    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();

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
            MoveInstrument(_instruments[0]);
            _instruments.RemoveAt(0);
        }
        else
        {
            Log.Info($"{gameObject.name} 所有治疗已完成");
        }

    }

    public void MoveInstrument(Instrument instrument)
    {
        StopAllCoroutines();
        StartCoroutine(Move(instrument.AddPatient(this)));
    }

    public void MoveTarget(Transform target)
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
            Log.Info($"{gameObject.name} 开始寻路 {target.name}");

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



            Vector3 lookDirection = (target.position - transform.position).normalized;
            while (Vector3.Angle(lookDirection, transform.forward) > 45f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 4f);
                yield return null;
            }

            Log.Info($"{gameObject.name} 到达目的地 {target.name}");

            Walk_Active = false;


            if (target.parent.TryGetComponent(out Instrument instrument))
            {
                Log.Info($"{gameObject.name} 开始治疗");

                yield return StartCoroutine(instrument.Inspection(this));

                Log.Info($"{gameObject.name} 治疗结束");

                MoveNextInspection();
            }

        }
    }


}