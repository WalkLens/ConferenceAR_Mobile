using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;

public class ShareScreenManager : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;

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

    // URL

    // Share-Complete
    
    private void OnEnable()
    {
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
        

        // Home
        shareButton = home.Q<VisualElement>("share-button");
        shareButton.RegisterCallback<ClickEvent>(evt => ShowNextScreen());

        // URL

        // Share-Complete
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
        container.style.translate = new Translate(-screenWidth, 0, 0); // TODO 1.5초 뒤에 넘어가게
    }
}
