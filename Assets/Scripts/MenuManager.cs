using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class MenuManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text logText;
    [SerializeField] TMP_InputField inputField;

    void Log(string message)
    {
        //yaz�y� sonraki sat�ra ge�irme
        logText.text += "\n";
        logText.text += message;
    }

    private void Start()
    {
        PhotonNetwork.NickName = "Player" + Random.Range(0,9999); // rastgele nickname
        //bu oyuncunun ad�n� log k�sm�na yazd�ral�m
        Log("Player Name:" + PhotonNetwork.NickName);
        //oyun ayarlar�
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "0.01";
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 15 });
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    public void ChangeName()
    {
        //InputField alan�na yaz�lan yaz�y� okumak
        PhotonNetwork.NickName = inputField.text;
        //Yeni kullan�c� ad�n� ��kt� vermek:
        Log("New Player name: " + PhotonNetwork.NickName);
    }
    public override void OnConnectedToMaster()
    {
        Log("Connected to server.");
    }

    public override void OnJoinedRoom()
    {
        Log("Joined Lobby");
        PhotonNetwork.LoadLevel("Lobby");
    }

}
