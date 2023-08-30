using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace TsingPigSDK
{
    public class UIDicManager
    {
        /// <summary>
        /// UI信息缓存字典
        /// </summary>
        private Dictionary<UIType, GameObject> _dicUI;

        /// <summary>
        /// 显示一个UI对象
        /// </summary>
        /// <param name="type">ui信息</param>
        /// <returns></returns>
        public async Task<GameObject> GetSingleUI(UIType type)
        {
            GameObject parent = GameObject.Find("Canvas");
            if (parent != null)
            {
                if (_dicUI.ContainsKey(type))
                {
                    return _dicUI[type];
                }
                else
                {
                    GameObject uiAsset = await Res.LoadAsync<GameObject>(type.Path);
                    GameObject ui = GameObject.Instantiate(uiAsset) as GameObject;
                    ui.name = type.Name;
                    _dicUI.Add(type, ui);
                    return ui;
                }
            }
            else
            {
                Log.Error("丢失Canvas，请创建Canvas对象");
                return null;
            }
        }
        public void DestroyUI(UIType type)
        {
            foreach (var item in _dicUI.Values)
            {
                Debug.Log(item.ToString());
            }
            if (_dicUI.ContainsKey(type))
            {
                GameObject.Destroy(_dicUI[type]);
                _dicUI.Remove(type);
            }
        }
    }
}