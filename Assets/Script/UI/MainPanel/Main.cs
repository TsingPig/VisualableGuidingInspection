using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Main : MonoBehaviour
{
    public CanvasGroup[] Panels;

    private TMP_Text[] _txtsInfoPanel;

    public void OpenPanel(string name)
    {
        CanvasGroup targetPanel= Panels.First((CanvasGroup item) => item.name == name);
        OpenPanel(targetPanel);
    }
 
    public void ClosePanel(string name)
    {
        CanvasGroup targetPanel = Panels.First((CanvasGroup item) => item.name == name);
       ClosePanel(targetPanel);
    }
   
    private void Start()
    {
        for (int i = 1; i < Panels.Length; i++)
        {
            ClosePanel(Panels[i]);
        }

        _txtsInfoPanel = new TMP_Text[] {
                UIManager.Instance.GetOrAddComponentInChilden<TMP_Text>("已模拟时间"),
                UIManager.Instance.GetOrAddComponentInChilden<TMP_Text>("模拟日历"),
                UIManager.Instance.GetOrAddComponentInChilden<TMP_Text>("治疗中人数"),
                UIManager.Instance.GetOrAddComponentInChilden<TMP_Text>("完成人数"),
                UIManager.Instance.GetOrAddComponentInChilden<TMP_Text>("完成检查数"),
                UIManager.Instance.GetOrAddComponentInChilden<TMP_Text>("总设备数"),
                UIManager.Instance.GetOrAddComponentInChilden<TMP_Text>("使用中"),
                UIManager.Instance.GetOrAddComponentInChilden<TMP_Text>("维修中"),
        };
       
    }
    private void OpenPanel(CanvasGroup targetPanel)
    {
        targetPanel.alpha = 1f;
        targetPanel.interactable = true;
        targetPanel.blocksRaycasts = true;
    }
    private void ClosePanel(CanvasGroup targetPanel)
    {
        targetPanel.alpha = 0f;
        targetPanel.interactable = false;
        targetPanel.blocksRaycasts = false;
    }
}
