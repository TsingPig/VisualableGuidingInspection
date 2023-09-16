using System.Collections.Generic;
using UnityEngine;
namespace TsingPigSDK
{
    /// <summary>
    /// 面板管理器，用栈来存储UI
    /// </summary>
    public class PanelBuffer
    {
        private CursorState _stateRecord;
        public CursorState StateRecord { get => _stateRecord; set => _stateRecord = value; }

        public GameObject TopPanelObject
        {
            get { return GetSingleUI(_topPanel.UIType); }
        }

        /// <summary>
        /// 面板栈
        /// </summary>
        private Stack<BasePanel> _panelStack;

        /// <summary>
        /// 当前栈顶的面板
        /// </summary>
        private BasePanel _topPanel;

        private Dictionary<UIType, GameObject> _dicUI;
        public PanelBuffer()
        {
            _panelStack = new Stack<BasePanel>();
            _dicUI = new Dictionary<UIType, GameObject>();
        }

        /// <summary>
        /// panel面板入栈操作
        /// </summary>
        /// <param name="nextPanel">要显示的面板</param>
        public void Push(BasePanel nextPanel)
        {
            if (_panelStack.Count == 0)
            {
                UISystem.Instance.CursorState = CursorState.UI;
            }
            if (_panelStack.Count > 0)
            {
                _topPanel = _panelStack.Peek();
                _topPanel.OnPause();
            }

            _panelStack.Push(nextPanel);
            GetSingleUI(nextPanel.UIType);
            _topPanel = _panelStack.Peek();
            _topPanel.OnEntry();
        }
        public void Pop()
        {
           
            if (_panelStack.Count > 0)
            {
                _panelStack.Peek().OnExit();
                DestroyUI(_panelStack.Peek().UIType);
                _panelStack.Pop();
            }
            if (_panelStack.Count > 0)
            {
                _panelStack.Peek().OnResume();
            }
            else
            {
                UISystem.Instance.CursorState = _stateRecord;
            }
        }

        /// <summary>
        /// 显示一个UI对象
        /// </summary>
        /// <param name="type">ui信息</param>
        /// <returns></returns>
        public GameObject GetSingleUI(UIType type)
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
                    Log.Info("InstantiateSingleUI");
                    GameObject uiAsset = Res.Load<GameObject>(type.Name);
                    GameObject ui = GameObject.Instantiate(uiAsset, parent.transform);
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
            if (_dicUI.ContainsKey(type))
            {
                GameObject.Destroy(_dicUI[type]);
                _dicUI.Remove(type);
            }
        }

    }
}