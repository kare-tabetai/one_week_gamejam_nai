using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent2D : MonoBehaviour {

    [System.Serializable]
    private class TriggerUnityEvent : UnityEvent<Collider2D> { }

    [SerializeField]
    TriggerUnityEvent m_enter_events;
    [SerializeField]
    TriggerUnityEvent m_stay_events;
    [SerializeField]
    TriggerUnityEvent m_exit_events;

    [SerializeField]
    string[] m_trigger_tag;

    public void DestroyGameObject(Collider2D collision)
    {
        if (collision.gameObject)
        {
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
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

    private void OnTriggerStay2D(Collider2D collision)
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

    private void OnTriggerExit2D(Collider2D collision)
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
