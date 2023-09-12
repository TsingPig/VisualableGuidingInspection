using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Highlighters;

public class Test : MonoBehaviour
{
    private Highlighter m_highlighter;
    private float m_timer;

    private void Start()
    {
        m_highlighter = GetComponent<Highlighter>();
        m_highlighter.GetRenderersInChildren();
    }

    private void Update()
    {
        m_timer += Time.deltaTime;
        if (m_timer > 0.25f)
        {
            m_timer = 0f;
            m_highlighter.enabled = !m_highlighter.enabled;
            return;
        }

        if (!m_highlighter.enabled)
            return;

        var sin = Mathf.Sin(Time.realtimeSinceStartup);
        sin = sin * 0.5f + 0.5f;

        m_highlighter.Settings.UseOuterGlow = true;
        m_highlighter.Settings.BoxBlurSize = Mathf.Lerp(0.01f, 0.025f, sin);
        m_highlighter.Settings.OuterGlowColorFront = Color.Lerp(Color.red, Color.green, sin);
        m_highlighter.Settings.BlurAdaptiveThickness = 0.5f;

        m_highlighter.Settings.UseOverlay = true;
        var colorA = new Color(1f, 0f, 0f, 0.1f);
        var colorB = new Color(0f, 1f, 0f, 0.1f);
        m_highlighter.Settings.OverlayFront.Color = Color.Lerp(colorA, colorB, sin);
    }
}