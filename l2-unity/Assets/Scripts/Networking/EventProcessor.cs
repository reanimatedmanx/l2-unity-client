using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;


public class EventProcessor : MonoBehaviour {

    private static EventProcessor _instance;
    public static EventProcessor Instance { get { return _instance; } }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
        } else if (_instance != this) {
            Destroy(this);
        }
    }

    private void Start() {
        if(World.Instance != null && World.Instance.OfflineMode) {
            this.enabled = false;
            return;
        }
    }

    public void QueueEvent(Action action) {
        lock(m_queueLock) {
            m_queuedEvents.Add(action);
        }
    }

    void Update() {
        MoveQueuedEventsToExecuting();
        try
        {
            while (m_executingEvents.Count > 0)
            {
                //Stopwatch stopwatch = Stopwatch.StartNew();
                Action e = m_executingEvents[0];
                
                m_executingEvents.RemoveAt(0);
                e();
               // DebugInfo(e, stopwatch);
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.Log("Критическая ошибка EventProcessor: " + ex.ToString());
        }

    }

    private void DebugInfo(Action e , Stopwatch stopwatch)
    { 
        stopwatch.Stop();
        UnityEngine.Debug.Log(" Event processor class " + e.Method.Name + " TimeLeft " + stopwatch.ElapsedMilliseconds + " ms ");
    }

    private void MoveQueuedEventsToExecuting() {
        lock(m_queueLock) {
            while (m_queuedEvents.Count > 0) {
                Action e = m_queuedEvents[0];
                m_executingEvents.Add(e);
                m_queuedEvents.RemoveAt(0);
            }
        }
    }

    private System.Object m_queueLock = new System.Object();
    private List<Action> m_queuedEvents = new List<Action>();
    private List<Action> m_executingEvents = new List<Action>();
}
