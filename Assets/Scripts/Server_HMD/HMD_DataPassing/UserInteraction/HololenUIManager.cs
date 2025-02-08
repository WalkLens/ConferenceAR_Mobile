using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

using UnityEngine.Diagnostics;
using System;
using Unity.VisualScripting;


public class HololenUIManager : MonoBehaviour
{
    public NotificationManager notificationManager;

    [Header("MainBar")]
    public TextMeshProUGUI currentTime;

    [Space]
    [Header("Calendar_L")]
    public TextMeshProUGUI[] SenderDataText;

    [Space]
    [Header("Calendar_R")]
    public TextMeshProUGUI[] ReservatedDataText;
    public Image ReservatedDataCircleTimer;
    public TextMeshProUGUI[] RecentMetDataText;

    [Space]
    [Header("Base UI Canvas")]
    public GameObject MatchingRequestDataPrefab;
    public Transform MatchingRequestDataParent;
    public List<GameObject> MatchingRequestData;                   // 매칭 요청이 들어올 때마다 리스트에 추가돼야 함
    public GameObject ReservedDataPrefab;
    public Transform ReservedDataParent;
    public List<GameObject> ReservedData;                           // 매칭 요청을 처리할 때마다 리스트에 추가돼야 함

    private Dictionary<string, float> timers = new Dictionary<string, float>();


    [Space]
    [Header("MatchingRequest")]
    public TextMeshProUGUI[] SenderDetailsText;


    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        // 시간 관련
        currentTime.text = DateTime.Now.ToString(("hh:mm tt"));

        LoadReservatedDataUpdate();

        // DB 관련
        if (notificationManager.isTimerUpdated)
        {
            timers["12345"] = (float)notificationManager.GetTime();
            LoadReservatedDataFromDB();
            LoadRecentMetDataFromDB();

            notificationManager.isTimerUpdated = false;
            notificationManager.SetTime(0);
        }

        //if (Input.GetKey(KeyCode.M))                            // 매칭 요청이 온 것을 가정하는 입력
        //{
        //    AddMatchingRequestData();
        //    LoadMatchingSenderDataFromDB();
        //}
    }

    //=================== Calendar_L =================//
    public void LoadMatchingSenderDataFromDB()
    {
        SenderDataText[0].text = DatabaseManager.Instance.getUserData("12345").name;
        SenderDataText[1].text = DatabaseManager.Instance.getUserData("12345").job;

        // TODO : 이미지, 접속 상태도 로드 필요
    }

    //=================== Calendar_R =================//
    public void LoadReservatedDataFromDB()
    {
        ReservatedDataText[2].text = DatabaseManager.Instance.getUserData("12345").name;
        ReservatedDataText[3].text = DatabaseManager.Instance.getUserData("12345").job;
        // TODO : 이미지, 접속 상태도 로드 필요
    }

    public void LoadReservatedDataUpdate()
    {
        List<string> keys = new List<string>(timers.Keys);                      // 예약된 유저들에 대해서 타이머 작동
        foreach (string key in keys)
        {
            if (timers[key] >= 3600)
            {
                timers[key] -= Time.deltaTime;
                ReservatedDataText[0].text = (timers[key] / 3600).ToString("F0") + "시간 " + ((timers[key]% 3600) / 60).ToString("F0") + "분 후";
                ReservatedDataText[1].text = DateTime.Now.AddSeconds(timers[key]).ToString("hh:mm tt");     // !!! 계속 연산할 필요 없으므로 이후에 밖으로 빼야함
                ReservatedDataCircleTimer.fillAmount = timers[key] / (3 * 60 * 60);
            }
            else if (timers[key] >= 60)
            {
                timers[key] -= Time.deltaTime;
                ReservatedDataText[0].text = (timers[key] / 60).ToString("F0") + "분 후";
                ReservatedDataText[1].text = DateTime.Now.AddSeconds(timers[key]).ToString("hh:mm tt");     // !!! 계속 연산할 필요 없으므로 이후에 밖으로 빼야함
                ReservatedDataCircleTimer.fillAmount = timers[key] / (3 * 60 * 60);
            }
            else if (timers[key] >= 0)
            {
                timers[key] -= Time.deltaTime;
                ReservatedDataText[0].text = timers[key].ToString("F0") + "초 후";
                ReservatedDataText[1].text = DateTime.Now.AddSeconds(timers[key]).ToString("hh:mm tt");     // !!! 계속 연산할 필요 없으므로 이후에 밖으로 빼야함
                ReservatedDataCircleTimer.fillAmount = timers[key] / (3 * 60 * 60);
            }
            else
            {
                notificationManager.OpenMatchingStartPopupUI();                 // 다른 유저별로 맞는 것이 뜨게 해야함
                ReservatedDataText[1].text = DateTime.Now.AddSeconds(timers[key]).ToString("hh:mm tt");     // !!! 계속 연산할 필요 없으므로 이후에 밖으로 빼야함
                ReservatedDataCircleTimer.fillAmount = timers[key] / (3 * 60 * 60);
            }
        }
    }

    public void LoadRecentMetDataFromDB()
    {
        RecentMetDataText[0].text = DatabaseManager.Instance.getUserData("12345").name;

        // TODO : 이미지, 접속 상태도 로드 필요
    }



    public void AddMatchingRequestData()                                        // !!! 포톤과 같이 사용할 부분
    {
        GameObject newObject = Instantiate(MatchingRequestDataPrefab);

        // 부모로 MatchingRequestData를 설정
        newObject.transform.SetParent(MatchingRequestDataParent);

        int index = MatchingRequestDataParent.childCount;
        newObject.name = "MatchingRequestData_" + index;
        newObject.transform.localPosition = Vector3.zero;
        newObject.transform.localScale = Vector3.one;
        newObject.transform.localRotation = Quaternion.identity;

        // MatchingRequestData 리스트에 추가
        MatchingRequestData.Add(newObject);

        // 텍스트들 연결
        Transform senderNameObject = newObject.transform.Find("Data0/ProfileBaseData - V/Name");
        Transform senderPositionObject = newObject.transform.Find("Data0/ProfileBaseData - V/Position|Team");
        SenderDataText[0] = senderNameObject.GetComponent<TextMeshProUGUI>();
        SenderDataText[1] = senderPositionObject.GetComponent<TextMeshProUGUI>();

        // 버튼들에 기능 세팅
        Transform sendMatchingRequestObject = newObject.transform.Find("Buttons - H/Send Matching Request");
        Transform declineObject = newObject.transform.Find("Buttons - H/Decline");
        Transform timePlusObject = newObject.transform.Find("Buttons - H/TimePlus");
        Transform expandObject = newObject.transform.Find("Data0/Expand");

        Button[] MatchingRequestButton = new Button[4];
        MatchingRequestButton[0] = sendMatchingRequestObject.GetComponent<Button>();
        MatchingRequestButton[1] = declineObject.GetComponent<Button>();
        MatchingRequestButton[2] = timePlusObject.GetComponent<Button>();
        MatchingRequestButton[3] = expandObject.GetComponent<Button>();

        MatchingRequestButton[0].onClick.AddListener(() =>
        {
            notificationManager.MeetTimeUpdate();
            RemoveMatchingRequestData(newObject);
            notificationManager.SendAcceptMessage();
            AddReservedData();                                  // 상대방에게 받은 요청 수락 후, UI에 표시
            notificationManager.MeetTimeUpdate();
        });

        MatchingRequestButton[1].onClick.AddListener(() =>
        {
            RemoveMatchingRequestData(newObject);
            notificationManager.SendDeclineMessage();
        });

        MatchingRequestButton[2].onClick.AddListener(() =>
        {
            notificationManager.MeetTimePlus();
        });

        MatchingRequestButton[3].onClick.AddListener(() =>
        {
            notificationManager.OpenProfileUI();
            LoadMatchingSenderDetailsFromDB();
        });
    }

    public void RemoveMatchingRequestData(GameObject objectToRemove)
    {
        if (MatchingRequestData.Contains(objectToRemove)) // 해당 오브젝트가 리스트에 포함되어 있는지 확인
        {
            MatchingRequestData.Remove(objectToRemove);        // 리스트에서 해당 오브젝트 제거
            Destroy(objectToRemove);                           // 씬에서 해당 오브젝트 삭제
        }
    }

    public void AddReservedData()
    {
        GameObject newObject = Instantiate(ReservedDataPrefab);

        //// 부모로 MatchingRequestData를 설정
        newObject.transform.SetParent(ReservedDataParent);

        int index = ReservedDataParent.childCount;
        newObject.name = "ReservedData_" + index;
        newObject.transform.localPosition = Vector3.zero;
        newObject.transform.localScale = Vector3.one;
        newObject.transform.localRotation = Quaternion.identity;

        ReservedData.Add(newObject);

        // 텍스트들 연결
        Transform leftTimeObject = newObject.transform.Find("DataContainer/TimeData/ProfileBaseData - V/LeftTime");
        Transform futureTimeObject = newObject.transform.Find("DataContainer/TimeData/ProfileBaseData - V/FutureTime");
        Transform timePlusObject = newObject.transform.Find("DataContainer/UserData/ProfileBaseData - V/Name");
        Transform positionObject = newObject.transform.Find("DataContainer/UserData/ProfileBaseData - V/Position|Team");
        Transform timerCircleObject = newObject.transform.Find("DataContainer/TimeData/TimerBackground/Timer");

        ReservatedDataText = new TextMeshProUGUI[4];
        ReservatedDataText[0] = leftTimeObject.GetComponent<TextMeshProUGUI>();
        ReservatedDataText[1] = futureTimeObject.GetComponent<TextMeshProUGUI>();
        ReservatedDataText[2] = timePlusObject.GetComponent<TextMeshProUGUI>();
        ReservatedDataText[3] = positionObject.GetComponent<TextMeshProUGUI>();
        ReservatedDataCircleTimer = timerCircleObject.GetComponent<Image>();
    }

    public void RemoveReservedData()
    {
        if (ReservedData.Count > 0)
        {
            ReservedData.RemoveAt(0);        // 첫 번째 요소를 리스트에서 제거
            Destroy(ReservedData[0]);        // 씬에서 첫 번째 요소의 게임 오브젝트를 삭제
        }
    }



    //=================== MatchingRequest =================//
    public void LoadMatchingSenderDetailsFromDB()
    {
        SenderDetailsText[0].text = DatabaseManager.Instance.getUserData("12345").name;
        SenderDetailsText[1].text = DatabaseManager.Instance.getUserData("12345").job;
        SenderDetailsText[2].text = DatabaseManager.Instance.getUserData("12345").introduction_text;
        SenderDetailsText[3].text = DatabaseManager.Instance.getUserData("12345").introduction_1;
        SenderDetailsText[4].text = DatabaseManager.Instance.getUserData("12345").introduction_2;
        SenderDetailsText[5].text = DatabaseManager.Instance.getUserData("12345").introduction_3;
        SenderDetailsText[6].text = DatabaseManager.Instance.getUserData("12345").introduction_4;
        SenderDetailsText[7].text = DatabaseManager.Instance.getUserData("12345").introduction_5;
        SenderDetailsText[8].text = DatabaseManager.Instance.getUserData("12345").interest_1;
        SenderDetailsText[9].text = DatabaseManager.Instance.getUserData("12345").interest_2;
        SenderDetailsText[10].text = DatabaseManager.Instance.getUserData("12345").interest_3;
        SenderDetailsText[11].text = DatabaseManager.Instance.getUserData("12345").interest_4;
        SenderDetailsText[12].text = DatabaseManager.Instance.getUserData("12345").interest_5;
        SenderDetailsText[13].text = DatabaseManager.Instance.getUserData("12345").url;

        // TODO : 이미지, 접속 상태도 로드 필요
    }
}
