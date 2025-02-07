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

    public ProfileCard(string name, string job)//, string photoURL, List<string> keywords, List<string> interests)
    {
        // UXML 및 USS 불러오기
        var visualTree = Resources.Load<VisualTreeAsset>("UI/VisualTree/ProfileCard");
        visualTree.CloneTree(this);
        // styleSheets.Add(Resources.Load<StyleSheet>("Chip"));

        // 요소 참조 가져오기
        _nameLabel = this.Q<Label>("name");
        _jobLabel = this.Q<Label>("job");
        _profilePhoto = this.Q<VisualElement>("photo");
        
        _keywordsContainer = this.Q<VisualElement>("keywords_container");
        _interestsContainer = this.Q<VisualElement>("interests_container");

        // 텍스트 설정
        _nameLabel.text = name;
        _jobLabel.text = job;
        // Profilephoto URL 설정
        // Container에 추가
    }
}