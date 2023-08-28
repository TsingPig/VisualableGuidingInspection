using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Patient : MonoBehaviour
{
    private PatientInfo _patientInfo;

    private NavMeshAgent _agent;

    private bool walk_active = false;

    [SerializeField] private Transform _target;

    private List<Animator> _anims = new List<Animator>();


    public PatientInfo PatientInfo => _patientInfo;

    public CharacterCustomization CharacterCustomization;
    public Transform Target { get => _target; set => _target = value; }

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
        foreach (Animator a in CharacterCustomization.animators)
            _anims.Add(a);
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartCoroutine(MoveNext());
        }
    }
    IEnumerator MoveNext()
    {
        if (Target != null)
        {
            Walk_Active = true;
            _agent.SetDestination(_target.position);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(_target.position, out hit, 10f, NavMesh.AllAreas))
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

            Debug.Log("到达目的");
            _agent.isStopped = true;
            Walk_Active = false;


            Vector3 lookDirection = (Target.position - transform.position).normalized;
            while (Vector3.Angle(lookDirection, transform.forward) > 45f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 4f);
                yield return null;
            }

        }
    }


}