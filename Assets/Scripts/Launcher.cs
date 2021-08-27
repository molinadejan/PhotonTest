using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Com.Molinadejan.TestGame
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private byte maxPlayersPerRoom = 20;

        string gameVersion = "1";

        bool isConnecting;

        [SerializeField]
        private GameObject controlPanel;

        [SerializeField]
        private GameObject progressLabel;

        private void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        private void Start()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }

        public void Connect()
        {
            isConnecting = true;

            progressLabel.SetActive(true);
            controlPanel.SetActive(false);

            // �ϴ� �׽�Ʈ 1
            // �ʴ� ��Ŷ ���� ��
            PhotonNetwork.SendRate = 60;
            PhotonNetwork.SerializationRate = 30;

            if(PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("OnConnectedToMaster() was called by Pun");

            if (isConnecting)
            {
                PhotonNetwork.JoinRandomRoom();
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);

            Debug.LogWarningFormat("OnDisconnected() was called by Pun");
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("OnJoinRandomFailed() was called by Pun");
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("OnJoinedRoom() was called by Pun");
            //PhotonNetwork.LoadLevel("Room for 1");
            PhotonNetwork.LoadLevel("Room for 4");
        }
    }
}
