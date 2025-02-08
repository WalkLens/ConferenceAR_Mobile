using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.UIElements;

public class SelectableChip : VisualElement
{
    private VisualElement _chip;
    private Label _label;
    private VisualElement _closeButton;

    public SelectableChip(string text)//, string photoURL, List<string> keywords, List<string> interests)
    {
        // UXML 및 USS 불러오기
        var visualTree = Resources.Load<VisualTreeAsset>("UI/VisualTree/Chip");
        visualTree.CloneTree(this);
        _chip = this.Q<VisualElement>("content");
        // styleSheets.Add(Resources.Load<StyleSheet>("Chip"));

        // 요소 참조 가져오기
        _label = this.Q<Label>("label");
        _closeButton = this.Q<VisualElement>("close-button");
        _closeButton.style.display = DisplayStyle.None;

        // 텍스트 설정
        _label.text = text;
    }

    public void ToggleSelect()
    {
        if(_chip.ClassListContains("tab-button-selected"))
        {
            _chip.RemoveFromClassList("tab-button-selected");
        }
        else
        {
            _chip.AddToClassList("tab-button-selected");
        }
    }
}