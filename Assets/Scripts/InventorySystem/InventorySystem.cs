using InventorySystem.Data;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace InventorySystem
{
    [RequireComponent(typeof(CharacterController), typeof(PlayerInput), typeof(AbilitySystem.Scripts.AbilitySystem))]
    public class InventorySystem : MonoBehaviour
    {
        [SerializeField] private InventoryChannel _inventoryChannel;
        
        private const int MaxItems = 3;
        private PlayerInput _playerInput;
        private AbilitySystem.Scripts.AbilitySystem _abilitySystem;
        private (ItemData itemData, GameObject pooled)[] _inventory;
        
        private InputAction _selectFirstItem;
        private InputAction _selectSecondItem;
        private InputAction _selectThirdItem;
        private InputAction _interactAction;

        public UnityAction AddItemUnityAction;

        private readonly ReactiveProperty<int> _activeItemIndex = new ReactiveProperty<int>();
        public ReadOnlyReactiveProperty<int> ActiveItemIndex;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _abilitySystem = GetComponent<AbilitySystem.Scripts.AbilitySystem>();
            _inventory = new(ItemData, GameObject)[MaxItems];
            _interactAction = _playerInput.actions["Interact"];
            _selectFirstItem = _playerInput.actions["FirstItem"];
            _selectSecondItem = _playerInput.actions["SecondItem"];
            _selectThirdItem = _playerInput.actions["ThirdItem"];

            _activeItemIndex.Value = -1;
            ActiveItemIndex = new ReadOnlyReactiveProperty<int>(_activeItemIndex);
            
            _activeItemIndex.Subscribe(activeIndex =>
            {
                _inventoryChannel.RiseInventoryActiveItemChanged(activeIndex);
            }).AddTo(this);
        }

        private void OnDestroy()
        {
            _inventoryChannel.OnInventoryContentChanged.RemoveAllListeners();
            _inventoryChannel.OnActiveItemChanged.RemoveAllListeners();
        }

        private void OnEnable()//todo refactor bindings
        {
            _selectFirstItem.performed += SelectFirstItem;
            _selectSecondItem.performed += SelectSecondItem;
            _selectThirdItem.performed += SelectThirdItem;
            _interactAction.performed += Interact;
        }
        
        private void OnDisable()
        {
            _selectFirstItem.performed -= SelectFirstItem;
            _selectSecondItem.performed -= SelectSecondItem;
            _selectThirdItem.performed -= SelectThirdItem;
            _interactAction.performed -= Interact;
        }

        void SelectFirstItem(InputAction.CallbackContext context) => _activeItemIndex.Value = 0;
        void SelectSecondItem(InputAction.CallbackContext context) => _activeItemIndex.Value = 1;
        void SelectThirdItem(InputAction.CallbackContext context) => _activeItemIndex.Value = 2;

        void Interact(InputAction.CallbackContext context)
        {
            if (AddItemUnityAction != null)
            {
                AddItemUnityAction.Invoke();
                AddItemUnityAction = null;
            }
            else
            {
                UseItem();
            }
        }

        [UsedImplicitly]
        public void AddItem((ItemData, GameObject) item)
        {
            for (var index = 0; index < _inventory.Length; index++)
            {
                ref var slot = ref _inventory[index];
                if (slot == (null, null))
                {
                    slot = (item.Item1, item.Item2);
                    item.Item2.SetActive(false);
                    _activeItemIndex.Value = index;
                    _inventoryChannel.RiseInventoryContentChanged(item.Item1._name, index);

                    return;
                }
            }

            var spawnPosition = item.Item2.transform.position;

            var swappedItem = _inventory[_activeItemIndex.Value].pooled;
            item.Item2.SetActive(false);
            swappedItem.transform.position = spawnPosition;
            swappedItem.SetActive(true);

            _inventory[_activeItemIndex.Value] = (item.Item1, item.Item2);
            _inventoryChannel.RiseInventoryContentChanged(item.Item1._name, _activeItemIndex.Value);
        }

        void UseItem()
        {
            var itemData = _inventory[_activeItemIndex.Value].itemData;
            if (itemData == null)
            {
                return;
            }
            
            _abilitySystem.ApplyGameplayEffect(itemData._gameplayEffect);
            
            Destroy(_inventory[_activeItemIndex.Value].pooled);//todo refactor to use new pool api
            _inventory[_activeItemIndex.Value] = (null, null);

            int firstValidIndex = -1;
            for (int i = 0; i < _inventory.Length; i++)
            {
                if (_inventory[i].itemData != null)//todo change null to "is valid" or something like that
                {
                    firstValidIndex = i;
                    break;
                }
            }
            
            _inventoryChannel.RiseInventoryContentChanged("", _activeItemIndex.Value);//todo this can also be set to container changed then it wouldn't need to be called explicitly
            _activeItemIndex.Value = firstValidIndex;
        }
    }
}