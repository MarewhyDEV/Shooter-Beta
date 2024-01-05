using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text chatText;
    [SerializeField] TMP_InputField inputText;
    [SerializeField] TMP_Text PlayersText;
    [SerializeField] GameObject startButton;

    private void Start()
    {
        RefreshPlayers();
        if(!PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(false);
        }
        // PlayerPrefs'te "Winner" anahtar� alt�nda kaydedilmi� bir de�erimiz varsa ve oyuncu master clientsa
        if (PlayerPrefs.HasKey("Winner") && PhotonNetwork.IsMasterClient)
        {
            // Kazanan�n takma ad�n� saklayacak ge�ici bir de�i�ken olu�turma
            string winner = PlayerPrefs.GetString("Winner");
            // Son ma�� kazanan oyuncunun ad�n� g�r�nt�lemek i�in sohbet mesaj� fonksiyonunu �a��rma
            photonView.RPC("ShowMessage", RpcTarget.All, "The last match was won by: " + winner);
            // Ayn� mesaj�n tekrarlanmamas� i�in PlayerPrefs'ten her �eyi silme
            PlayerPrefs.DeleteAll();
        }
    }
    public void Send()
    {
        //Alan�n i�inde herhangi bir metin yoksa, hi�bir �ey yapmay�z
        if (string.IsNullOrWhiteSpace(inputText.text)) { return; }
        // Bir oyuncu Enter d��mesine basarsa
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // Sunucudaki t�m oyuncular i�in ShowMessage y�ntemini �a��r�yoruz
            // Oyuncunun kullan�c� ad�n� ve InputField'�na yazd��� t�m metni ��kt� olarak vermemiz gerekiyor
            photonView.RPC("ShowMessage", RpcTarget.All, PhotonNetwork.NickName + ": " + inputText.text);
            // InputField'daki metni temizleme
            inputText.text = string.Empty;
        }
    }
    void RefreshPlayers()
    {
        // �a�r� yaln�zca Ana �stemci (sunucuyu olu�turan oyuncu) taraf�ndan yap�labilir
        if (PhotonNetwork.IsMasterClient)
        {
            // Lobideki t�m oyuncular i�in ShowPlayers y�ntemini �a��rma
            photonView.RPC("ShowPlayers", RpcTarget.All);
        }
    }

    [PunRPC]
    public void ShowPlayers()
    {
        // Oyuncu listesini temizleme, sadece 'Players:' sat�r�n� b�rakma
        PlayersText.text = "Players: ";
        // Sunucudaki t�m oyuncular�n i�in �al��acak bir d�ng� ba�latma
        foreach (Photon.Realtime.Player otherPlayer in PhotonNetwork.PlayerList)
        {
            // Sonraki sat�ra ge�i�
            PlayersText.text += "\n";
            // Kullan�c� ad�n� ��kt� vermek
            PlayersText.text += otherPlayer.NickName;
        }
    }

    [PunRPC]
    void ShowMessage(string message)
    {
        //yaz�y� sonraki sat�ra ge�irme
        chatText.text += "\n";
        chatText.text += message;
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel("Game");
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        ShowMessage(otherPlayer.NickName + "has left the room");
        RefreshPlayers();
        if (!PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        ShowMessage(newPlayer.NickName + "has joined the room");
        RefreshPlayers();
    }
}
