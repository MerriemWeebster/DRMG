using UnityEngine;
using UnityEngine.Events;

namespace DRMG.Core
{
    /// <summary>
    /// Component that fires UnityEvent based on target event.
    /// </summary>
    public class MonoUnityEventsObserver : MonoBehaviour
    {
        internal enum EventType
        {
            Awake,
            Start,
            OnEnable,
            OnDisable,
            OnDestroy
        }

        [SerializeField, Tooltip("OnTargetEvent UnityEvent will fire during specified EventType.")]
        private EventType targetEventType;
        [SerializeField] private UnityEvent onTargetEvent;

        private void Awake()
        {
            if (targetEventType == EventType.Awake)
                onTargetEvent.Invoke();
        }

        private void Start()
        {
            if (targetEventType == EventType.Start)
                onTargetEvent.Invoke();
        }

        private void OnEnable()
        {
            if (targetEventType == EventType.OnEnable)
                onTargetEvent.Invoke();
        }

        private void OnDisable()
        {
            if (targetEventType == EventType.OnDisable)
                onTargetEvent.Invoke();
        }

        private void OnDestroy()
        {
            if (targetEventType == EventType.OnDestroy)
                onTargetEvent.Invoke();
        }
    }
}