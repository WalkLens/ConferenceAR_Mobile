using CustomLogger;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace MRTK.Tutorials.MultiUserCapabilities
{
    public class PhotonRoomConferenceAR : MonoBehaviourPunCallbacks, IInRoomCallbacks
    {
        public static PhotonRoomConferenceAR Room;

        [SerializeField] private GameObject photonUserPrefab = default;
        //[SerializeField] private GameObject roverExplorerPrefab = default;
        //[SerializeField] private Transform roverExplorerLocation = default;

        // private PhotonView pv;
        private Player[] photonPlayers;
        private int playersInRoom;
        private int myNumberInRoom;

        // private GameObject module;
        // private Vector3 moduleLocation = Vector3.zero;

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            photonPlayers = PhotonNetwork.PlayerList;
            playersInRoom++;
        }

        private void Awake()
        {
            if (Room == null)
            {
                Room = this;
            }
            else
            {
                if (Room != this)
                {
                    Destroy(Room.gameObject);
                    Room = this;
                }
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            PhotonNetwork.AddCallbackTarget(this);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        private void Start()
        {
            // pv = GetComponent<PhotonView>();

            // Allow prefabs not in a Resources folder
            if (PhotonNetwork.PrefabPool is DefaultPool pool)
            {
                if (photonUserPrefab != null) pool.ResourceCache.Add(photonUserPrefab.name, photonUserPrefab);

                //if (roverExplorerPrefab != null) pool.ResourceCache.Add(roverExplorerPrefab.name, roverExplorerPrefab);
            }
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();

            photonPlayers = PhotonNetwork.PlayerList;
            playersInRoom = photonPlayers.Length;
            myNumberInRoom = playersInRoom;
            //PhotonNetwork.NickName = myNumberInRoom.ToString();

            #region Cddd

            // For Debugging
            FileLogger.Log("PhotonLobbyConferenceAR.OnJoinedRoom()", this);
            FileLogger.Log("Current room name: " + PhotonNetwork.CurrentRoom.Name, this);
            FileLogger.Log("Other players in room: " + PhotonNetwork.CountOfPlayersInRooms, this);
            FileLogger.Log("Total players in room: " + (PhotonNetwork.CountOfPlayersInRooms + 1), this);

            //string newNickName = "Player_" + PhotonNetwork.LocalPlayer.ActorNumber;
            string newNickName = PhotonLobbyConferenceAR.Lobby.input_PIN + "_";

            // 플랫폼별 실행 코드
#if UNITY_IOS || UNITY_ANDROID
            FileLogger.Log("?? Running on Mobile (iOS or Android)", this);
            newNickName += "Mobile";
#elif UNITY_WSA || UNITY_WINRT
            FileLogger.Log("?? Running on UWP (Windows Store App)", this);
            newNickName += "Hololens";
#elif UNITY_EDITOR
            FileLogger.Log(
                $"?? Running in Unity Editor, 지정된 빌드 옵션(0: mobile, 1: hololens)=> {DebugBuildOptionManager.Instance.buildOptions}를 이용해 이름을 수정합니다.",
                this);
            if (DebugBuildOptionManager.Instance.buildOptions == DebugBuildOptionManager.BuildOptions.Mobile)
            {
                newNickName += "Mobile";
            }
            else if (DebugBuildOptionManager.Instance.buildOptions == DebugBuildOptionManager.BuildOptions.HoloLens)
            {
                newNickName += "HoloLens";
            }
#else
            FileLogger.Log($"?? Running in Unity Editor, 지정된 빌드 옵션(0: mobile, 1: hololens)=> {DebugBuildOptionManager.Instance.buildOptions}를 이용해 이름을 수정합니다.", this);
            if (DebugBuildOptionManager.Instance.buildOptions == DebugBuildOptionManager.BuildOptions.Mobile)
            {
                newNickName += "Mobile";
            }else if (DebugBuildOptionManager.Instance.buildOptions == DebugBuildOptionManager.BuildOptions.HoloLens)
            {
                newNickName += "HoloLens";
            }
            Debug.Log("?? Running on an Window platform");
#endif

            UserMatchingManager.Instance.UpdateNickNameAfterJoin(newNickName);
            //UserMatchingManager.Instance.TrySendingUserInfo();

            FileLogger.Log($"포톤 룸에 입장 완료: |닉네입 설정: [{newNickName}] |유저 정보 동기화 시도 ", this);

            #endregion


            //UserMatchingManager.Instance.UpdateNickNameAfterJoin(newNickName);

            // 예: PhotonUserConferenceAR.cs의 Start() 또는 적절한 위치에서 실행
            if (PhotonNetwork.NickName.Contains("HoloLens"))
            {
                // 예를 들어 "12345_HoloLens"라면 PIN은 "12345"입니다.
                string pin = PhotonNetwork.NickName.Split('_')[0];
                // 모바일 클라이언트 닉네임은 "12345_Mobile"이라고 가정합니다.
                string mobileUserNick = $"{pin}_Mobile";

                // 유틸리티 함수를 사용해 해당 닉네임의 ActorNumber를 가져옵니다.
                int targetActorNumber = PhotonUserUtility.GetPlayerActorNumber(mobileUserNick);
                if (targetActorNumber != -1)
                {
                    // UserMatchingManager의 전송 메서드를 호출해 "Successssssssss"를 보냅니다.
                    UserMatchingManager.Instance.SendCustomStringToTarget(targetActorNumber, "Successssssssss");
                }
                else
                {
                    Debug.LogError($"모바일 사용자 {mobileUserNick}의 ActorNumber를 찾을 수 없습니다.");
                }
            }


            UserMatchingManager.Instance.TrySendingUserInfo();
            FileLogger.Log($"포톤 룸에 입장 완료: |닉네임 설정: [{newNickName}] |유저 정보 동기화 시도 ", this);

            StartGame();


        }

        private void StartGame()
        {
            CreatPlayer();

            if (!PhotonNetwork.IsMasterClient) return;

            //if (TableAnchor.Instance != null) CreateInteractableObjects();
        }

        private void CreatPlayer()
        {
            var player = PhotonNetwork.Instantiate(photonUserPrefab.name, Vector3.zero, Quaternion.identity);
        }

        /*private void CreateInteractableObjects()
        {
            var position = roverExplorerLocation.position;
            var positionOnTopOfSurface = new Vector3(position.x, position.y + roverExplorerLocation.localScale.y / 2,
                position.z);

            var go = PhotonNetwork.Instantiate(roverExplorerPrefab.name, positionOnTopOfSurface,
                roverExplorerLocation.rotation);
        }*/

        // private void CreateMainLunarModule()
        // {
        //     module = PhotonNetwork.Instantiate(roverExplorerPrefab.name, Vector3.zero, Quaternion.identity);
        //     pv.RPC("Rpc_SetModuleParent", RpcTarget.AllBuffered);
        // }
        //
        // [PunRPC]
        // private void Rpc_SetModuleParent()
        // {
        //     Debug.Log("Rpc_SetModuleParent- RPC Called");
        //     module.transform.parent = TableAnchor.Instance.transform;
        //     module.transform.localPosition = moduleLocation;
        // }
    }
}