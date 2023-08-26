using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class GameManager : Singleton<GameManager>
{
    private void Init()
    {
        Log.CallInfo($"{DataManager.Instance.name}Éú³É");
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
        if (Input.GetKeyUp(KeyCode.T))
        {
            RandomInfo.RandomPatientInfo();
        }
    }
}
