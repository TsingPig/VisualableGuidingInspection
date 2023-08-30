using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TsingPigSDK;

public class DataManager : Singleton<DataManager>
{
    private Address_SO _addressData;

    private PatientName_SO _patientNameData;
    public Address_SO AddressData => _addressData;
    public PatientName_SO PatientNameData => _patientNameData;
    private async void Init()
    {
        _addressData = await Res.LoadAsync<Address_SO>(Str_Def.ADDRESS_DATA_PATH);
        _patientNameData = await Res.LoadAsync<PatientName_SO>(Str_Def.PATIENT_NAME_DATA_PATH);
    }
    protected override void Awake()
    {
        base.Awake();
        Init();
        Log.CallInfo();
    }
}
