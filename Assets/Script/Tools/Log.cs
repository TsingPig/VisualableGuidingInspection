using System;
using System.Diagnostics;
using System.Reflection;
using Debug = UnityEngine.Debug;
public static class Log
{
    public static void CallInfo(string msg = "")
    {
        MethodBase callingMethod = new StackTrace().GetFrame(1).GetMethod();
        Type callingType = callingMethod.DeclaringType;
        //Debug.Log($"{callingType.Name} : {callingMethod.Name}    Msg:{msg}");
        Info(" ", callingType.Name, callingMethod.Name, $"  Msg : {msg}");
    }
    public static void Error(string msg = "")
    {
        Debug.LogError($"错误：{msg}");
    }
    public static void Info(string spliter = " ", params string[] strings)
    {
        string result = string.Join(spliter, strings);
        Debug.Log(result);
    }
    public static void Info(PatientInfo patientInfo)
    {
        Log.Info(" ",
            "ID：" + patientInfo.patientID,
            "姓名：" + patientInfo.patientName,
            "性别：" + patientInfo.patientGender.ToString(),
            "年龄：" + patientInfo.patientAge.ToString(),
            "地址：" + patientInfo.patientAddress,
            "电话：" + patientInfo.patientPhoneNumber
        );
    }
}
