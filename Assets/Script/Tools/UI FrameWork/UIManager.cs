using System;
using UnityEngine;
namespace TsingPigSDK
{

    public class UIManager : Singleton<UIManager>
    {
        private PanelBuffer _panelBuffer;
        public GameObject ActivePanelObject
        {
            get { return _panelBuffer.TopPanelObject; }
        }
        private new void Awake()
        {
            base.Awake();
            _panelBuffer = new PanelBuffer();
        }

        private void Start()
        {
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.A))
            {
                Enter(Str_Def.PATIENT_INFO_PANEL_DATA_PATH);
            }else if(Input.GetKeyUp(KeyCode.B))
            {
                Pop();
            }
        }

        /// <summary>
        /// 给当前的活动面板添加组件、或者获得当前活动面板的某个组件
        /// </summary>
        /// <typeparam name="T">组件类型 限定为Component类型</typeparam>
        /// <returns></returns>
        public T GetOrAddComponetToActivePanel<T>() where T : Component
        {
            if (ActivePanelObject.GetComponent<T>() == null)
            {
                ActivePanelObject.AddComponent<T>();

            }
            return ActivePanelObject.GetComponent<T>();
        }

        /// <summary>
        /// 根据名称查找一个子对象
        /// </summary>
        /// <param name="name">子对象名称</param>
        /// <returns></returns>
        public GameObject FindChildGameObject(string name)
        {
            Transform[] transforms = ActivePanelObject.GetComponentsInChildren<Transform>();
            foreach (var item in transforms)
            {
                if (item.gameObject.name == name)
                {
                    return item.gameObject;
                }
            }
            Debug.LogWarning($"{ActivePanelObject.name}里找不到名为{name}的子物体");
            return null;
        }

        /// <summary>
        /// 根据名称获取子对象的组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="name">子对象名称</param>
        /// <returns></returns>
        public T GetOrAddComponentInChilden<T>(string name) where T : Component
        {
            GameObject child = FindChildGameObject(name);
            if (child != null)
            {
                if (child.GetComponent<T>() != null)
                {
                    return child.GetComponent<T>();
                }
                child.AddComponent<T>();
            }
            return null;
        }

        public void Enter(string panelName)
        {
            Type type = Type.GetType(panelName);

            if (type != null)
            {
                object panelInstance = Activator.CreateInstance(type);
                _panelBuffer.Push(panelInstance as BasePanel);
            }
            else
            {
                Log.Error($"未找到类型：{panelName}");
            }
        }

        public void Pop()
        {
            _panelBuffer.Pop();
        }

        public GameObject GetSingleUI(UIType uiType)
        {
            return _panelBuffer.GetSingleUI(uiType);
        }
    }
}