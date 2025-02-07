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
    private ScrollView _profileCardsContainer;
    
    
    private void OnEnable()
    {
        root = uiDocument.rootVisualElement;

        Debug.Log("UI Document 연결완료");

        _profileCardsContainer = root.Q<ScrollView>("profile-cards-container");
        AddProfileCard("김김김", "무직");
        AddProfileCard("이이이", "학생");
        // AddLinkCard("하계 학술대회 논문", "www.naver.com");
    }

    private void AddProfileCard(string name, string job)
    {
        var profileCard = new ProfileCard(name, job);
        _profileCardsContainer.Add(profileCard);
    }

    private void AddLinkCard(string title, string link)
    {
        var linkCard = new LinkCard(title, link);
        _profileCardsContainer.Add(linkCard);
    }
    
}
