using UnityEngine;

namespace AbilitySystem.Scripts.AttributeListeners
{
    public interface IAbilityAttributeValueProcessor
    {
        void ProcessAbilityAttributeValue(float currentValue);//and updates for example a text field
    }
    
    public abstract class AbilityAttributeValueProcessorBase : MonoBehaviour, IAbilityAttributeValueProcessor
    {
        public abstract void ProcessAbilityAttributeValue(float currentValue);
    }
}