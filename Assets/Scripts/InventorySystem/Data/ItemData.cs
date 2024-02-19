using System;
using AbilitySystem.Scripts.Data;
using UnityEngine;
using UnityEngine.UIElements;

namespace InventorySystem.Data
{
    //todo item flags as consumable/stackable/
    [Flags]
    public enum ItemFlags
    {
        None = 0,
        Consumable = 1 << 0,
        Stackable = 1 << 1,
        Throwable = 1 << 2,
        Weapon = 1 << 3,
        Usable = 1 << 4,
        Pickable = 1 << 5,
    }
    
    [CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/Items", order = 1)]
    public class ItemData : ScriptableObject
    {
        public ItemFlags _flags;
        public GameplayEffectData _gameplayEffect;
        public Image _inventoryIcon;
        public string _name;
    }
}
