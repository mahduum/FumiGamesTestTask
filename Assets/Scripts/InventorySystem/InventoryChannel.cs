using UnityEngine;
using UnityEngine.Events;

namespace InventorySystem
{
    [CreateAssetMenu(fileName = "InventoryChannel", menuName = "ScriptableObjects/InventoryChannel", order = 5)]
    public class InventoryChannel : ScriptableObject
    {
        public UnityEvent<string, int> OnInventoryContentChanged;
        public UnityEvent<int> OnActiveItemChanged;
        
        public void RiseInventoryContentChanged(string itemName, int index)
        {
            OnInventoryContentChanged?.Invoke(itemName, index);
        }

        public void RiseInventoryActiveItemChanged(int activeIndex)
        {
            OnActiveItemChanged.Invoke(activeIndex);
        }
    }
}