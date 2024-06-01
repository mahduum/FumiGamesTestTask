using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace AbilitySystem.Scripts.AttributeListeners
{
    [CreateAssetMenu(fileName = "AbilityAttributesChannel", menuName = "ScriptableObjects/AbilityAttributesChannel", order = 3)]
    public class AbilityAttributesChannel : ScriptableObject
    {
        private UnityEvent<string, UnityAction<float>, Component> _onSubscribe;
        
        public void SubscribeToAttributeCurrentValueChanged(string attributeName, UnityAction<float> action, Component subscriber)
        {
            _onSubscribe?.Invoke(attributeName, action, subscriber);
        }

        public void AddSubscriptionHandler(UnityAction<string, UnityAction<float>, Component> subscriptionHandler)
        {
            _onSubscribe.AddListener(subscriptionHandler);
        }
        
        public void RemoveSubscriptionHandler(UnityAction<string, UnityAction<float>, Component> subscriptionHandler)
        {
            _onSubscribe.RemoveListener(subscriptionHandler);
        }
    }
}