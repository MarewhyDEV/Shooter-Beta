using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] List<Transform> playerSpawns = new List<Transform>();
    [SerializeField] List<Transform> walkSpawns = new List<Transform>();
    [SerializeField] List<Transform> turretSpawns = new List<Transform>();

    //oyuncu sayýsýný tutacak text objesine referans
    [SerializeField] public TMP_Text playersText;
    //oyuncularý tutacak array / depo
    GameObject[] players;
    // Aktif oyuncularýn kullanýcý adlarýný depolayacak bir liste
    List<string> activePlayers = new List<string>();

    // Deðiþkeni tanýtalým
    private int previousPlayerCount;

    [SerializeField] GameObject exitButton;


    int checkPlayers = 0;

    int randSpawn;

    void Start()
    {
        // Rastgele bir sayý seçmek
        // Bu satýrda Photon() fonksiyonunu kullanarak sunucudaki oyuncu sayýsýný ediniyoruz
        previousPlayerCount = PhotonNetwork.PlayerList.Length;
        randSpawn = Random.Range(0, playerSpawns.Count);
        PhotonNetwork.Instantiate("Player", playerSpawns[randSpawn].position, playerSpawns[randSpawn].rotation);
        Invoke("SpawnEnemy",5f);
    }

    private void Update()
    {
        if (PhotonNetwork.PlayerList.Length < previousPlayerCount)
        {
            ChangePlayersList();
        }
        previousPlayerCount = PhotonNetwork.PlayerList.Length;
    }

    public void ChangePlayersList()
    {
        photonView.RPC("PlayerList", RpcTarget.All);
    }

    [PunRPC]
    public void PlayerList()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        activePlayers.Clear();
        foreach (GameObject player in players)
        {
            // Eðer oyuncu hayattaysa
            if (player.GetComponent<PlayerController>().dead == false)
            {
                // activePlayers listesine kullanýcý adýnýn eklenmesi
                activePlayers.Add(player.GetComponent<PhotonView>().Owner.NickName);
            }
        }
        playersText.text = "Hayatta Kalan Oyuncu Sayýsý : " + activePlayers.Count.ToString();
        // Eðer sadece 1 (veya daha az) oyuncu kaldýysa...
        if (activePlayers.Count <= 1 && checkPlayers > 0)
        {
            PlayerPrefs.SetString("Winner", activePlayers[0]);
            // Oyundaki bütün düþmanlarý arayýp bir listede tutmak
            var enemies = GameObject.FindGameObjectsWithTag("enemy");
            // Listedeki düþmanlara sýrasýyla bakmak
            foreach (GameObject enemy in enemies)
            {
                // Listedeki her düþmana 100 zarar vermek. Eðer 100'den fazla caný olan düþmanlar varsa bu sayýyý ona göre düzenleyin!
                enemy.GetComponent<Enemy>().ChangeHealth(500);
            }
            Invoke("EndGame", 3f);
        }
        checkPlayers++;
    }


    public void EndGame()
    {
        // Lobiye geri dönüþ
        Debug.Log("MENÜYE DÖNÜLDÜ");
        PhotonNetwork.LoadLevel("Lobby");
    }


    // Buton için bir fonksiyon
    public void ExitGame()
    {
        PhotonNetwork.LeaveRoom();
    }
    // Oyuncu çýktýðýnda çalýþacak bir Photon fonksiyonu
    public override void OnLeftRoom()
    {
        // Menu sahnesinin yüklenmesi
        SceneManager.LoadScene(0);
        // Oyuncu listesinin güncellenmesi
        ChangePlayersList();
    }

    public void SpawnEnemy()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < walkSpawns.Count; i++)
            {
                PhotonNetwork.Instantiate("WalkEnemy", walkSpawns[1].position, walkSpawns[i].rotation);
            }
            for (int i = 0; i < turretSpawns.Count; i++)
            {
                PhotonNetwork.Instantiate("Turret", turretSpawns[1].position, turretSpawns[i].rotation);
            }
        }
    }
}
