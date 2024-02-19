using System;
using AbilitySystem.Scripts.Data;
using UnityEngine;

namespace AbilitySystem.Scripts.Attributes
{
    [Serializable]
    public class AttributeModifier
    {
        public delegate float ModifyDelegate(float inValue);
        public enum ModifierOp
        {
            Addition,
            Subtraction,
            Multiplication,
            Division
        }

        public enum GameplayEffectDurationOverride
        {
            None,
            InstantPeriodic,
            Instant
        }

        [SerializeField]
        public AbilityAttributeCreator AbilityAttribute;

        [SerializeField]
        public float Magnitude;

        [SerializeField]
        public ModifierOp Operation;

        [SerializeField]
        public GameplayEffectDurationOverride DurationOverride;

        public float Modify(float inValue)
        {
            return Operation switch
            {
                ModifierOp.Addition => inValue + Magnitude,
                ModifierOp.Subtraction => inValue - Magnitude,
                ModifierOp.Multiplication => inValue * Magnitude,
                _ => Magnitude != 0 ? inValue / Magnitude : throw new DivideByZeroException()
            };
        }
    }
}