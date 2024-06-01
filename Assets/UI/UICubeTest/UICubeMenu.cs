using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UICubeMenu : MonoBehaviour
{
    [SerializeField] private UIDocument _document;
    [SerializeField] private StyleSheet _styleSheet;

    public static event Action<float> ScaleChanged;
    public static event Action SpinClicked;
    
    private void Start()
    {
        StartCoroutine(Generate());
    }

    private void OnValidate()
    {
        if(Application.isPlaying) return;
        
        StartCoroutine(Generate());
    }

    IEnumerator Generate()
    {
        yield return null;
        var root = _document.rootVisualElement;
        root.Clear();
        
        root.styleSheets.Add(_styleSheet);

        var container = Create<VisualElement>("container");
        
        var viewBox = Create<VisualElement>("view-box", "bordered-box");
        container.Add(viewBox);

        var controlBox = Create<VisualElement>("control-box", "bordered-box");


        var spinButton = Create<Button>();
        spinButton.text = "Spin";
        spinButton.clicked -= SpinClicked;
        spinButton.clicked += SpinClicked;
        controlBox.Add(spinButton);
        
        var scaleSlider = Create<Slider>();
        scaleSlider.lowValue = 0.5f;
        scaleSlider.highValue = 2f;
        scaleSlider.value = 1f;
        scaleSlider.RegisterValueChangedCallback(v => ScaleChanged?.Invoke(v.newValue));//todo can use event on slider value changed directly
        controlBox.Add(scaleSlider);

        container.Add(controlBox);

        root.Add(container);
    }

    T Create<T>(params string[] ussClassNames) where T : VisualElement, new()
    {
        var elem = new T();
        foreach (var ussClassName in ussClassNames)
        {
            elem.AddToClassList(ussClassName);
        }
        return elem;
    }
}
