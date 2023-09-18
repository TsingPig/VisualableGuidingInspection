using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Main : MonoBehaviour
{
    public CanvasGroup[] Panels;

    private TMP_Text[] _txtsInfoPanel;

    /// <summary>
    /// 根据面板名字，切换Panel面板
    /// </summary>
    /// <param name="name">面板名字</param>
    public void SwitchPanel(string name)
    {
        CanvasGroup targetPanel = Panels.First((CanvasGroup item) => item.name == name);
        if (targetPanel != null)
        {
            var elemManager = UIManager.Instance.GetOrAddComponentInChilden<DemoElementSway>($"Btn_{name}", transform);
            bool wmSelect = elemManager.wmSelected;
            if (wmSelect)
            {
                ClosePanel(name);

            }
            else
            {
                OpenPanel(name);
            }
        }
    }
 
    private void OpenPanel(string name)
    {
        CanvasGroup targetPanel = Panels.First((CanvasGroup item) => item.name == name);
        if (targetPanel != null)
        {
            OpenPanel(targetPanel);
            var elemManager = UIManager.Instance.GetOrAddComponentInChilden<DemoElementSway>($"Btn_{name}", transform);
            elemManager.WindowManagerSelect();
        }
    }

    private void ClosePanel(string name)
    {
        CanvasGroup targetPanel = Panels.First((CanvasGroup item) => item.name == name);
        if (targetPanel != null)
        {
            ClosePanel(targetPanel);
            UIManager.Instance.GetOrAddComponentInChilden<DemoElementSway>($"Btn_{name}", transform).WindowManagerDeselect();
        }
    }


    private void Start()
    {
        for (int i = 1; i < Panels.Length; i++)
        {
            ClosePanel(Panels[i]);
        }

        _txtsInfoPanel = new TMP_Text[] {
                UIManager.Instance.GetOrAddComponentInChilden<TMP_Text>("已模拟时间",transform),
                UIManager.Instance.GetOrAddComponentInChilden<TMP_Text>("模拟日历",transform),
                UIManager.Instance.GetOrAddComponentInChilden<TMP_Text>("治疗中人数", transform),
                UIManager.Instance.GetOrAddComponentInChilden<TMP_Text>("完成人数", transform),
                UIManager.Instance.GetOrAddComponentInChilden<TMP_Text>("完成检查数", transform),
                UIManager.Instance.GetOrAddComponentInChilden<TMP_Text>("总设备数", transform),
                UIManager.Instance.GetOrAddComponentInChilden<TMP_Text>("使用中", transform),
                UIManager.Instance.GetOrAddComponentInChilden<TMP_Text>("维修中", transform),
        };
    }
    private void Update()
    {
        _txtsInfoPanel[0].text = PeriodManager.Instance.CurrentTimeString;
        _txtsInfoPanel[1].text = PeriodManager.Instance.CurrentPeriodString;
        _txtsInfoPanel[2].text = PatientManager.Instance.CurInspectingCount.ToString();
        _txtsInfoPanel[3].text = PatientManager.Instance.CurDestroyCount.ToString();
        _txtsInfoPanel[4].text = InspectionManager.Instance.CurFinishedInspectionsCount.ToString();
        _txtsInfoPanel[5].text = InstrumentManager.Instance.InstrumentCount.ToString();
    }
    private void OpenPanel(CanvasGroup targetPanel)
    {
        var animator = targetPanel.GetComponent<Animator>();
        animator?.Play("In");
        targetPanel.alpha = 1f;
        targetPanel.interactable = true;
        targetPanel.blocksRaycasts = true;
    }
    private void ClosePanel(CanvasGroup targetPanel)
    {
        if(targetPanel.interactable)
        {
            var animator = targetPanel.GetComponent<Animator>();
            animator?.Play("Out");
            targetPanel.alpha = 0f;
            targetPanel.interactable = false;
            targetPanel.blocksRaycasts = false;
        }
  
    }
}
