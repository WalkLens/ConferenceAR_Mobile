using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;

public class ShareScreenManager : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private GameObject homeScreen;

    private UserData playerUserData;

    // Screens    
    private VisualElement root;
    private VisualElement container; // 컨텐츠를 감싸는 컨테이너
    private TemplateContainer many;
    private TemplateContainer home;
    private TemplateContainer url;
    private TemplateContainer shareComplete;
    private float screenWidth; // 각 뷰의 너비 (화면 크기)

    // Many

    // Home
    private VisualElement shareButton;
    private Label _name;
    private Label _description;

    // URL
    private VisualElement _shareURLButton;
    private VisualElement _popup;
    private VisualElement _popupBackground;
    private TextField _popupLink;
    private Button _popupCancelButton;
    private Button _popupShareButton;

    // Share-Complete
    
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
        // playerUserData = DatabaseManager.Instance.playerUserData;
        playerUserData = dummyUserData;
        
        root = uiDocument.rootVisualElement;
        container = root.Q<VisualElement>("content");

        many = container.Q<TemplateContainer>("many");
        home = container.Q<TemplateContainer>("home");
        url = container.Q<TemplateContainer>("url");
        shareComplete = container.Q<TemplateContainer>("share-complete");

        Debug.Log("UI Document 연결완료");

        // VisualElement 생성 및 스타일 클래스 추가
        // 초기 화면 너비 출력
        UpdateScreenWidth();

        // 크기 변경 이벤트 등록
        root.RegisterCallback<GeometryChangedEvent>(evt => UpdateScreenWidth());

        // Many
        StartCoroutine(ShowHomeScreen());

        // Home
        shareButton = home.Q<VisualElement>("share-button");
        shareButton.RegisterCallback<ClickEvent>(evt => ShowNextScreen());
        _name = home.Q<Label>("name");
        _description = home.Q<Label>("description");

        _name.text = playerUserData.name;
        _description.text = playerUserData.introduction_text;

        // URL
        _shareURLButton = url.Q<VisualElement>("Share");
        _shareURLButton.RegisterCallback<ClickEvent>(evt => ShowPopup());

        _popupBackground = url.Q<VisualElement>("Background");
        _popup = url.Q<VisualElement>("Popup");

        _popupLink = url.Q<TextField>("PopupLink");

        _popupCancelButton = url.Q<Button>("ButtonCancel");
        _popupCancelButton.RegisterCallback<ClickEvent>(evt => ClosePopup());
        _popupShareButton = url.Q<Button>("ButtonShare");
        _popupShareButton.RegisterCallback<ClickEvent>(evt => Share());
        _popupShareButton.RegisterCallback<ClickEvent>(evt => ClosePopup());

        // Share-Complete
    }

    IEnumerator ShowHomeScreen()
    {
        yield return new WaitForSeconds(1.5f);
        container.style.translate = new Translate(-screenWidth, 0, 0);
    }

    private void ShowNextScreen()
    {
        // 새로운 위치 계산 (왼쪽으로 이동)
        float newX = -2 * screenWidth;
        container.style.translate = new Translate(newX, 0, 0);
    }
    private void UpdateScreenWidth()
    {
        screenWidth = root.resolvedStyle.width;
        Debug.Log($"Updated Screen Width: {screenWidth}px");
    }

    private void ShowPopup()
    {
        _popup.style.display = DisplayStyle.Flex;
        _popupBackground.style.display = DisplayStyle.Flex;
    }

    private void ClosePopup()
    {
        _popup.style.display = DisplayStyle.None;
        _popupBackground.style.display = DisplayStyle.None;
    }

    private void Share()
    {
        Debug.Log(_popupLink.text);
        homeScreen.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
