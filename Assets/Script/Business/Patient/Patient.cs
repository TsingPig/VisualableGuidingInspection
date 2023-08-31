using System.Collections;
using System.Collections.Generic;
using TsingPigSDK;
using UnityEngine;
using UnityEngine.AI;

public class Patient : MonoBehaviour
{
    #region 导航系统

    private NavMeshAgent _agent;

    private bool walk_active = false;

    private List<Animator> _anims = new List<Animator>();

    private Transform _prePatient;

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

    #endregion

    private Inspection _inspection;

    [SerializeField] private List<Instrument> _instruments;

    private PatientInfo _patientInfo;
    public PatientInfo PatientInfo => _patientInfo;
    public Inspection Inspection { get => _inspection; set => _inspection = value; }


    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();

        foreach (Animator a in CharacterCustomization.animators)
            _anims.Add(a);

    }

    private void Start()
    {
        _inspection = new Inspection();

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            MoveNextInspection();
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            _inspection.GetCurInspectionInfo();
        }
    }

    /// <summary>
    /// 进入下一项治疗
    /// </summary>
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

    /// <summary>
    /// 设置队列中跟随目标
    /// </summary>
    /// <param name="patient"></param>
    public void SetPrePatient(Transform patient)
    {
        if (_prePatient == null)
        {
            _prePatient = patient.parent;
            MoveTarget(patient);
        }
    }

    /// <summary>
    /// 跟随队列向前走
    /// </summary>
    /// <param name="patient"></param>
    public void FollowPrePatient(Transform patient)
    {
        StopAllCoroutines();
        StartCoroutine(FollowPrePatientCoroutine(patient));
    }

    /// <summary>
    /// 跟随队列向前走
    /// </summary>
    /// <param name="patient"></param>
    /// <returns></returns>
    IEnumerator FollowPrePatientCoroutine(Transform patient)
    {
        if (_prePatient == null)
        {
            MoveTarget(_instruments[0].Target);
        }
        else if (patient == _prePatient)
        {
            _prePatient = null;
            MoveTarget(_instruments[0].Target);
        }
        else
        {
            Log.Info($"{transform.name}跟随{_prePatient.name}");
            if (_instruments[0].Patients.Contains(this))
            {
                yield return new WaitForSeconds(_instruments[0].Patients.IndexOf(this) * 0.65f);
            }
            MoveTarget(_prePatient.GetChild(0));
        }
        yield break;
    }
    public void UpdatePrePatient(Transform target)
    {
        _prePatient = target.parent;
        MoveTarget(target);
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

            yield return StartCoroutine(instrument.StartInspection(this));

            Log.Info($"{gameObject.name} 治疗结束");

            _instruments.RemoveAt(0);

            MoveNextInspection();
        }

    }
}


