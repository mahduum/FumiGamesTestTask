using System;
using System.Collections.Generic;
using AbilitySystem.Scripts.AttributeListeners;
using AbilitySystem.Scripts.Data;
using UnityEngine;

namespace UI
{
    [Serializable]
    public class AttributeListenerBinding
    {
        public AbilityAttributeCreator AttributeToListen;
        public AbilityAttributeValueProcessorBase AbilityAttributeValueProcessor;//i.e. object with text component
    }
    
    public class AbilityAttributeListenersBinder : MonoBehaviour
    {
        [SerializeField] private List<AttributeListenerBinding> _attributeListenerBindings;
        [SerializeField] private AbilityAttributesChannel _abilityAttributesChannel;

        private void Start()
        {
            foreach (var binding in _attributeListenerBindings)
            {
                _abilityAttributesChannel
                    .SubscribeToAttributeCurrentValueChanged(
                        binding.AttributeToListen.Name,
                        binding.AbilityAttributeValueProcessor.ProcessAbilityAttributeValue,
                        binding.AbilityAttributeValueProcessor);//todo it should only need to take in processor interface but component is for UniRx
            }
        }

    }
}