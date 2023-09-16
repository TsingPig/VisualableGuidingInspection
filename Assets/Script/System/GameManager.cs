using TsingPigSDK;
using UnityEngine;

public sealed class GameManager : Singleton<GameManager>
{
    private void Init()
    {
        Log.CallInfo($"{DataManager.Instance.name}生成");
        Log.CallInfo($"{PeriodManager.Instance.name}生成");
        Log.CallInfo($"{InspectionManager.Instance.name}生成");
        Log.CallInfo($"{InstrumentManager.Instance.name}生成");
        Log.CallInfo($"{PatientManager.Instance.name}生成");
        Log.CallInfo($"{InputManager.Instance.name}生成");
    }
    private void GameEntry()
    {
        UIManager.Instance.Enter(new MainPanel());

    }
    protected override void Awake()
    {
        base.Awake();
        Init();
 
    }
    private void Start()
    {
        GameEntry();
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            RandomSystem.RandomPatientInfo();
        }
        if (Input.GetKeyUp(KeyCode.T))
        {
            PeriodManager.Instance.LogPeriod();
        }
    }
}
