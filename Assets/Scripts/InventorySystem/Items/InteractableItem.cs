using System;
using InventorySystem.Data;
using UI;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace InventorySystem.Items
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class InteractableItem : MonoBehaviour
    {
        [SerializeField] private ItemData _itemData;
        [SerializeField] private BoxCollider _boxCollider;
        [SerializeField] private InteractionPrompt _interactionPrompt;

        private void Awake()
        {
            if (GetComponent<MeshFilter>().mesh == null)
            {
                Debug.LogError("Pickable item must have a mesh representation!");
            }
        }

        void Start()
        {
            _boxCollider.OnTriggerEnterAsObservable().Subscribe(col =>
            {
                if (col.CompareTag("Player") == false)
                {
                    return;
                }

                _interactionPrompt.gameObject.SetActive(true);
                
                if (col.GetComponent<InventorySystem>() is {} inventory)
                {
                    void AddItemAction()
                    {
                        inventory.AddItem((_itemData, gameObject));
                    }

                    inventory.AddItemUnityAction = AddItemAction;
                }
            }).AddTo(this);
            
            _boxCollider.OnTriggerExitAsObservable().Subscribe(col =>
            {
                if (col.CompareTag("Player") == false)
                {
                    return;
                }
                
                if (col.GetComponent<InventorySystem>() is {} inventory)
                {
                    inventory.AddItemUnityAction = null;
                }
                
                _interactionPrompt.gameObject.SetActive(false);

            }).AddTo(this);
        }
    }
    
}