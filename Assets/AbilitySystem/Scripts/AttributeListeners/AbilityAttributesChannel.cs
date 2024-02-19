using UnityEngine;
using UnityEngine.Events;

namespace AbilitySystem.Scripts.AttributeListeners
{
    [CreateAssetMenu(fileName = "AbilityAttributesChannel", menuName = "ScriptableObjects/AbilityAttributesChannel", order = 3)]
    public class AbilityAttributesChannel : ScriptableObject
    {
        public UnityAction<string, UnityAction<float>, Component> OnSubsribe;
        
        public void SubscribeToAttributeCurrentValueChanged(string attributeName, UnityAction<float> action, Component subscriber)
        {
            OnSubsribe?.Invoke(attributeName, action, subscriber);
        }
    }
}