using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using MRTK.Tutorials.MultiUserCapabilities;
using UnityEngine.Windows;

public class RegisterScreenManager : MonoBehaviour
{
    // 0. Common
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private GameObject home;
    
    private VisualElement root;
    private VisualElement container; // 컨텐츠를 감싸는 컨테이너
    private float screenWidth; // 각 뷰의 너비 (화면 크기)
    private int currentPage = 1;
    [SerializeField] private int totalPages = 7;
    
    private Button _submitButton;

    // 1. BasicInfoScreen
    private VisualElement _profileArea;
    // public RawImage img;
    private TextField userName;
    private TextField userJob;
    private DropdownField userLanguage;
    
    // 2. KeywordScreen
    [SerializeField] private string[] researchChipString = {"게임 기획", "서비스 기획", "콘텐츠 기획", "데이터 분석", "마케팅", "UX 기획", "비즈니스 분석", "PM"};
    [SerializeField] private string[] designChipString = {"3D 디자인", "브랜딩 디자인", "UX/UI", "모션 그래픽", "캐릭터/레벨", "VFX", "컨셉 아티스트/그래픽", "사운드"};
    [SerializeField] private string[] devChipString = {"프론트엔드", "백엔드/서버", "게임 개발", "그래픽 개발", "임베디드", "데이터/AI", "데브옵스/지원", "XR"};

    private int _keywordActiveTab = 0; // 0: Research, 1: Design, 2: Dev
    private Button[] _keywordTabButtons = new Button[3];
    private VisualElement _keywordBar;
    private VisualElement _keywordChipContainer;
    private VisualElement keywordResearchContainer;
    private VisualElement keywordDesignContainer;
    private VisualElement keywordDevContainer;

    private int keywordResearchChipCount;
    private int keywordDesignChipCount;
    private int keywordDevChipCount;

    private List<string> selectedKeywords = new List<string>();

    // 3. InterestScreen
    private int _interestActiveTab = 0; // 0: Research, 1: Design, 2: Dev
    private Button[] _interestTabButtons = new Button[3];
    private VisualElement _interestBar;
    private VisualElement _interestChipContainer;
    private VisualElement interestResearchContainer;
    private VisualElement interestDesignContainer;
    private VisualElement interestDevContainer;

    private int interestResearchChipCount;
    private int interestDesignChipCount;
    private int interestDevChipCount;

    private List<string> selectedInterests = new List<string>();

    // 4. IntroduceScreen
    private TextField userIntroduction;
    private TextField userURL;
    private Label introductionHeader;

    // 5. RegisterCompleteScreen
    private Label registerCompleteHeader;

    // 6. SetPINScreen
    private TextField[] pinFields = new TextField[5];
    private string pinCode = "";
    private Label duplicateExplanation;

    // 7. ConnectDeviceScreen
    
    private void OnEnable()
    {
        root = uiDocument.rootVisualElement;

        Debug.Log("UI Document 연결완료");

        // VisualElement 생성 및 스타일 클래스 추가
        // 초기 화면 너비 출력
        UpdateScreenWidth();

        // 크기 변경 이벤트 등록
        root.RegisterCallback<GeometryChangedEvent>(evt => UpdateScreenWidth());
        root.RegisterCallback<GeometryChangedEvent>(evt => LoadChips());


        // 0. Common
        container = root.Q<VisualElement>("Content");

        _submitButton = root.Q<Button>("SubmitButton");

        _submitButton.RegisterCallback<ClickEvent>(evt => Submit());

        _submitButton.SetEnabled(false); // 초기값은 비활성화

        // 1. BasicInfoScreen

        _profileArea = root.Q<VisualElement>("Photo");

        _profileArea.RegisterCallback<ClickEvent>(evt => UpdatePhoto());
        
        userName = root.Q<TextField>("UserName");
        userJob = root.Q<TextField>("UserJob");
        userLanguage = root.Q<DropdownField>("UserLanguage");
        
        userName.RegisterValueChangedCallback(evt => OnChangeBasicInfo());
        userJob.RegisterValueChangedCallback(evt => OnChangeBasicInfo());
        userLanguage.RegisterValueChangedCallback(evt => OnChangeBasicInfo());

        // 2. KeywordScreen
        _keywordTabButtons[0] = root.Q<Button>("KeywordResearch");
        _keywordTabButtons[1] = root.Q<Button>("KeywordDesign");
        _keywordTabButtons[2] = root.Q<Button>("KeywordDev");

        _keywordTabButtons[0].RegisterCallback<ClickEvent>(evt => ToggleKeywordTabButton(0));
        _keywordTabButtons[1].RegisterCallback<ClickEvent>(evt => ToggleKeywordTabButton(1));
        _keywordTabButtons[2].RegisterCallback<ClickEvent>(evt => ToggleKeywordTabButton(2));

        _keywordBar = root.Q<VisualElement>("KeywordBar");

        _keywordChipContainer = root.Q<VisualElement>("KeywordChipContainer");
        
        keywordResearchContainer = root.Q<VisualElement>("KeywordResearchChips");
        keywordDesignContainer = root.Q<VisualElement>("KeywordDesignChips");
        keywordDevContainer = root.Q<VisualElement>("KeywordDevChips");

        // 3. InterestScreen
        _interestTabButtons[0] = root.Q<Button>("InterestResearch");
        _interestTabButtons[1] = root.Q<Button>("InterestDesign");
        _interestTabButtons[2] = root.Q<Button>("InterestDev");

        _interestTabButtons[0].RegisterCallback<ClickEvent>(evt => ToggleInterestTabButton(0));
        _interestTabButtons[1].RegisterCallback<ClickEvent>(evt => ToggleInterestTabButton(1));
        _interestTabButtons[2].RegisterCallback<ClickEvent>(evt => ToggleInterestTabButton(2));

        _interestBar = root.Q<VisualElement>("InterestBar");

        _interestChipContainer = root.Q<VisualElement>("InterestChipContainer");
        
        interestResearchContainer = root.Q<VisualElement>("InterestResearchChips");
        interestDesignContainer = root.Q<VisualElement>("InterestDesignChips");
        interestDevContainer = root.Q<VisualElement>("InterestDevChips");

        // 4. IntroduceScreen
        userIntroduction = root.Q<TextField>("UserIntroduction");
        userIntroduction.RegisterValueChangedCallback(evt => UpdateIntroductionHeader(evt.newValue));
        userURL = root.Q<TextField>("UserURL");
        introductionHeader = root.Q<Label>("IntroductionHeader");

        // 5. RegisterCompleteScreen
        registerCompleteHeader = root.Q<Label>("RegisterCompleteHeader");

        // 6. SetPINScreen
        pinFields[0] = root.Q<TextField>("PIN1");
        pinFields[1] = root.Q<TextField>("PIN2");
        pinFields[2] = root.Q<TextField>("PIN3");
        pinFields[3] = root.Q<TextField>("PIN4");
        pinFields[4] = root.Q<TextField>("PIN5");

        duplicateExplanation = root.Q<Label>("DuplicateExplanation");
        
        for(int i=0; i<5; i++)
        {
            int index = i; // 람다 캡처 문제 방지
            //pinFields[i].maxLength = 1; // 한 글자만 입력 가능하도록 설정
            pinFields[i].RegisterValueChangedCallback(evt => PINInput(evt.newValue, index));
        }
    }

    private void LoadChips() // OnEnable에는 폰트 데이터가 없기 때문에 GeometryChangedEvent로 구현
    {
        foreach(var chipString in researchChipString)
        {
            var chip = new Button
            {
                text = chipString
            };


            chip.RegisterCallback<ClickEvent>(evt => ToggleInterestChip(chip, 0));
            chip.AddToClassList("chip");
            interestResearchContainer.Add(chip);
        }

        foreach(var chipString in designChipString)
        {
            var chip = new Button
            {
                text = chipString
            };
            
            chip.RegisterCallback<ClickEvent>(evt => ToggleInterestChip(chip, 1));
            chip.AddToClassList("chip");
            interestDesignContainer.Add(chip);
        }

        foreach(var chipString in devChipString)
        {
            var chip = new Button
            {
                text = chipString
            };
            
            chip.RegisterCallback<ClickEvent>(evt => ToggleInterestChip(chip, 2));
            chip.AddToClassList("chip");
            interestDevContainer.Add(chip);
        }

        foreach(var chipString in researchChipString)
        {
            var chip = new Button
            {
                text = chipString
            };

            chip.RegisterCallback<ClickEvent>(evt => ToggleKeywordChip(chip, 0));
            chip.AddToClassList("chip");
            keywordResearchContainer.Add(chip);
        }

        foreach(var chipString in designChipString)
        {
            var chip = new Button
            {
                text = chipString
            };
            
            chip.RegisterCallback<ClickEvent>(evt => ToggleKeywordChip(chip, 1));
            chip.AddToClassList("chip");
            keywordDesignContainer.Add(chip);
        }

        foreach(var chipString in devChipString)
        {
            var chip = new Button
            {
                text = chipString
            };
            
            chip.RegisterCallback<ClickEvent>(evt => ToggleKeywordChip(chip, 2));
            chip.AddToClassList("chip");
            keywordDevContainer.Add(chip);
        }
    }

    private void UpdatePhoto()
    {
        Debug.Log("Update Photo");
    }

    private void Submit()
    {
        if(currentPage == 2)
        {
            registerCompleteHeader.text = $"{userName.text}님의 프로필이\n완성되었어요!";
        }
        
        if(currentPage == 6) // PIN 번호 등록
        {
            if(DatabaseManager.Instance.isPINDuplicate(pinCode))
            {
                currentPage--;
                duplicateExplanation.style.visibility = Visibility.Visible;
                foreach (var pinField in pinFields)
                {
                    pinField.value = "";
                }
                // TODO Style - wrong
            }
            else
            {
                DatabaseManager.Instance.registerProfile(ConvertToUserData());
                PhotonLobbyConferenceAR.Lobby.JoinOrCreateRoom(pinCode);
            }
        }

        if(currentPage == 7)
        {
            home.SetActive(true);
            this.gameObject.SetActive(false);
        }

        // Debug.Log("Clicked!!");
        if (currentPage < totalPages)
        {
            currentPage++;
            UpdateContainerPosition();

            switch(currentPage){
                case 1:
                    _submitButton.text = "다음으로";
                    break;
                case 2:
                    break;
                case 3:
                    _keywordChipContainer.style.visibility = Visibility.Hidden;
                    break;
                case 4:
                    _interestChipContainer.style.visibility = Visibility.Hidden;
                    break;
                case 5:
                    // DB에 넣을 데이터 정제하기
                    _submitButton.text = "PIN 번호 설정하기";
                    break;
                case 6:
                    _submitButton.SetEnabled(false);
                    _submitButton.text = "다음으로";
                    break;
                case 7:
                    _submitButton.SetEnabled(false);
                    _submitButton.text = "기기와 연결 중입니다...";
                    StartCoroutine(ConnectHMD());
                    break;
            }
        }
    }

    private void UpdateContainerPosition()
    {
        // 새로운 위치 계산 (왼쪽으로 이동)
        float newX = -(currentPage - 1) * screenWidth;
        container.style.translate = new Translate(newX, 0, 0);
    }

    private void UpdateScreenWidth()
    {
        screenWidth = root.resolvedStyle.width;
        Debug.Log($"Updated Screen Width: {screenWidth}px");
    }

    private void ToggleKeywordTabButton(int category)
    {
        // if (!_tabButtons[category].ClassListContains("tab-button-selected"))
        // {
        //     Debug.Log("'tab-button-selected' 클래스가 없습니다.");
        //     _tabButtons[category].RemoveFromClassList("tab-button-selected");
        //     _tabButtons[category].AddToClassList("tab-button-selected");

            
        // }
        if(_keywordActiveTab != category)
        {
            // Debug.Log($"Category Changed From {activeTab} To {category}");
            _keywordTabButtons[_keywordActiveTab].RemoveFromClassList("tab-button-selected");
            _keywordTabButtons[category].AddToClassList("tab-button-selected");
            _keywordActiveTab = category;
            _keywordBar.style.translate = new Translate(Length.Percent(132*(_keywordActiveTab-1)), 0);
            _keywordChipContainer.style.translate = new Translate(Length.Percent(-33*(_keywordActiveTab-1)), 0);
        }
    }

    private void ToggleInterestTabButton(int category)
    {
        // if (!_tabButtons[category].ClassListContains("tab-button-selected"))
        // {
        //     Debug.Log("'tab-button-selected' 클래스가 없습니다.");
        //     _tabButtons[category].RemoveFromClassList("tab-button-selected");
        //     _tabButtons[category].AddToClassList("tab-button-selected");

            
        // }
        if(_interestActiveTab != category)
        {
            // Debug.Log($"Category Changed From {activeTab} To {category}");
            _interestTabButtons[_interestActiveTab].RemoveFromClassList("tab-button-selected");
            _interestTabButtons[category].AddToClassList("tab-button-selected");
            _interestActiveTab = category;
            _interestBar.style.translate = new Translate(Length.Percent(132*(_interestActiveTab-1)), 0);
            _interestChipContainer.style.translate = new Translate(Length.Percent(-33*(_interestActiveTab-1)), 0);
        }
    }
    // For all chips
    private void ToggleKeywordChip(Button chip, int category) // TODO ClickEvent 넘겨주는 방식으로 처리?
    {
        if (chip.ClassListContains("chip-active"))
        {
            // Debug.Log("'chip-active' 클래스가 존재합니다.");
            selectedKeywords.Remove(chip.text);
            Debug.Log(selectedKeywords.Count);
            chip.RemoveFromClassList("chip-active");
            switch(category)
            {
                case 0:
                    keywordResearchChipCount -= 1;
                    break;
                case 1:
                    keywordDesignChipCount -= 1;
                    break;
                case 2:
                    keywordDevChipCount -= 1;
                    break;
                default:
                    break;
            }
        }
        else if(selectedKeywords.Count < 3)// Chip Deactivated && Active Chip Count < 3
        {
            // Debug.Log("'chip-active' 클래스가 없습니다.");
            Debug.Log(chip.text);
            selectedKeywords.Add(chip.text);
            Debug.Log(selectedKeywords.Count);
            chip.AddToClassList("chip-active");
            switch(category)
            {
                case 0:
                    keywordResearchChipCount += 1;
                    break;
                case 1:
                    keywordDesignChipCount += 1;
                    break;
                case 2:
                    keywordDevChipCount += 1;
                    break;
                default:
                    break;
            }
        }
    }

    private void ToggleInterestChip(Button chip, int category) // TODO ClickEvent 넘겨주는 방식으로 처리?
    {
        if (chip.ClassListContains("chip-active"))
        {
            // Debug.Log("'chip-active' 클래스가 존재합니다.");
            selectedInterests.Remove(chip.text);
            Debug.Log(selectedInterests.Count);
            chip.RemoveFromClassList("chip-active");
            switch(category)
            {
                case 0:
                    interestResearchChipCount -= 1;
                    break;
                case 1:
                    interestDesignChipCount -= 1;
                    break;
                case 2:
                    interestDevChipCount -= 1;
                    break;
                default:
                    break;
            }
        }
        else if(selectedInterests.Count < 3)// Chip Deactivated && Active Chip Count < 3
        {
            // Debug.Log("'chip-active' 클래스가 없습니다.");
            Debug.Log(chip.text);
            selectedInterests.Add(chip.text);
            Debug.Log(selectedInterests.Count);
            chip.AddToClassList("chip-active");
            switch(category)
            {
                case 0:
                    interestResearchChipCount += 1;
                    break;
                case 1:
                    interestDesignChipCount += 1;
                    break;
                case 2:
                    interestDevChipCount += 1;
                    break;
                default:
                    break;
            }
        }
    }

    private void OnChangeBasicInfo()
    {
        if(!string.IsNullOrEmpty(userName.text) && !string.IsNullOrEmpty(userJob.text) && !string.IsNullOrEmpty(userLanguage.text))
        {
            _submitButton.SetEnabled(true);
        }
    }

    private void PINInput(string newValue, int index)
    {
        // Debug.Log(newValue);
        // Debug.Log(index);
        if (!string.IsNullOrEmpty(newValue)) 
        {
            if (index < 4)
            {
                pinFields[index + 1].Focus();
            }
        }
        pinCode = UpdatePINCode();
    }

    private string UpdatePINCode()
    {
        string code = "";
        foreach (var field in pinFields)
        {
            code += field.text;
        }

        if(code.Length == pinFields.Length)
        {
            _submitButton.SetEnabled(true);
        }
        return code;
    }

    public UserData ConvertToUserData()
    {
        DatabaseManager.Instance.playerUserData = new UserData
        {
            pin = pinCode,
            name = userName.text,
            job = userJob.text,
            language = LanguageToCode(userLanguage.text),
            introduction_1 = selectedKeywords.Count > 0 ? selectedKeywords[0] : "", // TODO 최대 3개로 구조 수정
            introduction_2 = selectedKeywords.Count > 1 ? selectedKeywords[1] : "",
            introduction_3 = selectedKeywords.Count > 2 ? selectedKeywords[2] : "",
            introduction_4 = "",
            introduction_5 = "",
            interest_1 = selectedInterests.Count > 0 ? selectedInterests[0] : "", // TODO 최대 3개로 구조 수정
            interest_2 = selectedInterests.Count > 1 ? selectedInterests[1] : "",
            interest_3 = selectedInterests.Count > 2 ? selectedInterests[2] : "",
            interest_4 = "",
            interest_5 = "",
            introduction_text = userIntroduction.text,
            url = userURL.text,
            autoaccept = true
        };

        return DatabaseManager.Instance.playerUserData; // TODO 구조변경??
    }

    public string LanguageToCode(string language)
    {
        // 러시아 - "ru-RU"
        // 스페인어 - "es-ES"
        // 독일어 - "de-DE"
        // 중국어 - "zh-HK"
        // 한국어 - "ko-KR"
        // 영어 - "en-US"
        // 일본어 - "ja-JP"
        // Korean,English,Japanese,Chinese,Spanish,German,Russian
        // 한국어, English, 日本語, 中文, Español, Deutsch, Русский
        
        if(language == "한국어")
        {
            return "ko-KR";
        }
        else if(language == "English")
        {
            return "en-US";
        }
        else if(language == "日本語")
        {
            return "ja-JP";
        }
        else if(language == "中文")
        {
            return "zh-HK";
        }
        else if(language == "Español")
        {
            return "es-ES";
        }
        else if(language == "Deutsch")
        {
            return "de-DE";
        }
        else if(language == "Русский")
        {
            return "ru-RU";
        }
        else
        {
            return "ko-KR";
        }
    }

    private void UpdateIntroductionHeader(string newValue)
    {
        introductionHeader.text = $"자기소개 <color=grey>({newValue.Length}/200)";
    }

    private IEnumerator ConnectHMD() // TODO Event로 변경
    {
        // TODO yield return 대신 HMD와 연결하는 코드로 바꿔주세요
        yield return new WaitForSeconds(2f);
        
        _submitButton.SetEnabled(true);
        _submitButton.text = "기기 연결을 완료했어요";
    }
}
