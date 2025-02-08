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
    private TemplateContainer search;

    private TextField _searchBar;

    // Home
    private VisualElement _editProfileButton;
    private VisualElement _homeKeywordsContainer;
    private VisualElement _homeInterestsContainer;

    private VisualElement _historyButton;
    private VisualElement _wishButton;

    // EditProfile
    private VisualElement _prevButtonEditProfile;
    private VisualElement _modalBackground;
    private VisualElement _modalIntroduction;
    private VisualElement _modalKeyword;
    private VisualElement _modalInterest;
    private VisualElement _modalURL;

    private Button _modalIntroductionSubmitButton;
    private Button _modalKeywordSubmitButton;
    private Button _modalInterestSubmitButton;
    private Button _modalURLSubmitButton;
    private VisualElement _editIntroductionButton;
    private Button _addKeywordButton;
    private Button _addInterestButton;
    private Button _addURLButton;

    // History
    private VisualElement _prevButtonHistory;
    // 다음에 개발..

    // Wish
    private VisualElement _prevButtonWish;
    private ScrollView _profileCardsContainer;

    // Search
    private VisualElement _prevButtonSearch;
    
    
    private void OnEnable()
    {
        root = uiDocument.rootVisualElement;
        container = root.Q<VisualElement>("content");
        home = root.Q<TemplateContainer>("home");
        editProfile = root.Q<TemplateContainer>("edit-profile");
        history = root.Q<TemplateContainer>("history");
        wish = root.Q<TemplateContainer>("wish");
        search = root.Q<TemplateContainer>("search");

        Debug.Log("UI Document 연결완료");

        // VisualElement 생성 및 스타일 클래스 추가
        // 초기 화면 너비 출력
        UpdateScreenWidth();

        // 크기 변경 이벤트 등록
        root.RegisterCallback<GeometryChangedEvent>(evt => UpdateScreenWidth());

        _searchBar = root.Q<TextField>("search-bar");
        _searchBar.RegisterCallback<ClickEvent>(evt => ShowSearchScreen());

        // Home
        _editProfileButton = home.Q<VisualElement>("edit-button");
        _homeKeywordsContainer = home.Q<VisualElement>("keyword-chips-container");
        _homeInterestsContainer = home.Q<VisualElement>("interest-chips-container");
        _historyButton = home.Q<VisualElement>("history-button");
        _wishButton = home.Q<VisualElement>("wish-button");

        AddChip("모션그래픽", _homeKeywordsContainer);
        AddChip("3D 디자인", _homeKeywordsContainer);
        AddChip("UX/UI", _homeKeywordsContainer);

        AddChip("데이터/AI", _homeInterestsContainer);
        AddChip("XR", _homeInterestsContainer);
        AddChip("UX/UI", _homeInterestsContainer);

        _editProfileButton.RegisterCallback<ClickEvent>(evt => ShowNextScreen(editProfile));
        _historyButton.RegisterCallback<ClickEvent>(evt => ShowNextScreen(history));
        _wishButton.RegisterCallback<ClickEvent>(evt => ShowNextScreen(wish));

        // EditProfile
        _prevButtonEditProfile = editProfile.Q<VisualElement>("prev-button");
        _prevButtonEditProfile.RegisterCallback<ClickEvent>(evt => ShowHomeScreen(editProfile));

        _modalBackground = editProfile.Q<VisualElement>("modal-background");
        _modalBackground.RegisterCallback<ClickEvent>(evt => CloseModal());
        
        _modalIntroduction = editProfile.Q<VisualElement>("modal-introduction");
        _modalKeyword = editProfile.Q<VisualElement>("modal-keyword");
        _modalInterest = editProfile.Q<VisualElement>("modal-interest");
        _modalURL = editProfile.Q<VisualElement>("modal-url");
        
        var _keywordChip = new ChipsTab(3, 0);
        editProfile.Q<VisualElement>("keyword-chip-container").Add(_keywordChip);
        var _interestChip = new ChipsTab(3, 0);
        editProfile.Q<VisualElement>("interest-chip-container").Add(_interestChip);

        _modalIntroductionSubmitButton = _modalIntroduction.Q<Button>("submit-button");
        _modalIntroductionSubmitButton.RegisterCallback<ClickEvent>(evt => SaveAndCloseModal());
        _modalKeywordSubmitButton = _modalKeyword.Q<Button>("submit-button");
        _modalKeywordSubmitButton.RegisterCallback<ClickEvent>(evt => SaveAndCloseModal());
        _modalInterestSubmitButton = _modalInterest.Q<Button>("submit-button");
        _modalInterestSubmitButton.RegisterCallback<ClickEvent>(evt => SaveAndCloseModal());
        _modalURLSubmitButton = _modalURL.Q<Button>("submit-button");
        _modalURLSubmitButton.RegisterCallback<ClickEvent>(evt => SaveAndCloseModal());

        _editIntroductionButton = editProfile.Q<VisualElement>("edit-button");
        _editIntroductionButton.RegisterCallback<ClickEvent>(evt => OpenModal(0));

        _addKeywordButton = editProfile.Q<Button>("add-keyword-button");
        _addKeywordButton.RegisterCallback<ClickEvent>(evt => OpenModal(1));

        _addInterestButton = editProfile.Q<Button>("add-interest-button");
        _addInterestButton.RegisterCallback<ClickEvent>(evt => OpenModal(2));

        _addURLButton = editProfile.Q<Button>("add-url-button");
        _addURLButton.RegisterCallback<ClickEvent>(evt => OpenModal(3));

        // History
        _prevButtonHistory = history.Q<VisualElement>("prev-button");
        _prevButtonHistory.RegisterCallback<ClickEvent>(evt => ShowHomeScreen(history));

        // Wish
        _prevButtonWish = wish.Q<VisualElement>("prev-button");
        _prevButtonWish.RegisterCallback<ClickEvent>(evt => ShowHomeScreen(wish));

        _profileCardsContainer = wish.Q<ScrollView>("profile-cards-container");
        AddProfileCard("권가경", "기관명 도식", ".", new List<string>(){"디자인", "3D Modeling", "UX/UI"}, new List<string>(){"Game", "Character", "AI"});
        AddProfileCard("권나경", "기관명 도식", ",", new List<string>(){"디자인", "3D Modeling", "UX/UI"}, new List<string>(){"Game", "Character", "AI"});
        // AddLinkCard("하계 학술대회 논문", "www.naver.com");

        // Search
        _prevButtonSearch = search.Q<VisualElement>("prev-button");
        _prevButtonSearch.RegisterCallback<ClickEvent>(evt => ShowHomeScreen(search));
        var _searchChip = new ChipsTab(-1,0);
        _searchChip.style.width = Length.Percent(90);
        search.Q<VisualElement>("content").Add(_searchChip);
    }

    private void UpdateScreenWidth()
    {
        screenWidth = root.resolvedStyle.width;
        Debug.Log($"Updated Screen Width: {screenWidth}px");
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

    private void ShowSearchScreen() // 서치바가 홈에서 움직이지 않을 경우를 대비한 함수
    {
        search.style.display = DisplayStyle.Flex;
        container.style.translate = new Translate(-screenWidth, 0, 0);
    }

    private void OpenModal(int type) // 0: Introduction, 1: Keyword, 2: Interest, 3: URL
    {
        switch(type)
        {
            case 0:
                _modalIntroduction.style.display = DisplayStyle.Flex;
                break;
            case 1:
                _modalKeyword.style.display = DisplayStyle.Flex;
                break;
            case 2:
                _modalInterest.style.display = DisplayStyle.Flex;
                break;
            case 3:
                _modalURL.style.display = DisplayStyle.Flex;
                break;
        }
        
        _modalBackground.style.display = DisplayStyle.Flex;
    }

    private void CloseModal()
    {
        _modalIntroduction.style.display = DisplayStyle.None;
        _modalKeyword.style.display = DisplayStyle.None;
        _modalInterest.style.display = DisplayStyle.None;
        _modalURL.style.display = DisplayStyle.None;
        _modalBackground.style.display = DisplayStyle.None;
    }

    private void SaveAndCloseModal()
    {
        // TODO Save
        CloseModal();
    }

    private void AddProfileCard(string name, string job, string photoURL, List<string> keywords, List<string> interests)
    {
        var profileCard = new ProfileCard(name, job, photoURL, keywords, interests);
        _profileCardsContainer.Add(profileCard);
    }

    private void AddLinkCard(string title, string link)
    {
        var linkCard = new LinkCard(title, link);
        _profileCardsContainer.Add(linkCard);
    }

    private void AddChip(string text, VisualElement container)
    {
        var chip = new SelectableChip(text);
        container.Add(chip);
    }
}
