using CustomLogger;
using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UIElements;

public class ProfileCard : VisualElement
{
    private VisualElement _notification;
    private Label _nameLabel;
    private Label _jobLabel;
    private VisualElement _profilePhoto;
    private VisualElement _keywordsContainer;
    private VisualElement _interestsContainer;
    public Button _wishButton;
    private Button _meetButton;
    private int status; // 0: offline, 1: online, 2: away
    private VisualElement _card;
    public UserData profileData;

    public ProfileCard(UserData userData)
    {
        this.profileData = userData;
        // UXML 및 USS 불러오기
        var visualTree = Resources.Load<VisualTreeAsset>("UI/VisualTree/ProfileCard");
        visualTree.CloneTree(this);
        // styleSheets.Add(Resources.Load<StyleSheet>("Chip"));

        // 요소 참조 가져오기
        _notification = this.Q<VisualElement>("notification");
        _card = this.Q<VisualElement>("card");
        //_card.RegisterCallback<ClickEvent>(evt => ShowHMD());
        // 포커스가 되었을 때 실행
        _card.RegisterCallback<FocusEvent>(evt => ShowHMD());
        _card.RegisterCallback<FocusEvent>(evt => OnFocused());

        // 포커스를 잃었을 때 실행
        _card.RegisterCallback<BlurEvent>(evt => OnBlurred());
        _nameLabel = this.Q<Label>("name");
        _jobLabel = this.Q<Label>("job");
        _profilePhoto = this.Q<VisualElement>("photo");
        _meetButton = this.Q<Button>("meet-button");
        _meetButton.RegisterCallback<ClickEvent>(evt => {Meet(); evt.StopPropagation();});
        _wishButton = this.Q<Button>("wish");
        
        _keywordsContainer = this.Q<VisualElement>("keywords-container");
        _interestsContainer = this.Q<VisualElement>("interests-container");

        // 텍스트 설정
        _nameLabel.text = userData.name;
        _jobLabel.text = userData.job;
        // Profilephoto URL 설정

        AddChip(userData.introduction_1, _keywordsContainer);
        AddChip(userData.introduction_2, _keywordsContainer);
        AddChip(userData.introduction_3, _keywordsContainer);

        AddChip(userData.interest_1, _interestsContainer);
        AddChip(userData.interest_2, _interestsContainer);
        AddChip(userData.interest_3, _interestsContainer);
    }

    public void SetStatus(int status) // 0: offline, 1: online, 2: away
    {
        this.status = status;

        // TODO status 이미지 변경
    }

    private void AddChip(string text, VisualElement container)
    {
        var chip = new SelectableChip(text);
        container.Add(chip);
    }

    private void ShowHMD()
    {
        Debug.Log(this.profileData.pin); // AR에 프로필 띄우기
        OnSendPinNumButtonClicked(this.profileData.pin);

    }


    public void OnSendPinNumButtonClicked(string myNick)
    {
        // 현재 사용자의 닉네임에서 PIN 추출 (예: "12345_Mobile" -> "12345")
        //string myNick = PhotonNetwork.NickName;
        string pin = DatabaseManager.Instance.playerUserData.pin;
      
        // 대응하는 Hololens 사용자의 닉네임은 "PIN_Hololens"로 가정
        string targetUserName = $"{pin}_hololens";
        int targetActorNumber = PhotonUserUtility.GetPlayerActorNumber(targetUserName);
        Debug.Log($"{targetUserName}에게 데이터 전송햇음!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!.");
        if (targetActorNumber == null)
        {
            Debug.LogError($"대상 사용자 '{targetUserName}'를 찾을 수 없습니다.");
            return;
        }

        UserMatchingManager.Instance.ReceivePinnumForProfile(targetActorNumber, myNick);
    }




    public void OnSendMatchingButtonClicked(string myNick)
    {
        // 현재 사용자의 닉네임에서 PIN 추출 (예: "12345_Mobile" -> "12345")
        //string myNick = PhotonNetwork.NickName;
        string pin = myNick.Split('_')[0];

        // 대응하는 Hololens 사용자의 닉네임은 "PIN_Hololens"로 가정
        string targetUserName = $"{pin}_hololens";
        UserInfo targetUserInfo = UserMatchingManager.Instance.userInfos.Find(
            u => u.PhotonUserName.Contains(targetUserName, System.StringComparison.OrdinalIgnoreCase)
        );

        if (targetUserInfo == null)
        {
            //Debug.LogError($"대상 사용자 '{targetUserName}'를 찾을 수 없습니다.");
            return;
        }

        UserInfo myUserInfo = UserMatchingManager.Instance.myUserInfo;

        // 매칭 요청 전송 (매칭 요청 ID 0은 "Request..."로 처리한다고 가정)
        int matchRequestId = 0;

        if(myNick != DatabaseManager.Instance.playerUserData.pin)
        {
            DebugUserInfos.Instance.SendMatchRequestToAUser(targetUserName, myUserInfo, matchRequestId);

        }
        else
        {
            Debug.Log("Same ID!!!!!!!!!!!!!!!!!!!! You cant send");
        }

        // PhotonUserUtility를 통해 해당 닉네임의 ActorNumber를 획득
        /*int targetActorNumber = PhotonUserUtility.GetPlayerActorNumber(targethololensNick);

        if (targetActorNumber != -1)
        {
            // 예시 URL 데이터를 보내기 ("http://example.com")
            string urlToSend = "http://example.com";
            UserMatchingManager.Instance.SendURLDataToTarget(targetActorNumber, urlToSend);
        }
        else
        {
            Debug.LogError($"Hololens 사용자 {targetHololensNick}의 ActorNumber를 찾을 수 없습니다.");
        }*/
    }

    private void Meet()
    {
        Debug.Log(this.profileData.pin); // AR에 프로필 띄우기
        OnSendMatchingButtonClicked(this.profileData.pin);

        string pin = DatabaseManager.Instance.playerUserData.pin;

        // 대응하는 Hololens 사용자의 닉네임은 "PIN_hololens"로 가정
        string targetUserName = $"{pin}_hololens";
        int targetActorNumber = PhotonUserUtility.GetPlayerActorNumber(targetUserName);
        UserMatchingManager.Instance.PopUpUINotify(targetActorNumber);

    }

    private void OnFocused() // TODO 누르면 다시 없어짐
    {
        _notification.style.display = DisplayStyle.Flex;
        _notification.style.opacity = 1;
        _meetButton.style.display = DisplayStyle.Flex;
        _meetButton.style.opacity = 1;
        _card.AddToClassList("card-active");
    }

    private void OnBlurred()
    {
        _notification.style.opacity = 0;
        _notification.style.display = DisplayStyle.None;
        _meetButton.style.opacity = 0;
        _meetButton.style.display = DisplayStyle.None;
        _card.RemoveFromClassList("card-active");
    }
}