﻿using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{

    [System.Serializable]
    private class TriggerUnityEvent : UnityEvent<Collider> { }

    [SerializeField]
    TriggerUnityEvent m_enter_events;
    [SerializeField]
    TriggerUnityEvent m_stay_events;
    [SerializeField]
    TriggerUnityEvent m_exit_events;

    [SerializeField]
    string[] m_trigger_tag;

    public void DestroyGameObject(Collider collision)
    {
        if (collision.gameObject)
        {
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (m_enter_events == null)
        {
            return;
        }

        if (m_trigger_tag.Length == 0)
        {
            m_enter_events?.Invoke(collision);
            return;

        }

        foreach (var str in m_trigger_tag)
        {
            if (collision.CompareTag(str))
            {
                m_enter_events?.Invoke(collision);
                return;
            }
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (m_exit_events == null)
        {
            return;
        }

        if (m_trigger_tag.Length == 0)
        {
            m_stay_events?.Invoke(collision);
            return;
        }

        foreach (var str in m_trigger_tag)
        {
            if (collision.CompareTag(str))
            {
                m_stay_events?.Invoke(collision);
                return;
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (m_stay_events == null)
        {
            return;
        }

        if (m_trigger_tag.Length == 0)
        {
            m_exit_events?.Invoke(collision);
            return;
        }

        foreach (var str in m_trigger_tag)
        {
            if (collision.CompareTag(str))
            {
                m_exit_events?.Invoke(collision);
                return;
            }
        }
    }
}
