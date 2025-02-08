using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;

public class HomeScreenManager : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;

    // Screens    
    private VisualElement root;
    private VisualElement container; // 컨텐츠를 감싸는 컨테이너
    private float screenWidth; // 각 뷰의 너비 (화면 크기)
    private TemplateContainer home;
    private TemplateContainer editProfile;
    private TemplateContainer history;
    private TemplateContainer wish;

    // Home
    private VisualElement _editButton;
    private VisualElement _keywordsContainer;
    private VisualElement _interestsContainer;

    private VisualElement _historyButton;
    private VisualElement _wishButton;
    // private ScrollView _profileCardsContainer;

    // EditProfile
    private VisualElement _prevButtonEditProfile;

    // History
    private VisualElement _prevButtonHistory;

    // Wish
    private VisualElement _prevButtonWish;
    
    
    private void OnEnable()
    {
        root = uiDocument.rootVisualElement;
        container = root.Q<VisualElement>("content");
        home = root.Q<TemplateContainer>("home");
        editProfile = root.Q<TemplateContainer>("edit-profile");
        history = root.Q<TemplateContainer>("history");
        wish = root.Q<TemplateContainer>("wish");

        Debug.Log("UI Document 연결완료");

        // VisualElement 생성 및 스타일 클래스 추가
        // 초기 화면 너비 출력
        UpdateScreenWidth();

        // 크기 변경 이벤트 등록
        root.RegisterCallback<GeometryChangedEvent>(evt => UpdateScreenWidth());

        // Home
        _editButton = home.Q<VisualElement>("edit-button");
        _keywordsContainer = home.Q<VisualElement>("keyword-chips-container");
        _interestsContainer = home.Q<VisualElement>("interest-chips-container");
        _historyButton = home.Q<VisualElement>("history-button");
        _wishButton = home.Q<VisualElement>("wish-button");

        _editButton.RegisterCallback<ClickEvent>(evt => ShowNextScreen(editProfile));
        _historyButton.RegisterCallback<ClickEvent>(evt => ShowNextScreen(history));
        _wishButton.RegisterCallback<ClickEvent>(evt => ShowNextScreen(wish));

        // EditProfile
        _prevButtonEditProfile = editProfile.Q<VisualElement>("prev-button");
        _prevButtonEditProfile.RegisterCallback<ClickEvent>(evt => ShowHomeScreen(editProfile));

        // History
        _prevButtonHistory = history.Q<VisualElement>("prev-button");
        _prevButtonHistory.RegisterCallback<ClickEvent>(evt => ShowHomeScreen(history));

        // Wish
        _prevButtonWish = wish.Q<VisualElement>("prev-button");
        _prevButtonWish.RegisterCallback<ClickEvent>(evt => ShowHomeScreen(wish));

        // _profileCardsContainer = root.Q<ScrollView>("profile-cards-container");
        // AddProfileCard("김김김", "무직");
        // AddProfileCard("이이이", "학생");
        // AddLinkCard("하계 학술대회 논문", "www.naver.com");
    }

    private void ShowNextScreen(VisualElement nextScreen)
    {
        nextScreen.style.display = DisplayStyle.Flex;
        container.style.translate = new Translate(-screenWidth, 0, 0);
    }

    private void ShowHomeScreen(VisualElement currentScreen)
    {
        container.style.translate = new Translate(0, 0, 0);
        currentScreen.style.display = DisplayStyle.None;
    }

    private void UpdateScreenWidth()
    {
        screenWidth = root.resolvedStyle.width;
        Debug.Log($"Updated Screen Width: {screenWidth}px");
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
