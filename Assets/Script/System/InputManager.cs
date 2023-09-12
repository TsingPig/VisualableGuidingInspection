using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TsingPigSDK;
public class InputManager : Singleton<InputManager>
{

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Mouse0))
        //{
        //    RaycastHit hit;
        //    Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
        //    if(Physics.Raycast(ray, out hit))
        //    {
        //        if (hit.collider.CompareTag("Patient"))
        //        {

        //        }
        //    }
        //}
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
