using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class canva : MonoBehaviour
{
    public static canva instance;

    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;


    private void Awake()
    {
        instance = this;
    }

    public bool IsMouseOverUI()
    {


        m_Raycaster = GetComponent<GraphicRaycaster>();


        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();

        m_Raycaster.Raycast(m_PointerEventData, results);


        if (results.Count == 0)
        {
            return false;
        }
        else
        {
            return true;
        }


    }
}
