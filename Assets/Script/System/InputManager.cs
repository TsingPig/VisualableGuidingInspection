using UnityEngine;
using Highlighters;
using TsingPigSDK;
using System.Collections.Generic;

public enum SelectMode
{
    Single,
    Multi,
    Null
}

public class InputManager : Singleton<InputManager>
{
    private List<ISelectable> _selectables = new List<ISelectable>(); 

    private bool _canSelect = false;

    private SelectMode _selectMode= SelectMode.Single;
    public bool CanSelect { get => _canSelect; set => _canSelect = value; }
    public SelectMode SelectMode { get => _selectMode; set => _selectMode = value; }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Patient"))
                {
                    Transform patient = hit.collider.transform;

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
