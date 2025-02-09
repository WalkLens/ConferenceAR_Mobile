using System;
using System.Collections;
using System.Collections.Generic;
using CustomLogger;
using UnityEngine;

public class MeetingManager : MonoBehaviour
{
    public static MeetingManager Instance;

    public MeetingInfo currentMeetingInfo;
    private Coroutine notificationCoroutine;
    private float meetingTimeLeft = 0f; // 현재 남은 시간
    private bool isNotificationScheduled = false; // 알림 예약 여부

    [Header("Meeting Info(Input)")] 
    public float meetingTimeLeftSelected;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    #region 미팅 예약, 시간에 따른 메서드 호출

    // Meeting Info에 따라 동작 수행
    public void SetAlarmFromMeetingInfo(MeetingInfo meetingInfo)
    {
        // 기존 예약된 알림이 있으면 취소
        if (isNotificationScheduled && notificationCoroutine != null)
        {
            StopCoroutine(notificationCoroutine);
            isNotificationScheduled = false;
            currentMeetingInfo = null;
        }

        // 새로운 meetingInfo를 설정하고 알림 예약
        //meetingTimeLeft = MatchingUtils.GetRemainingMinutes(meetingInfo.MeetingDateTime);
        currentMeetingInfo = meetingInfo;

        if (meetingTimeLeft > 0)
        {
            notificationCoroutine = StartCoroutine(ScheduleNotification(meetingTimeLeft));
            isNotificationScheduled = true;
            FileLogger.Log($"{meetingTimeLeft}분 후 알림 예약됨.", this);
        }
        else
        {
            FileLogger.Log("⚠ 알림 예약 실패: 시간이 이미 지남", this);
        }
    }

    private IEnumerator ScheduleNotification(float timeLeft)
    {
        yield return new WaitForSeconds(timeLeft * 60); // timeLeft 분 후 알림 실행
        ShowNotification();
    }

    private void ShowNotification()
    {
        FileLogger.Log("🔔 Meeting 시간 도달! 알림 실행!", this);
        isNotificationScheduled = false;
        currentMeetingInfo = null;

        // TODO: 지정 시간에 도달, 일정 동작 수행하기
        HololenUIManager.Instance.OpenMatchingStartPopupUI();
    }
    
    // 
    public void SetAndSendMeetingInfo(float timeLeft = 0)
    {
        meetingTimeLeft = timeLeft / 60;
        
        string userA = DebugUserInfos.Instance.receivedMatchInfo.UserWhoSend ?? "No User A"; // 마지막 값이 없으면 기본 문자열 반환
        string userB = DebugUserInfos.Instance.receivedMatchInfo.UserWhoReceive ?? "No User B"; // 마지막 값이 없으면 기본 문자열 반환

        /*MeetingInfo newMeetingInfo = MatchingUtils.GetMeetingInfo(userA, userB, meetingTimeLeft); 
        
        Debug.Log($"MatchKey: {newMeetingInfo.MatchKey}, MeetingDateTime: {newMeetingInfo.MeetingDateTime}");
        Debug.Log("만나는 시간: "+ MatchingUtils.ConvertStringToDateTime(newMeetingInfo.MeetingDateTime));
        Debug.Log("만나기 까지 남은 시간: "+MatchingUtils.GetRemainingMinutes(newMeetingInfo.MeetingDateTime));*/
        
        //UserMatchingManager.Instance.SendMeetingInfo(newMeetingInfo, PhotonUserUtility.GetPlayerActorNumber(userA));
    }
    #endregion
}