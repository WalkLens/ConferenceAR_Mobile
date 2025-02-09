using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Diagnostics;
using System;
using System.Linq;
using Unity.VisualScripting;

public class HololenUIManager : MonoBehaviour
{
    public static HololenUIManager Instance;

    public NotificationManager notificationManager;

    [Header("MainBar")]
    public TextMeshProUGUI currentTime;

    [Space]
    [Header("Calendar_L")]
    public TextMeshProUGUI[] SenderDataText;
    public List<GameObject> requestCanvas;

    [Space]
    [Header("Calendar_R")]
    public TextMeshProUGUI[] ReservatedDataText;
    public Image ReservatedDataCircleTimer;
    public TextMeshProUGUI[] RecentMetDataText;

    [Space]
    [Header("Base UI Canvas")]
    public GameObject MatchingRequestDataPrefab;
    public Transform MatchingRequestDataParent;
    public List<GameObject> MatchingRequestData;                   // ��Ī ��û�� ���� ������ ����Ʈ�� �߰��ž� ��
    public GameObject ReservedDataPrefab;
    public Transform ReservedDataParent;
    public List<GameObject> ReservedData;                           // ��Ī ��û�� ó���� ������ ����Ʈ�� �߰��ž� ��

    public Dictionary<string, float> timers = new Dictionary<string, float>();
    public bool isTimerUpdated;
    private int time = 0;

    [Space]
    [Header("MatchingRequest")]
    public TextMeshProUGUI[] SenderDetailsText;
    [SerializeField] GameObject matchingStartPopupUI;


    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }


    // Update is called once per frame
    void Update()
    {
        // �ð� ����
        currentTime.text = DateTime.Now.ToString(("hh:mm tt"));

        LoadReservatedDataUpdate();

        // DB ����
        if (isTimerUpdated)
        {
            timers["12345"] = (float)GetTime();
            LoadReservatedDataFromDB();
            LoadRecentMetDataFromDB();

            isTimerUpdated = false;
            SetTime(0);
        }

        //if (Input.GetKey(KeyCode.M))                            // ��Ī ��û�� �� ���� �����ϴ� �Է�
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

        // TODO : �̹���, ���� ���µ� �ε� �ʿ�
    }

    //=================== Calendar_R =================//
    public void LoadReservatedDataFromDB()
    {
        ReservatedDataText[2].text = DatabaseManager.Instance.getUserData("12345").name;
        ReservatedDataText[3].text = DatabaseManager.Instance.getUserData("12345").job;
        // TODO : �̹���, ���� ���µ� �ε� �ʿ�
    }

    public void LoadReservatedDataUpdate()
    {
        List<string> keys = new List<string>(timers.Keys);                      // ����� �����鿡 ���ؼ� Ÿ�̸� �۵�
        foreach (string key in keys)
        {
            if (timers[key] >= 3600)
            {
                timers[key] -= Time.deltaTime;
                ReservatedDataText[0].text = (timers[key] / 3600).ToString("F0") + "시간 " + ((timers[key] % 3600) / 60).ToString("F0") + "분 이내";
                ReservatedDataText[1].text = DateTime.Now.AddSeconds(timers[key]).ToString("hh:mm tt");     // !!! ��� ������ �ʿ� �����Ƿ� ���Ŀ� ������ ������
                ReservatedDataCircleTimer.fillAmount = timers[key] / (4 * 60 * 60);
            }
            else if (timers[key] >= 60)
            {
                timers[key] -= Time.deltaTime;
                ReservatedDataText[0].text = (timers[key] / 60).ToString("F0") + "분 이내";
                ReservatedDataText[1].text = DateTime.Now.AddSeconds(timers[key]).ToString("hh:mm tt");     // !!! ��� ������ �ʿ� �����Ƿ� ���Ŀ� ������ ������
                ReservatedDataCircleTimer.fillAmount = timers[key] / (4 * 60 * 60);
            }
            else if (timers[key] >= 0)
            {
                timers[key] -= Time.deltaTime;
                ReservatedDataText[0].text = timers[key].ToString("F0") + "초 이내";
                ReservatedDataText[1].text = DateTime.Now.AddSeconds(timers[key]).ToString("hh:mm tt");     // !!! ��� ������ �ʿ� �����Ƿ� ���Ŀ� ������ ������
                ReservatedDataCircleTimer.fillAmount = timers[key] / (4 * 60 * 60);
            }
            else
            {
                if (timers[key] != -1)
                {
                    timers[key] = -1; // �ٸ� �������� �´� ���� �߰� �ؾ���
                    OpenMatchingStartPopupUI();
                    RemoveReservedData();
                }
                ReservatedDataText[1].text = DateTime.Now.AddSeconds(timers[key]).ToString("hh:mm tt");     // !!! ��� ������ �ʿ� �����Ƿ� ���Ŀ� ������ ������
                ReservatedDataCircleTimer.fillAmount = timers[key] / (4 * 60 * 60);
            }
        }
    }

    public void LoadRecentMetDataFromDB()
    {
        RecentMetDataText[0].text = DatabaseManager.Instance.getUserData("12345").name;

        // TODO : �̹���, ���� ���µ� �ε� �ʿ�
    }



    public void AddMatchingRequestData()                                        // !!! ����� ���� ����� �κ�
    {
        GameObject newObject = Instantiate(MatchingRequestDataPrefab);

        // �θ�� MatchingRequestData�� ����
        newObject.transform.SetParent(MatchingRequestDataParent);

        int index = MatchingRequestDataParent.childCount;
        newObject.name = "MatchingRequestData_" + index;
        newObject.transform.localPosition = Vector3.zero;
        newObject.transform.localScale = Vector3.one;
        newObject.transform.localRotation = Quaternion.identity;

        // MatchingRequestData ����Ʈ�� �߰�
        MatchingRequestData.Add(newObject);

        // �ؽ�Ʈ�� ����
        Transform senderNameObject = newObject.transform.Find("Data0/ProfileBaseData - V/Name");
        Transform senderPositionObject = newObject.transform.Find("Data0/ProfileBaseData - V/Position|Team");
        SenderDataText[0] = senderNameObject.GetComponent<TextMeshProUGUI>();
        SenderDataText[1] = senderPositionObject.GetComponent<TextMeshProUGUI>();

        // ��ư�鿡 ��� ����
        Transform sendMatchingRequestObject = newObject.transform.Find("Buttons - H/Send Matching Request");
        Transform declineObject = newObject.transform.Find("Buttons - H/Decline");
        Transform timePlusObject = newObject.transform.Find("Buttons - H/TimePlus");
        //Transform expandObject = newObject.transform.Find("Data0/Expand");

        Button[] MatchingRequestButton = new Button[4];
        MatchingRequestButton[0] = sendMatchingRequestObject.GetComponent<Button>();
        MatchingRequestButton[1] = declineObject.GetComponent<Button>();
        MatchingRequestButton[2] = timePlusObject.GetComponent<Button>();
        //MatchingRequestButton[3] = expandObject.GetComponent<PressableButton>();

        MatchingRequestButton[0].onClick.AddListener(() =>
        {
            RemoveMatchingRequestData(newObject);
            notificationManager.SendAcceptMessage();
            AddReservedData();                                  // ���濡�� ���� ��û ���� ��, UI�� ǥ��

            MeetTimeUpdate();

            MatchingRequestButton[0].onClick.RemoveAllListeners();
            MatchingRequestButton[2].onClick.RemoveAllListeners();
        });

        MatchingRequestButton[1].onClick.AddListener(() =>
        {
            RemoveMatchingRequestData(newObject);
            notificationManager.SendDeclineMessage();
            SetTime(0);

            MatchingRequestButton[1].onClick.RemoveAllListeners();
            MatchingRequestButton[2].onClick.RemoveAllListeners();
        });

        MatchingRequestButton[2].onClick.AddListener(() =>
        {
            MeetTimePlus();

        });

        /*MatchingRequestButton[3].OnClicked.AddListener(() =>
        {
            notificationManager.OpenProfileUI();
            LoadMatchingSenderDetailsFromDB();
            
            MatchingRequestButton[3].OnClicked.RemoveAllListeners();
        });*/

        requestCanvas.Add(newObject);
    }

    public void RemoveMatchingRequestData(GameObject objectToRemove)
    {
        if (MatchingRequestData.Contains(objectToRemove)) // �ش� ������Ʈ�� ����Ʈ�� ���ԵǾ� �ִ��� Ȯ��
        {
            MatchingRequestData.Remove(objectToRemove);        // ����Ʈ���� �ش� ������Ʈ ����
            Destroy(objectToRemove);                           // ������ �ش� ������Ʈ ����
        }
    }

    public void AddReservedData()
    {
        Debug.Log("Add Reserved Data");
        GameObject newObject = Instantiate(ReservedDataPrefab);

        //// �θ�� MatchingRequestData�� ����
        newObject.transform.SetParent(ReservedDataParent);

        int index = ReservedDataParent.childCount;
        newObject.name = "ReservedData_" + index;
        newObject.transform.localPosition = Vector3.zero;
        newObject.transform.localScale = Vector3.one;
        newObject.transform.localRotation = Quaternion.identity;

        ReservedData.Add(newObject);

        // �ؽ�Ʈ�� ����
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
            GameObject lastItem = ReservedData[ReservedData.Count - 1]; // 마지막 오브젝트 가져오기
            ReservedData.RemoveAt(ReservedData.Count - 1); // 리스트에서 제거
            Destroy(lastItem); // 오브젝트 삭제
        }
        else
        {
            Debug.LogWarning("ReservedData 리스트가 비어 있습니다.");
        }
        /*if (ReservedData.Count > 0)
        {
            ReservedData.RemoveAt(ReservedData.Count-1);        // ù ��° ��Ҹ� ����Ʈ���� ����
            Destroy(ReservedData[ReservedData.Count-1]);        // ������ ù ��° ����� ���� ������Ʈ�� ����
        }*/
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

        // TODO : �̹���, ���� ���µ� �ε� �ʿ�
    }



    //=================== Time  =================//
    public void MeetTimePlus()
    {
        time += 600;
        Debug.Log(time);
    }

    public void MeetTimeMinus()
    {
        int temp = time;
        temp -= 600;
        if (temp > 0) { time = temp; }
        else { time = 0; }

        Debug.Log(time);
    }

    public void MeetTimeUpdate()
    {
        isTimerUpdated = true;

        //MeetingManager.Instance.SetAndSendMeetingInfo(time);
    }

    public int GetTime()
    {
        return time;
    }

    public void SetTime(int newTime)
    {
        time = newTime;
    }




    //=================== Matching  =================//
    public void ShowMeetingUI()
    {
        Debug.Log("�� �ð�ȭ ����!");
        Vector3 temp = UserMatchingManager.Instance.partnerGameObject.transform.position - UserMatchingManager.Instance.myGameObject.transform.position;
        Debug.Log(temp.magnitude);              // 250205 : UI�� ��m ���Ҵ��� ǥ���� �� ����ؾ��� ��
    }

    public void UpdateRouteUI(Vector3 direction, float myRotY)
    {
        notificationManager.UpdateRouteVisualizationUI(direction, myRotY);
    }
    public void HideRouteUI()
    {
        notificationManager.CloseRouteVisualizationUI();
    }
    public void OpenMatchingStartPopupUI()
    {
        matchingStartPopupUI.SetActive(true);
    }
    // !!!!         2
}
