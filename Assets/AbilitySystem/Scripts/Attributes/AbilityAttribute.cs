using System;
using UniRx;

namespace AbilitySystem.Scripts.Attributes
{
    [Serializable]
    public abstract class AbilityAttribute//make attribute data
    {
        //impl base and current value
        protected float _baseValue = 100f;
        protected ReactiveProperty<float> _currentValue;
        public ReadOnlyReactiveProperty<float> CurrentValue;

        public AbilityAttribute()
        {
            _currentValue = new ReactiveProperty<float>(_baseValue);
            CurrentValue = _currentValue.ToReadOnlyReactiveProperty();
        }

        public void SetCurrentValue(AttributeModifier attributeModifier)
        {
            var current = _currentValue.Value;
            _currentValue.Value = attributeModifier.Modify(current);
        }

        public float GetCurrentValue()
        {
            return _currentValue.Value;
        }

        public float GetBaseValue()
        {
            return _baseValue;
        }

        public void SetBaseValue(AttributeModifier attributeModifier)
        {
            //when setting base value we must take into account that it may be protected by current buffed value
            var previousBaseValue = _baseValue;
            var newBaseValue = attributeModifier.Modify(_baseValue);
            if (newBaseValue > _currentValue.Value)
            {
                _baseValue = newBaseValue;
                _currentValue.Value = _baseValue;
            }
            else if (newBaseValue < previousBaseValue)
            {
                var delta = previousBaseValue - newBaseValue;
                var previousDifference = _currentValue.Value - previousBaseValue;
                if (delta < previousDifference)//if there was any buff active first check if we can only remove value from buff
                {
                    _currentValue.Value -= delta;
                }
                else//diminish base only by what overflows the previous difference
                {
                   var excessDifference = delta - previousDifference;
                   _baseValue -= excessDifference;
                   _currentValue.Value = _baseValue;
                }
            }
        }

        public void ResetCurrentValue()//todo remember about delta?
        {
            _currentValue.Value = _baseValue;
        }

    }
    
    [Serializable]
    public class HealthAttribute : AbilityAttribute
    {
    }
    
    [Serializable]
    public class StaminaAttribute : AbilityAttribute
    {
        
    }
}