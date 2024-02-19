using System;
using UnityEngine;

namespace UI
{
    public class InteractionPrompt : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        private Camera _mainCamera;

        private void Start()
        {
            _mainCamera = Camera.main;
            _canvas.worldCamera = _mainCamera;
            gameObject.SetActive(false);
        }

        private void LateUpdate()
        {
            var rotation = _mainCamera.transform.rotation;
            transform.LookAt(transform.position + rotation * Vector3.forward, rotation * Vector3.up);
        }
    }
}