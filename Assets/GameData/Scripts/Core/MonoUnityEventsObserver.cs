using UnityEngine;
using UnityEngine.Events;

namespace DRMG.Core
{
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

        [SerializeField] private EventType targetEventType;
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