using UnityEngine;
using UnityEngine.Events;

namespace InventorySystem
{
    [CreateAssetMenu(fileName = "InventoryChannel", menuName = "ScriptableObjects/InventoryChannel", order = 5)]
    public class InventoryChannel : ScriptableObject
    {
        //todo this can be simplified to just one reactive delegate binding
        public UnityAction<string, int> OnInventoryContentChanged;
        public UnityAction<UnityAction<int>, Component> OnActiveItemChanged;

        public void RiseInventoryContentChanged(string itemName, int index)
        {
            OnInventoryContentChanged?.Invoke(itemName, index);
        }
        
        public void SubscribeToInventoryActiveItemChanged(UnityAction<int> actionIndex, Component subscriber)
        {
            OnActiveItemChanged.Invoke(actionIndex, subscriber);
        }
    }
}