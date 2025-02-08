using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;

public class HomeScreenManager : MonoBehaviour
{
    // 0. Common
    [SerializeField] private UIDocument uiDocument;
    
    private VisualElement root;
    private VisualElement home;
    private VisualElement editProfile;
    private VisualElement history;
    private VisualElement wish;

    private Button _editButton;
    private VisualElement _keywordsContainer;
    private VisualElement _interestsContainer;

    private VisualElement _historyButton;
    private VisualElement _wishButton;
    // private ScrollView _profileCardsContainer;
    
    
    private void OnEnable()
    {
        root = uiDocument.rootVisualElement;
        home = root.Q<VisualElement>("home");
        editProfile = root.Q<VisualElement>("edit-profile");
        history = root.Q<VisualElement>("history");
        wish = root.Q<VisualElement>("wish");

        Debug.Log("UI Document 연결완료");

        _editButton = home.Q<Button>("edit-button");
        _keywordsContainer = home.Q<VisualElement>("keyword-chips-container");
        _interestsContainer = home.Q<VisualElement>("interest-chips-container");

        _historyButton = home.Q<VisualElement>("history-button");
        _wishButton = home.Q<VisualElement>("wish-button");
        // _profileCardsContainer = root.Q<ScrollView>("profile-cards-container");
        // AddProfileCard("김김김", "무직");
        // AddProfileCard("이이이", "학생");
        // AddLinkCard("하계 학술대회 논문", "www.naver.com");
    }

    private void ShowNextScreen()
    {
        ;
    }

    // private void AddProfileCard(string name, string job)
    // {
    //     var profileCard = new ProfileCard(name, job);
    //     _profileCardsContainer.Add(profileCard);
    // }

    // private void AddLinkCard(string title, string link)
    // {
    //     var linkCard = new LinkCard(title, link);
    //     _profileCardsContainer.Add(linkCard);
    // }
    
}
