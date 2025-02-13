using System;
using CustomLogger;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using System.Threading.Tasks;
using MRTK.Tutorials.MultiUserCapabilities;

public class UserMatchingManager : HostOnlyBehaviour
{
    public static UserMatchingManager Instance;
    public UserInfo myUserInfo; // 나의 정보
    public UserInfo partnerUserInfo = new UserInfo(); // 상대방의 정보
    public List<UserInfo> userInfos = new List<UserInfo>(); // 모든 유저 정보 리스트
    public DebugUserInfos debugUserInfo;

    private Dictionary<byte, Action<EventData>> eventHandlers;
    public const byte RenameEvent = 1; // 유저 이름 변경 이벤트 코드
    public const byte SendUserInfoEvent = 2; // 유저 정보 전송 이벤트 코드
    public const byte SendUsersInfoEvent = 3; // 모든 유저 정보 전송 이벤트 코드
    public const byte SendMatchInfoEvent = 4; // 매칭 요청 이벤트 코드
    public const byte SendMeetingInfoEvent = 5; // 미팅 정보 이벤트 코드(매칭 성사)
    public const byte SendCustomStringEvent = 6; // 초기에 접속하는 코드
    public const byte SendURLDataEvent = 7;// URL 전송 이벤트 코드
    public const byte ViewProfileEvent = 8;
    public const byte SendMyUIPopUP = 9;
    public const byte CloseProfileEvent = 10;

    //============ SM ADD ============//
    public bool _isMatchingSucceed = false;
    public bool _isTravelingToMeet = false;
    public bool _isLoopRunning = false;

    public event Action OnTravelingToMeet;

    //public bool isUserMet = false;
    public bool isUserRibbonSelected = false;
    public GameObject myGameObject;
    public GameObject partnerGameObject;
    //============ SM ADD ============//

    private void Awake()
    {
        if (Instance == null) Instance = this;

        // 이벤트 코드와 실행할 메서드 매핑
        eventHandlers = new Dictionary<byte, Action<EventData>>
        {
            { RenameEvent, HandleRenameEvent },
            { SendUserInfoEvent, HandleSendUserInfoEvent },
            { SendUsersInfoEvent, HandleSendUsersInfoEvent },
            { SendMatchInfoEvent, HandleSendMatchInfoEvent },
            { SendMeetingInfoEvent, HandleSendMeetingInfoEvent },
            { SendCustomStringEvent, HandleSendCustomStringEvent },
            { SendURLDataEvent, HandleSendURLDataEvent },
            { ViewProfileEvent, HandleSendViewProfileEvent },
            { SendMyUIPopUP, HandleSendMyUIPopUPEvent },
            { CloseProfileEvent, HandleCloseProfileEvent}
        };
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += HandleEvent; // 이벤트 핸들러 등록
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= HandleEvent; // 이벤트 핸들러 해제
    }

    #region PhotonEventOVERRIDE

    public void SendMyUserInfoToMatchingMobile()
    {


        FileLogger.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!", this);

        // 현재 로컬 플레이어가 Hololens인 경우에만 실행 (닉네임에 "Hololens"가 포함되어 있다고 가정)
        if (!PhotonNetwork.NickName.Contains("Hololens"))
        {
            FileLogger.Log("이 함수는 Hololens에서만 호출됩니다.", this);
            return;
        }

        // 현재 닉네임 예시: "12345_HoloLens"
        string[] parts = PhotonNetwork.NickName.Split('_');
        if (parts.Length < 2)
        {
            FileLogger.Log("닉네임 형식이 올바르지 않습니다.", this);
            return;
        }

        string pin = parts[0]; // "12345"
        string mobileUserName = pin + "_Mobile"; // "12345_Mobile"

        // PhotonUserUtility.GetPlayerActorNumber()를 사용하여 대상 ActorNumber 획득 (해당 유틸리티 함수는 기존에 구현되어 있다고 가정)
        int targetActorNumber = PhotonUserUtility.GetPlayerActorNumber(mobileUserName);
        if (targetActorNumber == -1)
        {
            FileLogger.Log($"대상 Mobile 유저 [{mobileUserName}]를 찾을 수 없습니다.", this);
            return;
        }

        // 전송할 데이터 생성 (예시로 자신의 UserInfo를 전송)
        object[] data = new object[] { myUserInfo };
        RaiseEventOptions options = new RaiseEventOptions { TargetActors = new int[] { targetActorNumber } };

        PhotonNetwork.RaiseEvent(SendUserInfoEvent, data, options, SendOptions.SendReliable);
        FileLogger.Log($"[{PhotonNetwork.NickName}]가 [{mobileUserName}] (Actor: {targetActorNumber})에게 전송했습니다.", this);
    }


    public void SendURLDataToTarget(int targetActorNumber, string url)
    {
        // URL 데이터를 object 배열에 넣어 전송
        object[] content = new object[] { url };

        // 지정한 Actor에게만 전송하기 위한 옵션 설정
        RaiseEventOptions options = new RaiseEventOptions
        {
            TargetActors = new int[] { targetActorNumber }
        };

        try
        {
            PhotonNetwork.RaiseEvent(SendURLDataEvent, content, options, SendOptions.SendReliable);
            FileLogger.Log($"[SendURLDataToTarget] URL '{url}'를 Actor {targetActorNumber}에게 전송", this);
        }
        catch (Exception ex)
        {
            FileLogger.Log($"[SendURLDataToTarget] URL 전송 실패: {ex.Message}", this);
        }
    }


    public void SendCustomStringToTarget(int targetActorNumber, string message)
    {
        // 전송할 데이터를 object 배열로 구성 (여기서는 단순 string 하나만 포함)
        object[] data = new object[] { message };

        // 전송 옵션: 지정한 Actor에게만 보내기
        RaiseEventOptions options = new RaiseEventOptions
        {
            TargetActors = new int[] { targetActorNumber }
        };

        try
        {
            PhotonNetwork.RaiseEvent(SendCustomStringEvent, data, options, SendOptions.SendReliable);
            FileLogger.Log($"[SendCustomStringToTarget] 메시지 '{message}'를 Actor {targetActorNumber}에게 전송", this);
        }
        catch (Exception ex)
        {
            FileLogger.Log($"[SendCustomStringToTarget] 메시지 전송 실패: {ex.Message}", this);
        }
    }

    public void ReceivePinnumForProfile(int targetActorNumber, string pinnum)
    {
        object[] data = new object[] { pinnum };

        // 전송 옵션: 지정한 Actor에게만 보내기
        RaiseEventOptions options = new RaiseEventOptions
        {
            TargetActors = new int[] { targetActorNumber }
        };

        try
        {
            PhotonNetwork.RaiseEvent(ViewProfileEvent, data, options, SendOptions.SendReliable);
            FileLogger.Log($"[ReceivePinnumForProfile] 메시지 '{pinnum}'를 Actor {targetActorNumber}에게 전송", this);
        }
        catch (Exception ex)
        {
            FileLogger.Log($"[ReceivePinnumForProfile] 메시지 전송 실패: {ex.Message}", this);
        }
    }

    public void PopUpUINotify(int targetActorNumber)
    {
        object[] data = new object[] { };

        // 전송 옵션: 지정한 Actor에게만 보내기
        RaiseEventOptions options = new RaiseEventOptions
        {
            TargetActors = new int[] { targetActorNumber }
        };

        try
        {
            PhotonNetwork.RaiseEvent(SendMyUIPopUP, data, options, SendOptions.SendReliable);
            FileLogger.Log($"[PopUpUINotify] UI {targetActorNumber}에게 전송", this);
        }
        catch (Exception ex)
        {
            FileLogger.Log($"[PopUpUINotify] 메시지 전송 실패: {ex.Message}", this);
        }
    }

    public void ClosedProfileUI(int targetActorNumber)
    {
        object[] data = new object[] { };

        // 전송 옵션: 지정한 Actor에게만 보내기
        RaiseEventOptions options = new RaiseEventOptions
        {
            TargetActors = new int[] { targetActorNumber }
        };

        try
        {
            PhotonNetwork.RaiseEvent(CloseProfileEvent, data, options, SendOptions.SendReliable);
            FileLogger.Log($"[ClosedProfileUI] UI {targetActorNumber}에게 전송", this);
        }
        catch (Exception ex)
        {
            FileLogger.Log($"[ClosedProfileUI] 메시지 전송 실패: {ex.Message}", this);
        }
    }


    private void HandleCloseProfileEvent(EventData photonEvent)
    {
        //notificationManager.CloseProfileUI();
    }


    private void HandleSendMyUIPopUPEvent(EventData photonEvent)
    {
        //notificationManager.OpenSendAcceptPopupUI();
    }

    private void HandleSendViewProfileEvent(EventData photonEvent)
    {
        object[] data = (object[])photonEvent.CustomData;
        if (data != null && data.Length > 0)
        {
            // 전송 받은 pin 데이터
            string receivedpin = data[0] as string;
            FileLogger.Log($"[SendURLDataEvent] 수신된 pin: {receivedpin}", this);
        }
        else
        {
            FileLogger.Log("SendURLDataEvent: 전달된 데이터가 없습니다.", this);
        }
    }


    private void HandleSendURLDataEvent(EventData photonEvent)
    {
        object[] data = (object[])photonEvent.CustomData;
        if (data != null && data.Length > 0)
        {
            // 전송 받은 url 데이터
            string receivedURL = data[0] as string;
            FileLogger.Log($"[SendURLDataEvent] 수신된 URL: {receivedURL}", this);
        }
        else
        {
            FileLogger.Log("SendURLDataEvent: 전달된 데이터가 없습니다.", this);
        }
    }


    public void HandleEvent(EventData photonEvent)
    {
        FileLogger.Log($"photon event {photonEvent.Code} received", this);

        // Dictionary에서 이벤트 코드에 맞는 핸들러를 실행
        if (eventHandlers.TryGetValue(photonEvent.Code, out var handler))
        {
            handler(photonEvent);
        }
        else
        {
            FileLogger.Log($"Unhandled event code: {photonEvent.Code}", this);
        }

        if (photonEvent.Code == 254) // 포톤 유저 한 명이 접속 종료 이벤트
        {
            SyncUserListWithPhotonPlayers();
        }
    }

    // 📌 제네릭 데이터 처리 메서드 (어떤 데이터 타입이든 처리 가능)
    private T ParseEventData<T>(EventData photonEvent) where T : class
    {
        try
        {
            if (photonEvent.CustomData == null)
            {
                FileLogger.Log("CustomData is null", this);
                return null;
            }

            object[] data = photonEvent.CustomData as object[];
            if (data == null || data.Length < 1 || !(data[0] is T parsedData))
            {
                FileLogger.Log($"Invalid CustomData received or missing {typeof(T).Name}", this);
                return null;
            }

            return parsedData;
        }
        catch (Exception ex)
        {
            FileLogger.Log($"Error parsing event data: {ex.Message}", this);
            return null;
        }
    }

    // 📌 개별 이벤트 처리 메서드
    private void HandleRenameEvent(EventData photonEvent)
    {
        object[] data = (object[])photonEvent.CustomData;
        int actorNumber = (int)data[0];
        string newNickName = (string)data[1];

        Player player = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
        if (player != null)
        {
            player.NickName = newNickName; // 닉네임 업데이트
            DebugUserInfos.Instance.DebugMyUserInfo(myUserInfo);
            FileLogger.Log($"Photon Actor Number [{actorNumber}]의 닉네임이 {newNickName}(으)로 변경되었습니다.");
        }
        else
        {
            Debug.Log($"[Error] 플레이어를 찾을 수 없음! actorNumber: {actorNumber}");
        }
    }

    private void HandleSendUserInfoEvent(EventData photonEvent)
    {
        var receivedUserInfo = ParseEventData<UserInfo>(photonEvent);
        if (receivedUserInfo != null)
        {
            FileLogger.Log($"Received UserInfo: {receivedUserInfo.PhotonUserName}", this);
            // TODO: 유저 정보 저장 및 업데이트
            userInfos.Add(receivedUserInfo); // 연결된 유저 데이터가 리스트에 추가됨

            // 모든 유저 정보 시각화
            debugUserInfo.LogAllUsersInfo(ref userInfos);
            DebugUserInfos.Instance.DebugAllUsersInfo();

            // List<UserInfo> 동기화 
            BroadcastUserInfos();
            FileLogger.Log($"UserInfo received for {receivedUserInfo.PhotonUserName}", this);
        }
    }

    private void HandleSendUsersInfoEvent(EventData photonEvent)
    {
        try
        {
            FileLogger.Log($"photon event {photonEvent.Code} received", this);

            // 수신된 데이터를 배열로 역직렬화
            UserInfo[] receivedUserInfoArray = (UserInfo[])photonEvent.CustomData;

            // 배열을 List로 변환
            var receivedUserInfos = new List<UserInfo>(receivedUserInfoArray);

            foreach (var receivedUserInfo in receivedUserInfos)
            {
                // 기존 리스트에서 해당 유저 정보 찾기
                var existingUserInfo =
                    userInfos.Find(user => user.PhotonUserName == receivedUserInfo.PhotonUserName);

                if (existingUserInfo != null)
                {
                    // 기존 유저 정보 업데이트
                    existingUserInfo.CurrentRoomNumber = receivedUserInfo.CurrentRoomNumber;
                    existingUserInfo.PhotonRole = receivedUserInfo.PhotonRole;
                    existingUserInfo.CurrentState = receivedUserInfo.CurrentState;
                }
                else
                {
                    // 새로운 유저 정보 추가
                    userInfos.Add(receivedUserInfo);
                    FileLogger.Log($"Added new UserInfo: {receivedUserInfo.PhotonUserName}", this);
                }
            }

            DebugUserInfos.Instance.DebugAllUsersInfo();
            FileLogger.Log($"UserInfo list updated successfully. Total users: {userInfos.Count}", this);
        }
        catch (Exception ex)
        {
            FileLogger.Log($"Failed to handle UserInfoList event: {ex.Message}", this);
        }
    }

    private void HandleSendMatchInfoEvent(EventData photonEvent)
    {
        var receivedMatchInfo = ParseEventData<MatchInfo>(photonEvent);
        if (receivedMatchInfo != null)
        {
            FileLogger.Log($"UserWhoSend: {receivedMatchInfo.UserWhoSend}, UserWhoReceive: {receivedMatchInfo.UserWhoReceive}, MatchRequest: {receivedMatchInfo.MatchRequest}", this);
            // TODO: 매칭 요청 처리

            debugUserInfo.receivedMatchInfo = receivedMatchInfo;

            if (debugUserInfo.receivedMatchInfo.MatchRequest == "Request...") // 매칭 요청을 받음
            {
                partnerUserInfo = null;
                partnerUserInfo = new UserInfo();
                partnerUserInfo.PhotonUserName = debugUserInfo.receivedMatchInfo.UserWhoSend;
                debugUserInfo.SetMatchButtonStatus(true);
                debugUserInfo.ShowMatchRequestUI();
                HololenUIManager.Instance.AddMatchingRequestData();
                HololenUIManager.Instance.LoadMatchingSenderDataFromDB();

                //Debug.Log($"matchInfo.whoSend = {receivedMatchInfo.userWhoSend}"); -> Player2라고 제대로 나오고 있음. 근데 답장으로는 여기로 안 가고 있음..
            }
            // 매칭을 받았을 때 - Receive
            else if (debugUserInfo.receivedMatchInfo.MatchRequest == "Accept") // 매칭 응답(Yes)을 받음
            {
                partnerUserInfo.PhotonUserName = debugUserInfo.receivedMatchInfo.UserWhoSend;
                debugUserInfo.ShowReceiveAcceptUI();

                /*// TODO : 여기에 HoloenUIManager.cs로 부터 함수를 가져와 매칭에 대한 이력을 띄워야함
                HololenUIManager.Instance.AddReservedData();
                HololenUIManager.Instance.timers["12345"] = 10f;            // 임시로 시간 넣었음
                HololenUIManager.Instance.LoadReservatedDataFromDB();*/

                MatchingStateUpdateAsTrue();
                //Debug.Log("난 분명 Accept 응답을 받았다");
                //notificationManager.SendAcceptMessage();
                //Debug.Log("보낸 요청에 대해 Accept 응답을 받음!");
            }
            else if (debugUserInfo.receivedMatchInfo.MatchRequest == "Decline") // 매칭 응답(No)을 받음
            {
                debugUserInfo.ShowReceiveDeclineUI();
                //Debug.Log("난 분명 Decline 응답을 받았다");
                //notificationManager.SendDeclineMessage();
                //Debug.Log("보낸 요청에 대해 Decline 응답을 받음!");
            }
        }
    }

    private void HandleSendMeetingInfoEvent(EventData photonEvent)
    {
        var meetingInfo = ParseEventData<MeetingInfo>(photonEvent);
        if (meetingInfo != null)
        {
            FileLogger.Log($"Meeting confirmed for: {meetingInfo.MeetingDateTime}", this);
            Debug.Log("Match Key: " + meetingInfo.MatchKey);
            Debug.Log("Meeting Time: " + meetingInfo.MeetingDateTime);

            // Set MatchInfo 저장, 알람 실행 코드
            /*if (MeetingManager.Instance && PhotonNetwork.NickName.Contains("Hololens"))
            {
                MeetingManager.Instance.SetAlarmFromMeetingInfo(meetingInfo);

                HololenUIManager.Instance.AddReservedData();
                *//*HololenUIManager.Instance.timers["12345"] =
                    (float)MatchingUtils.GetRemainingMinutes(meetingInfo.MeetingDateTime) * 60; // 분 단위에서 초 단위로 변경*//*
                HololenUIManager.Instance.LoadReservatedDataFromDB();
            }
            else
            {
                FileLogger.Log("Meeting Manager is Null", this);
            }*/
        }
    }

    private void HandleSendCustomStringEvent(EventData photonEvent)
    {
        // 전송된 데이터는 object[] 형태로 전달됩니다.
        object[] data = (object[])photonEvent.CustomData;
        if (data != null && data.Length > 0)
        {
            // 첫 번째 인자가 string이라고 가정
            string receivedMessage = data[0] as string;
            FileLogger.Log($"[SendCustomStringEvent] 수신된 메시지: {receivedMessage}", this);
            // 필요하다면 여기서 UI 업데이트 등 추가 로직 실행
        }
        else
        {
            FileLogger.Log("SendCustomStringEvent: 전달된 데이터가 없습니다.", this);
        }
    }





    #endregion

    #region HostBehaviourOVERRIDE

    public override void HandleOnJoinedRoom()
    {
        // 사용자가 방에 접속할 때 myUserInfo 초기화
        FileLogger.Log("UserMatchingManager 사용자 접속, 정보 입력", this);

        myUserInfo = new UserInfo
        {
            CurrentRoomNumber = PhotonNetwork.CurrentRoom.Name,
            PhotonRole = FileLogger.GetRoleString(),
            PhotonUserName = PhotonNetwork.NickName,
            CurrentState = "None"
        };
        FileLogger.Log($"{myUserInfo.PhotonUserName}: {myUserInfo.PhotonRole}", this);
        FileLogger.Log("UserMatchingManager 사용자 접속 초기 정보 생성 완료", this);
        base.HandleOnJoinedRoom();
    }

    public override void OnBecameHost()
    {
        FileLogger.Log("UserMatchingManager 초기화 시작", this);

        // TODO: 초기화 작업 구현

        FileLogger.Log("UserMatchingManager 초기화 완료", this);

        base.OnBecameHost();
    }

    public override void OnStoppedBeingHost()
    {
        FileLogger.Log("UserMatchingManager 잉여 데이터 정리 시작", this);

        // TODO: 잉여 데이터 정리 작업 구현

        FileLogger.Log("UserMatchingManager 잉여 데이터 정리 완료", this);

        base.OnStoppedBeingHost();
    }

    #endregion

    #region MessageHandlers

    public void TrySendingUserInfo()
    {
        // 중앙 호스트에게 User Info 전송
        if (!FileLogger.GetRoleString().Contains("CentralHost"))
        {
            int centralHostActorNumber = PhotonUserUtility.GetCentralHostActorNumber();
            FileLogger.Log($"UserMatchingManager GetCentralHostActorNumber {centralHostActorNumber}", this);
            if (centralHostActorNumber != -1)
                SendUserInfo(myUserInfo, centralHostActorNumber);
        }
        else
        {
            FileLogger.Log($"UserMatchingManager GetCentralHostActorNumber {-1}", this);
        }
    }

    // User Info를 전송한다.
    public void SendUserInfo(UserInfo userInfo, int targetActorNumber)
    {
        FileLogger.Log($"Send User Info to {targetActorNumber}", this);

        // 유효성 검사: ActorNumber 확인
        if (PhotonNetwork.CurrentRoom.GetPlayer(targetActorNumber) == null)
        {
            FileLogger.Log($"Invalid targetActorNumber: {targetActorNumber}", this);
            return;
        }

        // UserInfo를 포함한 데이터 생성
        object[] data = new object[] { userInfo };

        // RaiseEventOptions 설정
        RaiseEventOptions options = new RaiseEventOptions
        {
            TargetActors = new int[] { targetActorNumber } // 지정된 ActorNumber(플레이어)에게만 전송
        };

        try
        {
            PhotonNetwork.RaiseEvent(SendUserInfoEvent, data, options, SendOptions.SendReliable);
            FileLogger.Log($"Successfully sent UserInfo to {targetActorNumber}", this);
        }
        catch (Exception ex)
        {
            FileLogger.Log($"Failed to send UserInfo: {ex.Message}", this);
        }
    }

    // MatchInfo는 DebugUserInfo에 구현되어 있음.
    // 매칭 시간에 대한 정보를 전송한다.
    public void SendMeetingInfo(MeetingInfo meetingInfo, int targetActorNumber)
    {
        FileLogger.Log($"Send Meeting Info to {targetActorNumber}", this);

        // 유효성 검사: ActorNumber 확인
        if (PhotonNetwork.CurrentRoom.GetPlayer(targetActorNumber) == null)
        {
            FileLogger.Log($"Invalid targetActorNumber: {targetActorNumber}", this);
            return;
        }

        // UserInfo를 포함한 데이터 생성
        object[] data = new object[] { meetingInfo };

        // RaiseEventOptions 설정
        RaiseEventOptions options = new RaiseEventOptions
        {
            TargetActors = new int[] { targetActorNumber } // 지정된 ActorNumber(플레이어)에게만 전송
        };

        try
        {
            PhotonNetwork.RaiseEvent(SendMeetingInfoEvent, data, options, SendOptions.SendReliable);
            //MeetingManager.Instance.SetAlarmFromMeetingInfo(meetingInfo);

            FileLogger.Log($"Successfully sent Meeting Info to {targetActorNumber}", this);
        }
        catch (Exception ex)
        {
            FileLogger.Log($"Failed to send Meeting Info: {ex.Message}", this);
        }
    }

    // 내 포톤 닉네임 변경을 다른 사람에게 알린다.
    public void UpdateNickNameAfterJoin(string newNickName)
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.NickName = newNickName;
            myUserInfo.PhotonUserName = newNickName;

            // 닉네임 변경 브로드캐스트
            object[] content = new object[] { PhotonNetwork.LocalPlayer.ActorNumber, newNickName };
            RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(1, content, options, SendOptions.SendReliable);
        }
        else
        {
            FileLogger.Log("닉네임을 변경하려면 방에 입장해야 합니다.", this);
        }
    }

    public void SyncUserListWithPhotonPlayers()
    {
        // Photon에 접속 중인 모든 유저 이름 가져오기
        HashSet<string> connectedPlayerNames = new HashSet<string>();
        foreach (var player in PhotonNetwork.PlayerList)
        {
            connectedPlayerNames.Add(player.NickName);
        }

        // userInfos 리스트에서 Photon에 존재하지 않는 유저 제거
        userInfos.RemoveAll(user => !connectedPlayerNames.Contains(user.PhotonUserName));

        FileLogger.Log($"UserInfos synced. Remaining users: {userInfos.Count}", this);
    }

    public void BroadcastUserInfos()
    {
        FileLogger.Log("Broadcasting user info list to all clients", this);

        // List<UserInfo>를 배열로 변환
        var userInfoArray = userInfos.ToArray();

        try
        {
            PhotonNetwork.RaiseEvent(
                SendUsersInfoEvent,
                userInfoArray, // 배열로 전송
                new RaiseEventOptions { Receivers = ReceiverGroup.All }, // 모든 클라이언트에게 전송
                SendOptions.SendReliable
            );
            FileLogger.Log("Successfully sent user info list to all clients", this);
        }
        catch (Exception ex)
        {
            FileLogger.Log($"Failed to send user info list: {ex.Message}", this);
        }
    }


    //============ SM ADD ============//
    public void MatchingStateUpdateAsTrue()
    {
        isMatchingSucceed = true;
        // 버튼들에 반드시 할당할 것!
    }

    // 프로퍼티로 추가
    public bool isMatchingSucceed
    {
        get => _isMatchingSucceed;
        set
        {
            if (_isMatchingSucceed != value) // 값이 변경되었는지 확인
            {
                _isMatchingSucceed = value;

                // 값이 true로 바뀌었을 때만 UI를 띄움
                if (_isMatchingSucceed)
                {
                    Debug.Log($"myGameObject.name: {myUserInfo.PhotonUserName}");
                    Debug.Log($"partnerGameObject.name: {debugUserInfo.receivedMatchInfo.UserWhoSend}");

                    FileLogger.Log($"Try Find Partner GameObject: {debugUserInfo.receivedMatchInfo.UserWhoSend}", this);

                    myGameObject = GameObject.Find($"{myUserInfo.PhotonUserName}");
                    partnerGameObject = GameObject.Find($"{debugUserInfo.receivedMatchInfo.UserWhoSend}");

                    if (partnerGameObject)
                    {
                        FileLogger.Log($"Found Partner GameObject: {partnerGameObject.name}", this);
                    }
                    else
                    {
                        FileLogger.Log($"Not Found Partner GameObject:", this);
                    }

                    HololenUIManager.Instance.ShowMeetingUI();
                    isTravelingToMeet = true;
                }
            }
        }
    }

    public void TravelingStart()
    {
        isTravelingToMeet = true;
    }

    public void TravelingEnd()
    {
        isTravelingToMeet = false;
    }

    public bool isTravelingToMeet
    {
        get => _isTravelingToMeet;
        set
        {
            if (_isTravelingToMeet != value) // 값 변경 확인
            {
                //Debug.Log("값이 바뀜!");
                _isTravelingToMeet = value;

                if (_isTravelingToMeet) // 만나기 위해 이동 중이 true라면
                {
                    OnTravelingToMeet?.Invoke(); // 이벤트 트리거
                    StartTravelingLoop(); // 반복 작업 시작
                }
                else
                {
                    StopTravelingLoop(); // 반복 작업 중단
                }
            }
        }
    }

    private async void StartTravelingLoop()
    {
        _isLoopRunning = true;

        while (_isTravelingToMeet && _isLoopRunning)
        {
            Vector3 temp = myGameObject.transform.position - partnerGameObject.transform.position;
            HololenUIManager.Instance.UpdateRouteUI(temp, myGameObject.transform.rotation.eulerAngles.y);
            await Task.Delay(500); // 1초 대기

            // 만남 종료 - 테스트를 위해 0.5m으로 잡음
            if (temp.magnitude < 0.5)
            {
                isTravelingToMeet = false;
            }
        }
    }

    private void StopTravelingLoop()
    {
        _isLoopRunning = false;
        HololenUIManager.Instance.HideRouteUI();
    }
    //============ SM ADD ============//
    #endregion
}