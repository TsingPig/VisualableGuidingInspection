using UnityEngine;

namespace TsingPigSDK
{
    public abstract class BasePanel
    {
        public UIType UIType { get; protected set; }
        public GameObject PanelObject
        {
            get
            {
                return UIManager.Instance.GetSingleUI(UIType);
            }
        }
        public BasePanel()
        {
            UIType = new UIType(this.GetType().Name);
        }

        public virtual void OnEntry()
        {
            Debug.Log($"打开{UIType.Name}面板");
        }
        public virtual void OnPause()
        {
            PanelObject.GetComponent<CanvasGroup>().interactable = false;
        }
        public virtual void OnResume()
        {
            PanelObject.GetComponent<CanvasGroup>().interactable = true;

        }
        public virtual void OnExit()
        {

        }

    }
}

//public class MainPanel : BasePanel
//{
//    static readonly string path = Str_Def.MainPanel;
//    public MainPanel() : base(new UIType(path))
//    {

//    }
//    /*
//     对于A a = new B();调用a中虚方法时，
//    new调用A的，override调用B的
//    override会将基类方法覆盖掉不存在，new是方法共存
//     */

//}

//public class EnsurePanel : BasePanel
//{
//    static readonly string path = Path.EnsurePanel;

//    //确认点击面板
//    public delegate void ContinueEvent();   //委托事件
//    private ContinueEvent _continueEvent;
//    private string ensureTipString;            //确认点击的字符串提示
//    public EnsurePanel(ContinueEvent continueEvent, string tipString = "是否确认？") : base(new UIType(path))
//    {
//        _continueEvent = continueEvent;
//        ensureTipString = tipString;
//    }
//    public override void OnEntry()
//    {

//        UIManager.Instance.GetOrAddComponentInChilden
//           <Button>("Cancel").onClick.AddListener(() =>
//           {
//               UIManager.panelManager.Pop();
//           });
//        UIManager.Instance.GetOrAddComponentInChilden
//           <Button>("Continue").onClick.AddListener(() =>
//           {

//               _continueEvent.Invoke();
//           });
//        UIManager.Instance.GetOrAddComponentInChilden
//           <Text>("EnsureTipText").text = ensureTipString;
//        //EnsureTipText
//    }
//}
