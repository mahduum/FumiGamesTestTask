using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem.Scripts.Data
{
    [CreateAssetMenu(fileName = "AttributeSet", menuName = "ScriptableObjects/AttributeSet", order = 2)]
    public class AttributeSetData : ScriptableObject
    {
        public string Name = "";
        public List<AbilityAttributeCreator> AbilityAttributes;
    }
}