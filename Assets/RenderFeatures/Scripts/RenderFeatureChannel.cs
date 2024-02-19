using System.Collections.Generic;
using AbilitySystem.Scripts.Data;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace RenderFeatures.Scripts
{
    [CreateAssetMenu(fileName = "RenderFeatureChannel", menuName = "ScriptableObjects/RenderFeatureChannel", order = 4)]
    public class RenderFeatureChannel : ScriptableObject
    {
        [SerializeField]
        public List<RenderFeatureTagged> TaggedRenderFeatures;

        private readonly Dictionary<string, ScriptableRendererFeature> _tagToRenderFeature = new Dictionary<string, ScriptableRendererFeature>();

        public void ActivateFeature(string abilityTag, bool isActive)
        {
            if (GetFeatures().TryGetValue(abilityTag, out var feature))
            {
                feature.SetActive(isActive);
            }
        }
        
        public bool ActivateFeatureWithIntensityControl(string abilityTag, out IMaterialIntensity intensityControl)
        {
            if (GetFeatures().TryGetValue(abilityTag, out var feature) && feature is IMaterialIntensity control)
            {
                feature.SetActive(true);
                intensityControl = control;
                return true;
            }

            intensityControl = null;
            return false;
        }
        
        private void OnEnable()
        {
            InitializeDictionary();
        }

        private Dictionary<string, ScriptableRendererFeature> InitializeDictionary()
        {
            foreach (var tagged in TaggedRenderFeatures)
            {
                _tagToRenderFeature[tagged.AbilityTag.Name] = tagged.Feature;
            }

            return _tagToRenderFeature;
        }

        Dictionary<string, ScriptableRendererFeature> GetFeatures()
        {
            if (_tagToRenderFeature.Count != TaggedRenderFeatures.Count)
            {
                return InitializeDictionary();
            }

            return _tagToRenderFeature;
        }
    }
    
    [System.Serializable]
    public struct RenderFeatureTagged
    {
        public AbilityTagCreator AbilityTag;
        public ScriptableRendererFeature Feature;
    }
}