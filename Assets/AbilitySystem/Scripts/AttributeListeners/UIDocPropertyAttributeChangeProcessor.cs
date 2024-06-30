using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;

namespace AbilitySystem.Scripts.AttributeListeners
{
    public class UIDocPropertyAttributeChangeProcessor : AbilityAttributeValueProcessorBase
    {
        [SerializeField] private UIDocument _uiDocument;

        private Label _healthLabel;//find all names in visual asset tree and list them in a dropdown menu
        
        private void Awake()
        {
            _healthLabel = _uiDocument.rootVisualElement.Q<Label>("HealthValue");
            var button = _uiDocument.rootVisualElement.Q<Button>("ButtonTest");//todo delete
            if (button != null)
            {
                button.clicked += () => Debug.Log("OK button on Main Menu clicked.");
            }
            UQueryState<Label> query = _uiDocument.rootVisualElement.Query<Label>().Build();//todo for finding all the names like with attributes, assign in component by name in editor only
        }

        public override void ProcessAbilityAttributeValue(float currentValue)
        {
            if (_healthLabel == null)
            {
                Debug.LogError("Health value label not found!!!");
                return;
            }
            _healthLabel.text = currentValue.ToString(CultureInfo.InvariantCulture);
        }
    }
}