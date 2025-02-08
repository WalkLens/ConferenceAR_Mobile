using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.UIElements;

public class SmallProfileCard : VisualElement
{
    private Label _titleLabel;
    private VisualElement _profilePhoto;
    private VisualElement _status;
    private int status; // 0: offline, 1: online, 2: away

    public SmallProfileCard(string name, string photoURL, int status)
    {
        // UXML 및 USS 불러오기
        var visualTree = Resources.Load<VisualTreeAsset>("UI/VisualTree/SmallProfileCard");
        visualTree.CloneTree(this);
        // styleSheets.Add(Resources.Load<StyleSheet>("Chip"));

        // 요소 참조 가져오기
        _titleLabel = this.Q<Label>("title");
        _profilePhoto = this.Q<VisualElement>("profile-photo");
        _status = this.Q<VisualElement>("status");


        // 텍스트 설정
        _titleLabel.text = name;
        // _profilePhoto.~~background~~ = photoURL;
    }

    public void SetStatus(int status) // 0: offline, 1: online, 2: away
    {
        this.status = status;

        // TODO status 이미지 변경
    }
}