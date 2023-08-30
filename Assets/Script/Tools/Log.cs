using System;
using System.Diagnostics;
using System.Reflection;
using Debug = UnityEngine.Debug;

namespace TsingPigSDK
{

    public static partial class Log
    {
        public static void CallInfo(string msg = "")
        {
            MethodBase callingMethod = new StackTrace().GetFrame(1).GetMethod();
            Type callingType = callingMethod.DeclaringType;
            //Debug.Log($"{callingType.Name} : {callingMethod.Name}    Msg:{msg}");
            Info(callingType.Name, callingMethod.Name, $"  Msg : {msg}");
        }
        public static void Error(string msg = "")
        {
            Debug.LogError($"´íÎó£º{msg}");
        }
        public static void Info(params string[] strings)
        {
            string result = string.Join(" ", strings);
            Debug.Log(result);
        }
        
        public static void Warning(params string[] strings)
        {
            string result = string.Join(" ", strings);
            Debug.LogWarning(result);
        }
    }
}
