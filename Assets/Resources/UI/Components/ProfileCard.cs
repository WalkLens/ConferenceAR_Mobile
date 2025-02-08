using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.UIElements;

public class ProfileCard : VisualElement
{
    private Label _nameLabel;
    private Label _jobLabel;
    private VisualElement _profilePhoto;
    private VisualElement _keywordsContainer;
    private VisualElement _interestsContainer;
    private Button _wishButton;
    private Button _meetButton;
    private int status; // 0: offline, 1: online, 2: away
    private VisualElement _card;

    public ProfileCard(string name, string job, string photoURL, List<string> keywords, List<string> interests)
    {
        // UXML 및 USS 불러오기
        var visualTree = Resources.Load<VisualTreeAsset>("UI/VisualTree/ProfileCard");
        visualTree.CloneTree(this);
        // styleSheets.Add(Resources.Load<StyleSheet>("Chip"));

        // 요소 참조 가져오기
        _card = this.Q<VisualElement>("card");
        _card.RegisterCallback<ClickEvent>(evt => ShowHMD());
        _nameLabel = this.Q<Label>("name");
        _jobLabel = this.Q<Label>("job");
        _profilePhoto = this.Q<VisualElement>("photo");
        _meetButton = this.Q<Button>("meet-button");
        _meetButton.RegisterCallback<ClickEvent>(evt => Meet());
        
        _keywordsContainer = this.Q<VisualElement>("keywords-container");
        _interestsContainer = this.Q<VisualElement>("interests-container");

        // 텍스트 설정
        _nameLabel.text = name;
        _jobLabel.text = job;
        // Profilephoto URL 설정
        foreach(var keyword in keywords)
        {
            AddChip(keyword, _keywordsContainer);
        }
        foreach(var interest in interests)
        {
            AddChip(interest, _interestsContainer);
        }
    }

    public void SetStatus(int status) // 0: offline, 1: online, 2: away
    {
        this.status = status;

        // TODO status 이미지 변경
    }

    private void AddChip(string text, VisualElement container)
    {
        var chip = new SelectableChip(text);
        container.Add(chip);
    }

    private void ShowHMD()
    {
        ; // AR에 프로필 띄우기
    }

    private void Meet()
    {
        ; // 만나러 가기;
    }
}