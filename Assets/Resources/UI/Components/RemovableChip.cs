using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class RemovableChip : VisualElement
{
    private VisualElement _chip;
    private Label _label;
    private VisualElement _closeButton;

    public string text{get{return _label.text;}}

    public RemovableChip(string text)
    {
        // UXML 및 USS 불러오기
        var visualTree = Resources.Load<VisualTreeAsset>("UI/VisualTree/Chip");
        visualTree.CloneTree(this);
        _chip = this.Q<VisualElement>("chip-container");
        // styleSheets.Add(Resources.Load<StyleSheet>("Chip"));

        // 요소 참조 가져오기
        _label = this.Q<Label>("label");
        _closeButton = this.Q<VisualElement>("close-button");
        _closeButton.style.display = DisplayStyle.Flex;

        // 텍스트 설정
        _label.text = text;
    }

    public string RemoveChip()
    {
        this.RemoveFromHierarchy();
        return _label.text;
    }
}