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
        // PlayerPrefs'te "Winner" anahtarý altýnda kaydedilmiþ bir deðerimiz varsa ve oyuncu master clientsa
        if (PlayerPrefs.HasKey("Winner") && PhotonNetwork.IsMasterClient)
        {
            // Kazananýn takma adýný saklayacak geçici bir deðiþken oluþturma
            string winner = PlayerPrefs.GetString("Winner");
            // Son maçý kazanan oyuncunun adýný görüntülemek için sohbet mesajý fonksiyonunu çaðýrma
            photonView.RPC("ShowMessage", RpcTarget.All, "The last match was won by: " + winner);
            // Ayný mesajýn tekrarlanmamasý için PlayerPrefs'ten her þeyi silme
            PlayerPrefs.DeleteAll();
        }
    }
    public void Send()
    {
        //Alanýn içinde herhangi bir metin yoksa, hiçbir þey yapmayýz
        if (string.IsNullOrWhiteSpace(inputText.text)) { return; }
        // Bir oyuncu Enter düðmesine basarsa
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // Sunucudaki tüm oyuncular için ShowMessage yöntemini çaðýrýyoruz
            // Oyuncunun kullanýcý adýný ve InputField'ýna yazdýðý tüm metni çýktý olarak vermemiz gerekiyor
            photonView.RPC("ShowMessage", RpcTarget.All, PhotonNetwork.NickName + ": " + inputText.text);
            // InputField'daki metni temizleme
            inputText.text = string.Empty;
        }
    }
    void RefreshPlayers()
    {
        // Çaðrý yalnýzca Ana Ýstemci (sunucuyu oluþturan oyuncu) tarafýndan yapýlabilir
        if (PhotonNetwork.IsMasterClient)
        {
            // Lobideki tüm oyuncular için ShowPlayers yöntemini çaðýrma
            photonView.RPC("ShowPlayers", RpcTarget.All);
        }
    }

    [PunRPC]
    public void ShowPlayers()
    {
        // Oyuncu listesini temizleme, sadece 'Players:' satýrýný býrakma
        PlayersText.text = "Players: ";
        // Sunucudaki tüm oyuncularýn için çalýþacak bir döngü baþlatma
        foreach (Photon.Realtime.Player otherPlayer in PhotonNetwork.PlayerList)
        {
            // Sonraki satýra geçiþ
            PlayersText.text += "\n";
            // Kullanýcý adýný çýktý vermek
            PlayersText.text += otherPlayer.NickName;
        }
    }

    [PunRPC]
    void ShowMessage(string message)
    {
        //yazýyý sonraki satýra geçirme
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
