using System;
using System.Collections.Generic;
using AbilitySystem.Scripts.Abilities;
using AbilitySystem.Scripts.Attributes;
using AbilitySystem.Scripts.GameplayEffects;
using UnityEngine;

namespace AbilitySystem.Scripts.Data//todo try making this assembly if everything works ok, or make an assembly all ability system
{
    [CreateAssetMenu(fileName = "GameplayEffectData", menuName = "ScriptableObjects/GameplayEffect", order = 2)]
    public class GameplayEffectData : ScriptableObject
    {
        [SerializeField]
        public List<AttributeModifier> ModifierInfos;
        
        [SerializeField]
        public AbilityTagCreator AbilityActivationTag;
        
        [SerializeField]
        public List<AbilityTagCreator> RequiredAbilityTags;
        
        [SerializeField]
        public List<AbilityTagCreator> AbilityTagsToRemove;

        [SerializeField] public GameplayEffectDuration DurationType;

        [SerializeField] public double Duration = 0;

        [SerializeField] public GameplayEffectCue EffectCueType;

        private void OnValidate()
        {
            AbilityTagCreator.GetAbilityTagNames();
            AbilityAttributeCreator.GetAbilityAttributeNames();
        }

        public bool HasDuration()
        {
            return DurationType != GameplayEffectDuration.Instant && AbilityActivationTag.Name != typeof(NoneTag).FullName;
        }
    }

    [Serializable]
    public class AbilityTagCreator
    {
        [SerializeField]
        public string Name = "None";
        
        //public Type Type;//todo set during creation of instance
        
        [SerializeField]
        public int SelectedIndex;
        
        private static string[] _cachedAbilityTagNames;

        public static string[] GetAbilityTagNames()
        {
            if (_cachedAbilityTagNames?.Length > 0)
            {
                return _cachedAbilityTagNames;
            }

            _cachedAbilityTagNames = AbilitiesUtility.GetInterfaceImplementors(typeof(IAbilityTag));
            return _cachedAbilityTagNames;
        }
        
        public IAbilityTag Create()//TODO create all instances OnAwake to avoid doing it in runtime
        {
            Type type = Type.GetType(Name);
            return (IAbilityTag)Activator.CreateInstance(type ?? throw new InvalidOperationException());
        }
    }

    [Serializable]
    public class AbilityAttributeCreator
    {
        [SerializeField]
        public string Name = "None";
        //public Type Type;//todo set during creation of instance
        [SerializeField]
        public int SelectedIndex;
        
        private static string[] _cachedAttributeNames;
        
        public static string[] GetAbilityAttributeNames()
        {
            if (_cachedAttributeNames?.Length > 0)
            {
                return _cachedAttributeNames;
            }
            return _cachedAttributeNames = AbilitiesUtility.GetDerivedTypeNames(typeof(AbilityAttribute));
        }

        public AbilityAttribute Create()
        {
            Type type = Type.GetType(Name);
            return (AbilityAttribute)Activator.CreateInstance(type ?? throw new InvalidOperationException());
        }
    }
}
