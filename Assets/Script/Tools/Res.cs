using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
namespace TsingPigSDK
{
    public static class Res
    {
        public static T Load<T>(string path)
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(path);
            Log.CallInfo($"{handle.Result}异步加载完成");
            T result = handle.WaitForCompletion();
            Addressables.Release(handle);
            return result;
        }

        public static async Task<T> LoadAsync<T>(string path)
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(path);
            await handle.Task;
            Log.CallInfo($"{handle.Result}异步加载完成");
            T result = handle.Result;
            Addressables.Release(handle);
            return result;
        }
    }
}