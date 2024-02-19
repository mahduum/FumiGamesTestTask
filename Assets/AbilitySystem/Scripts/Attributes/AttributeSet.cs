using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Scripts.Data;
using UnityEngine;

namespace AbilitySystem.Scripts.Attributes
{
    [RequireComponent(typeof(AbilitySystem))]
    public class AttributeSet : MonoBehaviour//allow adding subsets? check if different sets dont have attributes in common
    {
        [SerializeField]
        private AttributeSetData _attributeSetData;

        private readonly Dictionary<string, AbilityAttribute> _abilityAttributes = new Dictionary<string, AbilityAttribute>();

        private void Awake()
        {
            foreach (var attribute in _attributeSetData.AbilityAttributes)
            {
                if (_abilityAttributes.ContainsKey(attribute.Name))
                {
                    Debug.LogWarning($"{nameof(AttributeSetData)} should contain only a single instance of any type, another instance of {nameof(attribute.Name)} will not be added to {nameof(AttributeSet)}");
                    continue;
                }
                
                _abilityAttributes.Add(attribute.Name, attribute.Create());
            }

            GetComponent<AbilitySystem>().RegisterAttributeSet(this);
        }

        public bool TryGetAbilityAttribute(string className, out AbilityAttribute foundAttribute)
        {
            if (_abilityAttributes.TryGetValue(className, out AbilityAttribute attribute))
            {
                foundAttribute = attribute;
                return true;
            }

            foundAttribute = null;
            return false;
        }

        public AbilityAttribute[] GetAllAbilityAttributes()
        {
            return _abilityAttributes.Values.ToArray();
        }
    }
}