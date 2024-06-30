using TMPro;
using UnityEngine;

namespace AbilitySystem.Scripts.AttributeListeners
{
    public class AbilityAbilityAttributeTextValueProcessor : AbilityAttributeValueProcessorBase
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private string _textFormat;
        [SerializeField] private AbilityAttributesChannel _abilityAttributesChannel;

        private void Start()
        {
            //_abilityAttributesChannel.SubscribeToAttributeCurrentValueChanged();
        }

        public override void ProcessAbilityAttributeValue(float currentValue)//subscribe this way to UI doc property
        {
            _text.text = string.Format(_textFormat, currentValue);
        }
    }
}