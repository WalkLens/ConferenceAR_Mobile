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
    private UserData profileData;

    public ProfileCard(UserData userData)
    {
        this.profileData = userData;
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
        _meetButton.RegisterCallback<ClickEvent>(evt => {Meet(); evt.StopPropagation();});
        
        _keywordsContainer = this.Q<VisualElement>("keywords-container");
        _interestsContainer = this.Q<VisualElement>("interests-container");

        // 텍스트 설정
        _nameLabel.text = userData.name;
        _jobLabel.text = userData.job;
        // Profilephoto URL 설정

        AddChip(userData.introduction_1, _keywordsContainer);
        AddChip(userData.introduction_2, _keywordsContainer);
        AddChip(userData.introduction_3, _keywordsContainer);

        AddChip(userData.interest_1, _interestsContainer);
        AddChip(userData.interest_2, _interestsContainer);
        AddChip(userData.interest_3, _interestsContainer);
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
        Debug.Log(this.profileData.pin); // AR에 프로필 띄우기
    }

    private void Meet()
    {
        Debug.Log(this.profileData.pin); // 만나러 가기;
    }
}