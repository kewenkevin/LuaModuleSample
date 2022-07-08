using System;
using System.Collections;
using System.Collections.Generic;
using ND.UI;
using UnityEngine;

public class UIExpansionDemo : MonoBehaviour
{
    private UIExpansion m_uiExpansion;

    private float m_timer = 1;

    private float timer = 0;

    private int m_index = 0;

    private void Awake()
    {
        m_uiExpansion = GetComponent<UIExpansion>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_uiExpansion == null)
        {
            return;
        }
        
        timer += Time.deltaTime;
        if (timer >= m_timer)
        {
            timer = 0;

            if (m_index >= 3)
            {
                m_index = 0;
            }
            else
            {
                m_index++;
            }
            
            m_uiExpansion.SetController("ControllerTest",m_index);
        }
    }
}
