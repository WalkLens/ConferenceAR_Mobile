using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.UIElements;

public class LinkCard : VisualElement
{
    private Label _titleLabel;
    private Label _linkLabel;

    public LinkCard(string title, string link)
    {
        // UXML 및 USS 불러오기
        var visualTree = Resources.Load<VisualTreeAsset>("UI/VisualTree/LinkCard");
        visualTree.CloneTree(this);
        // styleSheets.Add(Resources.Load<StyleSheet>("Chip"));

        // 요소 참조 가져오기
        _titleLabel = this.Q<Label>("title");
        _linkLabel = this.Q<Label>("link");


        // 텍스트 설정
        _titleLabel.text = title;
        _linkLabel.text = link;
    }
}