using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WSHealthBar : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private VisualTreeAsset _healthBarAsset;
    [SerializeField] private UIDocument _uiDocument;
    private VisualElement _root;
    private VisualElement _healthBar;
    void Start()
    {
        _root = _uiDocument.rootVisualElement;
        _healthBar = _healthBarAsset.Instantiate();
        _root.Add(_healthBar);
    }

    // Update is called once per frame
    void Update()
    {
        var screenPos = Camera.main.WorldToScreenPoint(transform.position);
        _healthBar.style.left = screenPos.x - (_healthBar.layout.width / 2f);
        _healthBar.style.top = (Screen.height - screenPos.y) - 50;
    }
}
