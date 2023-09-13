using UnityEngine;
using TsingPigSDK;
using System.Collections.Generic;
using UnityEngine.UI;

public enum SelectMode
{
    Single,
    Multi,
    Null
}

public class InputManager : Singleton<InputManager>
{
    //private Dictionary<string, List<ISelectable>> _dic_selectables = new Dictionary<string, List<ISelectable>>();
    
    List<ISelectable> _selectables = new List<ISelectable>();    

    private bool _canSelect = false;

    private SelectMode _selectMode = SelectMode.Single;
    public bool CanSelect { get => _canSelect; set => _canSelect = value; }
    public SelectMode SelectMode { get => _selectMode; set => _selectMode = value; }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ScreenPointToRay("Selectable");
        }
    }
    private void ScreenPointToRay(string layerName)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.layer==LayerMask.NameToLayer(layerName))
            {
                ISelectable selectable = hit.collider.transform.GetComponent<ISelectable>();
                if (selectable != null)
                {
                    OnClick(selectable);
                }
                else
                {
                    Log.Warning($"{layerName} 没有找到 ISelectable组件");
                }
            }
            else
            {
                foreach(var selectable in _selectables)
                {
                    selectable.OffSelected();
                }
                _selectables.Clear();
                Log.Info("清空_selectables");
            }
        }
    }
    private void OnClick(ISelectable selectable)
    {
        if (SelectMode == SelectMode.Single)
        {
            if (_selectables.Count == 0)
            {
                _selectables.Add(selectable);
                selectable.OnSelected();
            }
            else
            {
                if (_selectables[0] == selectable)
                {
                    _selectables.Clear();
                    selectable.OffSelected();
                }
                else
                {
                    _selectables[0].OffSelected();
                    _selectables[0] = selectable;
                    selectable.OnSelected();
                }
            }
        }
    }
    private void Init()
    {

    }

    private new void Awake()
    {
        base.Awake();
        Init();
    }
}
