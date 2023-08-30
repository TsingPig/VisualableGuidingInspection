using UnityEngine;
namespace TsingPigSDK
{

    public class UIManager : Singleton<UIManager>
    {
        [HideInInspector] public static PanelManager panelManager;
        
        public GameObject ActivePanelObject
        {
            get { return panelManager.TopPanelObject; }
        }

        private new void Awake()
        {
            base.Awake();
            panelManager = new PanelManager();
        }

        private void Start()
        {
            LoadMainPanel();
        }

        private void Update()
        {

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

        public void LoadMainPanel()
        {

        }
    }
}