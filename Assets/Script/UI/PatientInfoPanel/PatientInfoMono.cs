using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.MUIP;
public class PatientInfoMono : MonoBehaviour
{
    ListView _listView;
    WindowManager _manager;
    private void Awake()
    {
        _manager = transform.GetChild(0).GetComponent<WindowManager>();
    }
    public void WindowsChange()
    {

        int index = _manager.currentWindowIndex;
        switch (index)
        {
            case 0:
                break;
            case 1:
                _listView = UIManager.Instance.GetOrAddComponentInChilden<ListView>("患者检查项目列表", transform);
                _listView.InitializeItems(5);
                break;
        }
    }
}
