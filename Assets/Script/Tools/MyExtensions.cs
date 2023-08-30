using System.Collections.Generic;
using TsingPigSDK;
using UnityEngine;

public static class MyExtensions
{
    public static T GetRandomItem<T>(this List<T> list)
    {
        int index = Random.Range(0, list.Count);
        if (list == null || list.Count == 0)
        {
            Log.Error($"{typeof(T).Name}列表未初始化或者长度为0");
            return default(T); // 或者根据实际需求返回合适的默认值
        }
        return list[index];
    }
}
