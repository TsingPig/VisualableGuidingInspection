using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class GameManager : Singleton<GameManager>
{
    private void Init()
    {
        Log.CallInfo($"{DataManager.Instance.name}生成");
        Log.CallInfo($"{PeriodManager.Instance.name}生成");
    }
    private void GameEntry()
    {

    }
    protected override void Awake()
    {
        base.Awake();
        Init();
        GameEntry();
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            RandomInfo.RandomPatientInfo();
        }
        if (Input.GetKeyUp(KeyCode.T))
        {
            PeriodManager.Instance.LogPeriod();
        }
    }
}
