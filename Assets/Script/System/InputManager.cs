using UnityEngine;
using TsingPigSDK;
using Cinemachine;
using System;
using UnityEngine.Playables;

public enum SelectMode
{
    Single,
    Multi,
    Null
}

public class InputManager : Singleton<InputManager>
{
    private int _curCamIdx = 0;

    private CinemachineVirtualCameraBase[] _cams;

    private Transform _cinemachineVirtualCameraTarget;
    public Transform CinemachineVirtualCameraTarget
    {
        get => _cinemachineVirtualCameraTarget;
        set
        {
            for (int i = 1; i < _cams.Length; i++)
            {
                _cams[i].Follow = value;
                _cams[i].LookAt = value;
            }
        }
    }

    MyList<ISelectable> _selectables = new MyList<ISelectable>();

    private bool _canSelect = false;

    private SelectMode _selectMode = SelectMode.Single;
    public bool CanSelect { get => _canSelect; set => _canSelect = value; }
    public SelectMode SelectMode { get => _selectMode; set => _selectMode = value; }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            
            _cams[_curCamIdx].enabled = false;
            _curCamIdx = (_curCamIdx + 1) % _cams.Length;
            _cams[_curCamIdx].enabled = true;
            if (_curCamIdx == 2)
            {
                UIManager.Instance.CursorState = CursorState.Lock;
            }
            else
            {
                UIManager.Instance.CursorState = CursorState.None;
            }
        }

        if (UIManager.Instance.CursorState==CursorState.None &&  Input.GetKeyDown(KeyCode.Mouse0))
        {
            ScreenPointToRay("Selectable");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            EnterInfoPanel();
        }
    }
    private void ScreenPointToRay(string layerName)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer(layerName))
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
                _selectables.Clear();
            }
        }
    }
    private void EnterInfoPanel()
    {
        if (SelectMode == SelectMode.Single && _selectables.Count > 0)
        {
            _selectables[0].EnterInfoPanel();
        }
    }
    private void OnClick(ISelectable selectable)
    {
        if (SelectMode == SelectMode.Single)
        {
            if (_selectables.Count == 0)
            {
                _selectables.Add(selectable);
            }
            else
            {
                if (_selectables[0] == selectable)
                {
                    _selectables.Clear();
                }
                else
                {
                    _selectables.Pop();
                    _selectables.Add(selectable);
                }
            }
        }
    }

    private void Init()
    {
        _cams = new CinemachineVirtualCameraBase[]
        {
            GameObject.Find("CMWorld").GetComponent<CinemachineVirtualCamera>(),
            GameObject.Find("CMLock").GetComponent<CinemachineVirtualCamera>(),
            GameObject.Find("CMFree").GetComponent<CinemachineFreeLook>()
        };
        //_cams[0] = GameObject.Find("CMWorld").GetComponent<CinemachineVirtualCamera>();
        //_cams[1] = GameObject.Find("CMLock").GetComponent<CinemachineVirtualCamera>();
        //_cams[2] = GameObject.Find("CMFree").GetComponent<CinemachineFreeLook>();

        _selectables.OnItemAdded_Event += (ISelectable selectable) => selectable.OnSelected();
        _selectables.OnItemRemoved_Event += (ISelectable selectable) => selectable.OffSelected();
    }

    private new void Awake()
    {
        base.Awake();
        Init();
    }
}
