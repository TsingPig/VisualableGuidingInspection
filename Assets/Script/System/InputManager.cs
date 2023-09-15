using UnityEngine;
using TsingPigSDK;
using Cinemachine;
public enum SelectMode
{
    Single,
    Multi,
    Null
}

public class InputManager : Singleton<InputManager>
{
    private CinemachineVirtualCameraBase _cam;

    private Transform _cinemachineVirtualCameraTarget;

    public Transform CinemachineVirtualCameraTarget
    {
        get => _cinemachineVirtualCameraTarget;
        set
        {
            _cam.Follow = value;
            _cam.LookAt = value;
        }
      
    }

    MyList<ISelectable> _selectables = new MyList<ISelectable>();

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
        _cam=FindObjectOfType<CinemachineVirtualCameraBase>();
        _selectables.OnItemAdded_Event += (ISelectable selectable) => selectable.OnSelected();
        _selectables.OnItemRemoved_Event += (ISelectable selectable) => selectable.OffSelected();
    }

    private new void Awake()
    {
        base.Awake();
        Init();
    }
}
