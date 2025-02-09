using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using MRTK.Tutorials.MultiUserCapabilities;
using UnityEngine.Windows;

public class LoginScreenManager: MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private GameObject register;
    [SerializeField] private GameObject home;

    // Screens    
    private VisualElement root;
    private VisualElement container; // 컨텐츠를 감싸는 컨테이너
    private TextField[] pinFields = new TextField[5];
    private VisualElement createAccount;
    private string pinCode = "";
    private float screenWidth; // 각 뷰의 너비 (화면 크기)
    
    
    
    private void OnEnable()
    {
        root = uiDocument.rootVisualElement;
        container = root.Q<VisualElement>("content");
        

        Debug.Log("UI Document 연결완료");

        // VisualElement 생성 및 스타일 클래스 추가
        // 초기 화면 너비 출력
        UpdateScreenWidth();

        // 크기 변경 이벤트 등록
        root.RegisterCallback<GeometryChangedEvent>(evt => UpdateScreenWidth());

        createAccount = root.Q<VisualElement>("CreateAccount");
        createAccount.RegisterCallback<ClickEvent>(evt => Register());

        pinFields[0] = root.Q<TextField>("PIN1");
        pinFields[1] = root.Q<TextField>("PIN2");
        pinFields[2] = root.Q<TextField>("PIN3");
        pinFields[3] = root.Q<TextField>("PIN4");
        pinFields[4] = root.Q<TextField>("PIN5");

        for(int i=0; i<5; i++)
        {
            int index = i; // 람다 캡처 문제 방지
            //pinFields[i].maxLength = 1; // 한 글자만 입력 가능하도록 설정
            pinFields[i].RegisterValueChangedCallback(evt => PINInput(evt.newValue, index));
        }
    }

    private void UpdateScreenWidth()
    {
        screenWidth = root.resolvedStyle.width;
        Debug.Log($"Updated Screen Width: {screenWidth}px");
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
            if(DatabaseManager.Instance.isPINDuplicate(code))
            {
                DatabaseManager.Instance.playerUserData = DatabaseManager.Instance.getUserData(code);// -> 여기서 접속 진행
                PhotonLobbyConferenceAR.Lobby.JoinOrCreateRoom(code);
                home.SetActive(true);
                this.gameObject.SetActive(false);
            }
            else
            {
                foreach (var field in pinFields)
                {
                    field.value = "";
                }
                // TODO 경고 메시지
            }
        }
        return code;
    }

    private void Register()
    {
        register.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
