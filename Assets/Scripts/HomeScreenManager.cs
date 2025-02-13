using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;

public class HomeScreenManager : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private GameObject shareScreen;

    private UserData playerUserData;
    private UserDataList fullUserDataList;
    private UserDataList wishList;

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
    private VisualElement _homePhoto;
    private VisualElement _editProfileButton;
    private Label _homeName;
    private Label _homeIntroduction;
    private VisualElement _homeKeywordsContainer;
    private VisualElement _homeInterestsContainer;

    private VisualElement _historyButton;
    private VisualElement _wishButton;

    // EditProfile
    private List<string> SelectedIntroductionString = new List<string>();
    private List<string> SelectedInterestString = new List<string>();
    private VisualElement _prevButtonEditProfile;
    private VisualElement _modalBackground;
    private VisualElement _modalIntroduction;
    private VisualElement _modalKeyword;
    private VisualElement _modalInterest;
    private VisualElement _modalURL;

    private TextField _modalIntroductionTextField;
    private TextField _modalURLTextField;

    private Button _modalIntroductionSubmitButton;
    private Button _modalKeywordSubmitButton;
    private Button _modalInterestSubmitButton;
    private Button _modalURLSubmitButton;
    private VisualElement _editIntroductionButton;
    private Button _addKeywordButton;
    private Button _addInterestButton;
    private Button _addURLButton;
    private VisualElement _editKeywordChipContainer;
    private VisualElement _editInterestChipContainer;
    private VisualElement _editURLChipContainer;

    private Label _editProfileName;
    private Label _editProfileJob;
    private Label _editProfileIntroduction;
    
    // History
    private VisualElement _prevButtonHistory;
    private VisualElement _matchHistoryContainer;
    // 다음에 개발..

    // Wish
    private VisualElement _prevButtonWish;
    private ScrollView _profileCardsContainer;

    // Search
    private VisualElement _prevButtonSearch;
    private TextField _searchBarTextField;
    private ScrollView _searchResultsContainer;
    private VisualElement _searchBackground;
    private Button _searchSubmitKeywordOnly;
    private VisualElement _searchChipsTab;

    private UserData dummyUserData = new UserData
    {
        pin = "12345",
        name = "권가경",
        job = "기관명 도식",
        language = "ko-KR",
        introduction_1 = "디자인",
        introduction_2 = "3D Modeling",
        introduction_3 = "UX/UI",
        introduction_4 = "",
        introduction_5 = "",
        // interest_1 = selectedInterests.Count > 0 ? selectedInterests[0] : "", // TODO 최대 3개로 구조 수정
        // interest_2 = selectedInterests.Count > 1 ? selectedInterests[1] : "",
        // interest_3 = selectedInterests.Count > 2 ? selectedInterests[2] : "",
        interest_1 = "Game",
        interest_2 = "Character",
        interest_3 = "AI",
        interest_4 = "",
        interest_5 = "",
        introduction_text = "Hello",
        url = "",
        autoaccept = true
    };
    
    
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

        playerUserData = DatabaseManager.Instance.playerUserData;
        Debug.Log("DB 연결 완료");

        // VisualElement 생성 및 스타일 클래스 추가
        // 초기 화면 너비 출력
        UpdateScreenWidth();

        // 크기 변경 이벤트 등록
        root.RegisterCallback<GeometryChangedEvent>(evt => UpdateScreenWidth());

        _searchBar = root.Q<TextField>("search-bar");
        _searchBar.RegisterCallback<ClickEvent>(evt => ShowSearchScreen());

        // Home
        // UI
        _editProfileButton = home.Q<VisualElement>("edit-button");
        _homeKeywordsContainer = home.Q<VisualElement>("keyword-chips-container");
        _homeInterestsContainer = home.Q<VisualElement>("interest-chips-container");
        _historyButton = home.Q<VisualElement>("history-button");
        _wishButton = home.Q<VisualElement>("wish-button");

        _homePhoto = home.Q<VisualElement>("photo");
        _homePhoto.RegisterCallback<ClickEvent>(evt => ShowShareScreen());
        _homeName = home.Q<Label>("Name");
        _homeIntroduction = home.Q<Label>("Introduction");

        _editProfileButton.RegisterCallback<ClickEvent>(evt => ShowNextScreen(editProfile));
        _historyButton.RegisterCallback<ClickEvent>(evt => ShowNextScreen(history));
        _wishButton.RegisterCallback<ClickEvent>(evt => ShowNextScreen(wish));
        _wishButton.RegisterCallback<ClickEvent>(evt => UpdateWishCards());

        SelectedIntroductionString.Add(playerUserData.introduction_1);
        if(playerUserData.introduction_2 != "")
        {
            SelectedIntroductionString.Add(playerUserData.introduction_2);
        }
        if(playerUserData.introduction_3 != "")
        {
            SelectedIntroductionString.Add(playerUserData.introduction_3);
        }

        SelectedInterestString.Add(playerUserData.interest_1);
        if(playerUserData.interest_2 != "")
        {
            SelectedInterestString.Add(playerUserData.interest_2);
        }
        if(playerUserData.interest_3 != "")
        {
            SelectedInterestString.Add(playerUserData.interest_3);
        }

        UpdateHomeScreen();

        // EditProfile
        // UI
        _prevButtonEditProfile = editProfile.Q<VisualElement>("prev-button");
        _prevButtonEditProfile.RegisterCallback<ClickEvent>(evt => ShowHomeScreen(editProfile));

        _modalBackground = editProfile.Q<VisualElement>("modal-background");
        _modalBackground.RegisterCallback<ClickEvent>(evt => CloseModal());
        
        _modalIntroduction = editProfile.Q<VisualElement>("modal-introduction");
        _modalKeyword = editProfile.Q<VisualElement>("modal-keyword");
        _modalInterest = editProfile.Q<VisualElement>("modal-interest");
        _modalURL = editProfile.Q<VisualElement>("modal-url");
        
        var _keywordChip = new ChipsTab(3, 0);
        editProfile.Q<VisualElement>("modal-keyword-chip-container").Add(_keywordChip);
        var _interestChip = new ChipsTab(3, 0);
        editProfile.Q<VisualElement>("modal-interest-chip-container").Add(_interestChip);

        _editProfileName = editProfile.Q<Label>("Name");
        _editProfileJob = editProfile.Q<Label>("Introduction");
        _editProfileIntroduction = editProfile.Q<Label>("introduction-label");

        _modalIntroductionTextField = editProfile.Q<TextField>("introduction-textfield");
        _modalURLTextField = editProfile.Q<TextField>("url-textfield");
        _modalIntroductionTextField.value = playerUserData.introduction_text;
        _modalURLTextField.value = playerUserData.url;

        _modalIntroductionSubmitButton = _modalIntroduction.Q<Button>("submit-button");
        _modalIntroductionSubmitButton.RegisterCallback<ClickEvent>(evt => SaveAndCloseModal("introduction"));
        _modalKeywordSubmitButton = _modalKeyword.Q<Button>("submit-button");
        _modalKeywordSubmitButton.RegisterCallback<ClickEvent>(evt => SaveAndCloseModal("keyword"));
        _modalInterestSubmitButton = _modalInterest.Q<Button>("submit-button");
        _modalInterestSubmitButton.RegisterCallback<ClickEvent>(evt => SaveAndCloseModal("interest"));
        _modalURLSubmitButton = _modalURL.Q<Button>("submit-button");
        _modalURLSubmitButton.RegisterCallback<ClickEvent>(evt => SaveAndCloseModal("url"));

        _editIntroductionButton = editProfile.Q<VisualElement>("edit-button");
        _editIntroductionButton.RegisterCallback<ClickEvent>(evt => OpenModal(0));

        _addKeywordButton = editProfile.Q<Button>("add-keyword-button");
        _addKeywordButton.RegisterCallback<ClickEvent>(evt => OpenModal(1));
        _editKeywordChipContainer = editProfile.Q<VisualElement>("keyword-chip-container");

        _addInterestButton = editProfile.Q<Button>("add-interest-button");
        _addInterestButton.RegisterCallback<ClickEvent>(evt => OpenModal(2));
        _editInterestChipContainer = editProfile.Q<VisualElement>("interest-chip-container");

        _addURLButton = editProfile.Q<Button>("add-url-button");
        _addURLButton.RegisterCallback<ClickEvent>(evt => OpenModal(3));
        _editURLChipContainer = editProfile.Q<VisualElement>("url-chip-container");

        // DB
        UpdateEditProfileScreen();

        // History
        _prevButtonHistory = history.Q<VisualElement>("prev-button");
        _prevButtonHistory.RegisterCallback<ClickEvent>(evt => ShowHomeScreen(history));
        _matchHistoryContainer = history.Q<VisualElement>("match-history-container");
        AddMatchHistoryCards();

        // Wish
        _prevButtonWish = wish.Q<VisualElement>("prev-button");
        _prevButtonWish.RegisterCallback<ClickEvent>(evt => ShowHomeScreen(wish));

        _profileCardsContainer = wish.Q<ScrollView>("profile-cards-container");
        UpdateWishCards();
        // AddLinkCard("하계 학술대회 논문", "www.naver.com");

        // Search
        _prevButtonSearch = search.Q<VisualElement>("prev-button");
        _prevButtonSearch.RegisterCallback<ClickEvent>(evt => OnSearchBack());
        _searchBackground = search.Q<VisualElement>("background");
        _searchBarTextField = search.Q<TextField>("search-bar");
        _searchBarTextField.RegisterValueChangedCallback(evt => UpdateSearchCards(evt.newValue));
        _searchBarTextField.RegisterCallback<FocusEvent>(evt => {_searchBackground.style.visibility = Visibility.Visible; _searchBackground.style.opacity = 1;});
        _searchBarTextField.RegisterCallback<BlurEvent>(evt => {_searchBackground.style.visibility = Visibility.Hidden; _searchBackground.style.opacity = 0;});
        _searchResultsContainer = search.Q<ScrollView>("profile-cards-container");
        _searchSubmitKeywordOnly = search.Q<Button>("submit-keyword-only");
        _searchSubmitKeywordOnly.RegisterCallback<ClickEvent>(evt => OnSubmitKeywordsOnly());

        var _searchChip = new ChipsTab(-1,0);
        _searchChip.style.width = Length.Percent(90);
        _searchChipsTab = search.Q<VisualElement>("search-chips-tab");
        _searchChipsTab.Add(_searchChip); 
        _searchChipsTab.RegisterCallback<ClickEvent>(evt => OnFilterSelected());
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
        UpdateUserList();
    }

    private void OnSearchBack()
    {
        if(_searchChipsTab.style.visibility == Visibility.Hidden)
        {
            _searchBarTextField.value = "";
            _searchResultsContainer.style.visibility = Visibility.Hidden;
            _searchChipsTab.style.visibility = Visibility.Visible;
        }
        else
        {
            ShowHomeScreen(search);
        }
    }

    private void OpenModal(int type) // 0: Introduction, 1: Keyword, 2: Interest, 3: URL, OpenModal시 값 업데이트
    {
        switch(type)
        {
            case 0:
                _modalIntroduction.RemoveFromClassList("modal-inactive");
                break;
            case 1:
                _modalKeyword.Q<ChipsTab>().SelectChipsByName(SelectedIntroductionString);
                _modalKeyword.RemoveFromClassList("modal-inactive");
                break;
            case 2:
                _modalInterest.Q<ChipsTab>().SelectChipsByName(SelectedInterestString);
                _modalInterest.RemoveFromClassList("modal-inactive");
                break;
            case 3:
                _modalURL.RemoveFromClassList("modal-inactive");
                break;
        }
        
        _modalBackground.style.display = DisplayStyle.Flex;
    }

    private void CloseModal()
    {
        _modalIntroduction.AddToClassList("modal-inactive");
        _modalKeyword.AddToClassList("modal-inactive");
        _modalInterest.AddToClassList("modal-inactive");
        _modalURL.AddToClassList("modal-inactive");
        _modalBackground.style.display = DisplayStyle.None;
    }

    private void SaveAndCloseModal(string type) // TODO UserData에 저장
    {
        if(type == "introduction")
        {
            playerUserData.introduction_text = _modalIntroductionTextField.text;
        }
        else if(type == "keyword")
        {
            var selection = _modalKeyword.Q<ChipsTab>().SelectedKeywords;
            // 리스트에 먼저 저장한 후 삭제
            SelectedIntroductionString = selection;
            playerUserData.introduction_1 = selection[0];
            playerUserData.introduction_2 = selection[1];
            playerUserData.introduction_3 = selection[2];
        }
        else if(type == "interest")
        {
            var selection = _modalInterest.Q<ChipsTab>().SelectedKeywords;
            // 리스트에 먼저 저장한 후 삭제
            SelectedInterestString = selection;
            playerUserData.interest_1 = selection[0];
            playerUserData.interest_2 = selection[1];
            playerUserData.interest_3 = selection[2];
        }
        else if(type == "url")
        {
            playerUserData.url = _modalURLTextField.text;
        }

        DatabaseManager.Instance.editProfile(playerUserData.pin, playerUserData);
        UpdateHomeScreen();
        UpdateEditProfileScreen();
        CloseModal();
    }

    private void UpdateUserList()
    {
        fullUserDataList = DatabaseManager.Instance.getAllUserData();
        wishList = DatabaseManager.Instance.getWishList(playerUserData.pin);
    }

    private void AddWishProfileCard(UserData userData)
    {
        // string name, string job, string photoURL, List<string> keywords, List<string> interests
        var profileCard = new ProfileCard(userData, isInWish: true, alwaysShowWish: true);
        profileCard._wishButton.RegisterCallback<ClickEvent>(evt => RemoveWishProfileCard(profileCard));
        _profileCardsContainer.Add(profileCard);
    }

    private void RemoveWishProfileCard(ProfileCard card)
    {
        card.RemoveFromHierarchy();
    }

    private void AddSmallProfileCard(UserData userData)
    {
        // string name, string job, string photoURL, List<string> keywords, List<string> interests
        var profileCard = new SmallProfileCard(userData);
        profileCard._closeButton.RegisterCallback<ClickEvent>(evt => RemoveSmallProfileCard(profileCard));
        _matchHistoryContainer.Add(profileCard);
    }

    private void RemoveSmallProfileCard(SmallProfileCard card)
    {
        card.RemoveFromHierarchy();
        DatabaseManager.Instance.removeHistory(card.profileData.pin);
    }

    private void AddSearchResultCard(UserData userData)
    {
        bool isInWish = false;
        foreach(var user in wishList.users)
        {
            if(userData.pin == user.pin)
            {
                isInWish = true;
                break;
            }
        }
        var resultCard = new ProfileCard(userData, isInWish);
        _searchResultsContainer.Add(resultCard);
    }

    private void AddProfileCards() // TODO 받아오도록
    {
        UserDataList userDataList = DatabaseManager.Instance.getAllUserData();
        foreach(var userData in userDataList.users)
        {
            AddWishProfileCard(userData);
        }
    }

    private void UpdateWishCards()
    {
        UserDataList userDataList = DatabaseManager.Instance.getWishList(playerUserData.pin);
        _profileCardsContainer.Clear();
        foreach(var userData in userDataList.users)
        {
            AddWishProfileCard(userData);
        }
    }

    private void AddMatchHistoryCards()
    {
        UserDataList userDataList = DatabaseManager.Instance.getMatchHistory(playerUserData.pin);
        foreach(var userData in userDataList.users)
        {
            AddSmallProfileCard(userData);
        }
    }

    private void UpdateSearchCards(string keyword)
    {
        UserDataList userDataList = DatabaseManager.Instance.Search(fullUserDataList, keyword, search.Q<ChipsTab>("").SelectedKeywords);
        _searchResultsContainer.Clear();

        if(userDataList.users.Count == 0)
        {
            _searchResultsContainer.style.visibility = Visibility.Hidden;
            ToggleSearchChipsTab();
        }
        else
        {
            _searchResultsContainer.style.visibility = Visibility.Visible;
            ToggleSearchChipsTab();
        }

        foreach(var userData in userDataList.users)
        {
            AddSearchResultCard(userData);
        }
    }

    private void AddChip(string text, VisualElement container)
    {
        var chip = new SelectableChip(text);
        container.Add(chip);
    }

    private RemovableChip AddRemovableChip(string text, VisualElement container, List<string> keywords=null)
    {
        var chip = new RemovableChip(text);
        if(keywords != null)
        {
            chip.RegisterCallback<ClickEvent>(evt => {keywords.Remove(chip.text);UpdateInterestKeyword();UpdateIntroductionKeyword();});
        }
        chip.RegisterCallback<ClickEvent>(evt => {chip.RemoveChip();});
        container.Add(chip);
        return chip;
    }

    private void OnFilterSelected()
    {
        if(search.Q<ChipsTab>("").SelectedKeywords.Count > 0)
        {
            _searchSubmitKeywordOnly.style.visibility = Visibility.Visible;
        }
        else // 선택된 필터 없음
        {
            _searchSubmitKeywordOnly.style.visibility = Visibility.Hidden;
        }
    }

    private void ToggleSearchChipsTab()
    {
        Debug.Log("Changed");
        if(_searchResultsContainer.style.visibility == Visibility.Visible)
        {
            _searchChipsTab.style.visibility = Visibility.Hidden;
        }
        else
        {
            _searchChipsTab.style.visibility = Visibility.Visible;
        }
    }

    private void OnSubmitKeywordsOnly()
    {
        UpdateSearchCards("");
        search.Q<VisualElement>("search-chips-tab").style.visibility = Visibility.Hidden;
        _searchSubmitKeywordOnly.style.visibility = Visibility.Hidden;
    }

    private void UpdateHomeScreen()
    {
        _homeName.text = playerUserData.name;
        _homeIntroduction.text = playerUserData.introduction_text;

        _homeKeywordsContainer.Clear();
        foreach(var introduction in SelectedIntroductionString)
        {
            AddChip(introduction, _homeKeywordsContainer);
        }
        
        _homeInterestsContainer.Clear();
        foreach(var interest in SelectedInterestString)
        {
            AddChip(interest, _homeInterestsContainer);
        }
    }

    private void UpdateEditProfileScreen()
    {
        _editProfileName.text = playerUserData.name;
        _editProfileJob.text = playerUserData.job;
        _editProfileIntroduction.text = playerUserData.introduction_text;

        List<VisualElement> children = new List<VisualElement>(_editKeywordChipContainer.Children());
            
        foreach (var child in children)
        {
            if (!child.ClassListContains("chip-add"))
            {
                _editKeywordChipContainer.Remove(child);
            }
        }

        children = new List<VisualElement>(_editInterestChipContainer.Children());
            
        foreach (var child in children)
        {
            if (!child.ClassListContains("chip-add"))
            {
                _editInterestChipContainer.Remove(child);
            }
        }

        children = new List<VisualElement>(_editURLChipContainer.Children());
            
        foreach (var child in children)
        {
            if (!child.ClassListContains("chip-add"))
            {
                _editURLChipContainer.Remove(child);
            }
        }


        AddRemovableChip(playerUserData.introduction_1, _editKeywordChipContainer, SelectedIntroductionString);
        if(playerUserData.introduction_2 != "")
        {
            AddRemovableChip(playerUserData.introduction_2, _editKeywordChipContainer, SelectedIntroductionString);
        }
        if(playerUserData.introduction_3 != "")
        {
            AddRemovableChip(playerUserData.introduction_3, _editKeywordChipContainer, SelectedIntroductionString);
        }

        AddRemovableChip(playerUserData.interest_1, _editInterestChipContainer, SelectedInterestString);
        if(playerUserData.interest_2 != "")
        {
            AddRemovableChip(playerUserData.interest_2, _editInterestChipContainer, SelectedInterestString);
        }
        if(playerUserData.interest_3 != "")
        {
            AddRemovableChip(playerUserData.interest_3, _editInterestChipContainer, SelectedInterestString);
        }

        AddRemovableChip(playerUserData.url, _editURLChipContainer);
    }

    private void UpdateIntroductionKeyword()
    {
        if(SelectedIntroductionString.Count > 0)
        {
            playerUserData.introduction_1 = SelectedIntroductionString[0];
        }
        else
        {
            playerUserData.introduction_1 = "";
        }

        if(SelectedIntroductionString.Count > 1)
        {
            playerUserData.introduction_2 = SelectedIntroductionString[1];
            
        }
        else
        {
            playerUserData.introduction_2 = "";
        }

        if(SelectedIntroductionString.Count > 2)
        {
            playerUserData.introduction_3 = SelectedIntroductionString[2];
        }
        else
        {
            playerUserData.introduction_3 = "";
        }

        DatabaseManager.Instance.editProfile(playerUserData.pin, playerUserData);
        UpdateHomeScreen();
    }

    private void UpdateInterestKeyword()
    {
        if(SelectedInterestString.Count > 0)
        {
            playerUserData.interest_1 = SelectedInterestString[0];
        }
        else
        {
            playerUserData.interest_1 = "";
        }
        if(SelectedInterestString.Count > 1)
        {
            playerUserData.interest_2 = SelectedInterestString[1];
            
        }
        else
        {
            playerUserData.interest_2 = "";
        }
        if(SelectedInterestString.Count > 2)
        {
            playerUserData.interest_3 = SelectedInterestString[2];
        }
        else
        {
            playerUserData.interest_3 = "";
        }

        DatabaseManager.Instance.editProfile(playerUserData.pin, playerUserData);
        UpdateHomeScreen();
    }

    private void ShowShareScreen()
    {
        shareScreen.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
