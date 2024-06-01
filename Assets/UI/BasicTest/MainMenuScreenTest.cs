using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuScreenTest : MonoBehaviour
{
    [SerializeField] private UIDocument _document;
    [SerializeField] private StyleSheet _styleSheet;

    private void Start()
    {
        Generate();
    }

    private void OnValidate()
    {
        if(Application.isPlaying) return;
        
        Generate();
    }

    void Generate()
    {
        var root = _document.rootVisualElement;
        root.Clear();
        
        root.styleSheets.Add(_styleSheet);

        var titleLabel = new Label("Label says hello!");
        //titleLabel.style.color = new StyleColor(Color.green);//avoid inline styling

        var firstRow = new VisualElement();
        firstRow.AddToClassList("first-row");
        firstRow.Add(titleLabel);

        var secondRow = new VisualElement();
        secondRow.AddToClassList("second-row");

        var aquaBoy = new VisualElement();
        aquaBoy.AddToClassList("aqua-boy");
        
        secondRow.Add(aquaBoy);

        var ultraBoy = new VisualElement();
        ultraBoy.AddToClassList("ultra-boy");
        
        secondRow.Add(ultraBoy);
        
        root.Add(firstRow);
        root.Add(secondRow);
    }
}
