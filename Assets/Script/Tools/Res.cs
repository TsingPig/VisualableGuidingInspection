using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;

public static class Res
{
    public static async Task<T> Load<T>(string path) where T : ScriptableObject
    {
        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(path);
        await handle.Task;
        Log.CallInfo($"{handle.Result.name}º”‘ÿÕÍ≥…");
        T result = handle.Result;
        Addressables.Release(handle);
        return result;
    }
}
