using System;
using System.Collections.Generic;
using AbilitySystem.Scripts.Abilities;
using AbilitySystem.Scripts.AttributeListeners;
using AbilitySystem.Scripts.Attributes;
using AbilitySystem.Scripts.Data;
using AbilitySystem.Scripts.GameplayEffects;
using RenderFeatures.Scripts;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

namespace AbilitySystem.Scripts
{
    public class AbilitySystem : MonoBehaviour
    {
        [SerializeField]
        private AbilityAttributesChannel _abilityAttributesChannel;

        [SerializeField]
        private RenderFeatureChannel _renderFeatureChannel;
        
        private readonly HashSet<AttributeSet> _registeredAttributeSets = new HashSet<AttributeSet>();
        private readonly Dictionary<string, IAbilityTag> _ownedAbilityTags = new Dictionary<string, IAbilityTag>();
        
        private readonly Dictionary<string, BoolReactiveProperty> _activeGameEffects = new Dictionary<string, BoolReactiveProperty>();
        private readonly Dictionary<string, CompositeDisposable> _disposables = new Dictionary<string, CompositeDisposable>();

        private readonly Dictionary<string, ReadOnlyReactiveProperty<float>> _attributeEvents = new Dictionary<string, ReadOnlyReactiveProperty<float>>();

        private void Awake()
        {
            _abilityAttributesChannel.AddSubscriptionHandler(SubscribeToAttributeCurrentValueChanged);
        }

        private void OnDestroy()
        {
            _abilityAttributesChannel.RemoveSubscriptionHandler(SubscribeToAttributeCurrentValueChanged);
        }

        private void SubscribeToAttributeCurrentValueChanged(string attributeName, UnityAction<float> processAttributeEvent, Component subscriber)
        {
            if (_attributeEvents.TryGetValue(attributeName, out var attributeEvent))
            {
                attributeEvent.Subscribe(currentValue =>
                {
                    processAttributeEvent?.Invoke(currentValue);
                }).AddTo(subscriber);
            }
        }

        public void RegisterAttributeSet(AttributeSet attributeSet)
        {
            bool registered = _registeredAttributeSets.Add(attributeSet);

            if (registered == false)
            {
                Debug.LogError($"Cannot register two identical sets with {nameof(AbilitySystem)}");
                return;
            }

            foreach (var set in _registeredAttributeSets)
            {
                foreach (var attr in set.GetAllAbilityAttributes())
                {
                    var fullName = attr.GetType().FullName;//todo refactor so we deal with types not strings, but its only registered once here at the beginning so small penalty
                    if (fullName != null) _attributeEvents.Add(fullName, attr.CurrentValue);
                }
            }
        }

        public void RemoveAbilityTag(string abilityTag)
        {
            if (_activeGameEffects.TryGetValue(abilityTag, out var isActive))
            {
                isActive.Value = false;
            }
            
            _ownedAbilityTags.Remove(abilityTag);
        }

        public void ApplyGameplayEffect(GameplayEffectData gameplayEffectData)
        {
            if (gameplayEffectData.RequiredAbilityTags.Count > 0)
            {
                foreach (var required in gameplayEffectData.RequiredAbilityTags)
                {
                    if (_ownedAbilityTags.ContainsKey(required.Name) == false)
                    {
                        return;
                    }
                }
            }
            
            if (gameplayEffectData.AbilityTagsToRemove.Count > 0)
            {
                foreach (var tagToRemove in gameplayEffectData.AbilityTagsToRemove)
                {
                    RemoveAbilityTag(tagToRemove.Name);
                }
            }

            BoolReactiveProperty isActive = null;
            if (gameplayEffectData.AbilityActivationTag is {} activationTag &&
                activationTag.Name != typeof(NoneTag).FullName)
            {
                if (_ownedAbilityTags.ContainsKey(activationTag.Name))
                {
                    //for now assume that if tag is applied and this implies that effect has duration
                    //it is not possible to apply the same effect twice util the previous is still active
                    return;
                }
                
                _ownedAbilityTags.Add(activationTag.Name, activationTag.Create());
                
                isActive = SetEffectActiveForTag(activationTag);
                
                //todo we need to have info on what effect, or effect in time is to be applied, like periodic effect
                ActivatePostProcessingEffect(gameplayEffectData, activationTag, isActive);
                //call other types of effects todo refactor based on tag effect type, maybe put info about effect type inside tag?
                            
                if (gameplayEffectData.DurationType == GameplayEffectDuration.Duration &&
                    gameplayEffectData.HasDuration())
                {
                    ScheduleRemoveAbilityTag(activationTag.Name, gameplayEffectData.Duration);
                }
            }

            UpdateAttributeValues(gameplayEffectData, isActive);
        }

        private void ScheduleRemoveAbilityTag(string tagName, double duration)
        {
            var disposable = Observable
                .Timer(TimeSpan.FromSeconds(duration))
                .Subscribe(_ => { },
                    () =>
                    {
                        RemoveAbilityTag(tagName);
                    }).AddTo(this);
            GetCompositeForTag(tagName).Add(disposable);
        }
        
        private void UpdateAttributeValues(GameplayEffectData gameplayEffectData, BoolReactiveProperty isActive)
        {
            if (gameplayEffectData.ModifierInfos.Count == 0)
            {
                return;
            }
            
            foreach (AttributeSet set in _registeredAttributeSets)
            {
                foreach (var info in gameplayEffectData.ModifierInfos)
                {
                    if (set.TryGetAbilityAttribute(info.AbilityAttribute.Name, out var found))
                    {
                        //if there are no tags, the effect must be instant and irreversible:
                        if (gameplayEffectData.DurationType == GameplayEffectDuration.Instant ||
                            info.DurationOverride == AttributeModifier.GameplayEffectDurationOverride.Instant)
                        {
                            found.SetBaseValue(info);
                            continue;
                        }

                        IDisposable disposable = null;
                        if (isActive == null)
                        {
                            Debug.LogError("Ability tag reactive property should not be null at this point!");
                            return;
                        }
                        //if is periodic is not reversible, maybe I need only bool here?
                        if (info.DurationOverride == AttributeModifier.GameplayEffectDurationOverride.InstantPeriodic)
                        {
                            disposable =
                                SetAttributeBaseValuePeriodically(found, info, gameplayEffectData.Duration, isActive);
                            GetCompositeForTag(gameplayEffectData.AbilityActivationTag).Add(disposable);
                            continue;
                        }
                        
                        found.SetCurrentValue(info);
                        disposable = isActive
                            .SkipWhile(val => val)
                            .TakeUntil(isActive.Where(val => !val))
                            .Subscribe(val =>
                        {
                            if (val == false)
                            {
                                found.ResetCurrentValue();
                            }
                        },
                            () =>
                            {
                                found.ResetCurrentValue();
                            }).AddTo(this);
                        
                        GetCompositeForTag(gameplayEffectData.AbilityActivationTag).Add(disposable);
                    }
                }
            }
        }
        
        private IDisposable SetAttributeBaseValuePeriodically(AbilityAttribute attribute, AttributeModifier attributeModifier, double duration,
            BoolReactiveProperty isActive)
        {
            var disposable = Observable.Interval(TimeSpan.FromSeconds(1))
                .TakeUntil(isActive.Where(value => !value))
                .Subscribe(_ =>
                    {
                        attribute.SetBaseValue(attributeModifier);
                    },
                    () =>
                    {
                    }).AddTo(this);
            return disposable;
        }
        
        private void ActivatePostProcessingEffect(GameplayEffectData gameplayEffectData, AbilityTagCreator activationTag, BoolReactiveProperty isActive)
        {
            IMaterialIntensity control = null;
            if (gameplayEffectData.HasDuration() &&
                _renderFeatureChannel.ActivateFeatureWithIntensityControl(activationTag.Name, out control) == false)
            {
                return;
            }
           
            var increaseIntensityOverTime = 
                SimpleBlendInPostProcessMaterial(control, Time.timeSinceLevelLoad);

            var effectCountDown = SetEffectCountDown(gameplayEffectData, control, isActive, activationTag);

            var disposable = GetCompositeForTag(activationTag);
      
            disposable.Add(effectCountDown);
            disposable.Add(increaseIntensityOverTime);
        }

        private BoolReactiveProperty SetEffectActiveForTag(AbilityTagCreator granted)
        {
            if (_activeGameEffects.TryGetValue(granted.Name, out var isActive) == false)
            {
                isActive = new BoolReactiveProperty(true);
                _activeGameEffects.Add(granted.Name, isActive);
            }
            else
            {
                isActive.Value = true;
            }

            return isActive;
        }

        private IDisposable SetEffectCountDown(GameplayEffectData gameplayEffectData, IMaterialIntensity control,
            BoolReactiveProperty isActive, AbilityTagCreator activationTag)
        {
            var effectCountDown = Observable.Timer(TimeSpan.FromSeconds(gameplayEffectData.Duration))
                .TakeUntil(isActive.Where(value => !value))
                .Subscribe(_ => { },
                    () =>
                    {
                        var decreaseIntensityOverTime =
                            SimpleBlendOutPostProcessMaterial(control, Time.timeSinceLevelLoad, activationTag);
                        
                        GetCompositeForTag(activationTag).Add(decreaseIntensityOverTime);
                    }).AddTo(this);
            return effectCountDown;
        }

        private CompositeDisposable GetCompositeForTag(AbilityTagCreator abilityTag)
        {
            return GetCompositeForTag(abilityTag.Name);
        }

        private CompositeDisposable GetCompositeForTag(string abilityTagName)
        {
            if (_disposables.TryGetValue(abilityTagName, out var disposable) == false)
            {
                var composite = new CompositeDisposable();
                _disposables[abilityTagName] = composite;
                disposable = composite;
            }

            return disposable;
        }
        
        private IDisposable SimpleBlendInPostProcessMaterial(IMaterialIntensity control, float startTime)
        {
            var intensityTimer = Observable.Timer(TimeSpan.FromSeconds(2));

            var increaseIntensity = Observable.EveryUpdate();
            
            var increaseIntensityOverTime =
                increaseIntensity.TakeUntil(intensityTimer).Subscribe(_ =>
                    {
                        var time = (Time.timeSinceLevelLoad - startTime) / 2;
                        control.SetIntensity(time);
                    })
                    .AddTo(this);
            return increaseIntensityOverTime;
        }
        
        private IDisposable SimpleBlendOutPostProcessMaterial(IMaterialIntensity control, float startTime, AbilityTagCreator effectTag)
        {
            var intensityTimer = Observable.Timer(TimeSpan.FromSeconds(2));

            var decreaseIntensity = Observable.EveryUpdate();
            var decreaseIntensityOverTime =
                decreaseIntensity.TakeUntil(intensityTimer).Subscribe(_ =>
                    {
                        var time = (Time.timeSinceLevelLoad - startTime) / 2;
                        control.SetIntensity(Mathf.Clamp01(1f - time));
                    }, () =>
                    {
                        _renderFeatureChannel.ActivateFeature(effectTag.Name, false);
                    })
                    .AddTo(this);
            
            return decreaseIntensityOverTime;
        }
    }
}