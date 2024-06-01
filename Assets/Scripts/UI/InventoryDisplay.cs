using InventorySystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class InventoryDisplay : MonoBehaviour
    {
        [SerializeField] private InventoryChannel _inventoryChannel;
        [SerializeField] private Color _activeItemColor;
        [SerializeField] private Color _inactiveItemColor;
        [SerializeField] private TextMeshProUGUI _firstItemName;
        [SerializeField] private TextMeshProUGUI _secondItemName;
        [SerializeField] private TextMeshProUGUI _thirdItemName;
        [SerializeField] private Image _firstItemImage;
        [SerializeField] private Image _secondItemImage;
        [SerializeField] private Image _thirdItemImage;
        
        
        private void Start()
        {
            _inventoryChannel.SubscribeToInventoryActiveItemChanged(OnInventoryActiveItemChanged, this);
            _inventoryChannel.OnInventoryContentChanged += OnInventoryContentChanged;
            _firstItemImage.color = _inactiveItemColor;
            _secondItemImage.color = _inactiveItemColor;
            _thirdItemImage.color = _inactiveItemColor;
        }

        private void OnInventoryActiveItemChanged(int activeIndex)//todo refactor
        {
            _firstItemImage.color = activeIndex == 0 ? _activeItemColor : _inactiveItemColor;
            _secondItemImage.color = activeIndex == 1 ? _activeItemColor : _inactiveItemColor;
            _thirdItemImage.color = activeIndex == 2 ? _activeItemColor : _inactiveItemColor;
        }

        private void OnInventoryContentChanged(string itemName, int itemIndex)
        {
            var text = itemIndex switch
            {
                0 => _firstItemName,
                1 => _secondItemName,
                _ => _thirdItemName
            };

            text.text = itemName;
        }
    }
}