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

    //oyuncu say�s�n� tutacak text objesine referans
    [SerializeField] public TMP_Text playersText;
    //oyuncular� tutacak array / depo
    GameObject[] players;
    // Aktif oyuncular�n kullan�c� adlar�n� depolayacak bir liste
    List<string> activePlayers = new List<string>();

    // De�i�keni tan�tal�m
    private int previousPlayerCount;

    [SerializeField] GameObject exitButton;


    int checkPlayers = 0;

    int randSpawn;

    void Start()
    {
        // Rastgele bir say� se�mek
        // Bu sat�rda Photon() fonksiyonunu kullanarak sunucudaki oyuncu say�s�n� ediniyoruz
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
            // E�er oyuncu hayattaysa
            if (player.GetComponent<PlayerController>().dead == false)
            {
                // activePlayers listesine kullan�c� ad�n�n eklenmesi
                activePlayers.Add(player.GetComponent<PhotonView>().Owner.NickName);
            }
        }
        playersText.text = "Hayatta Kalan Oyuncu Say�s� : " + activePlayers.Count.ToString();
        // E�er sadece 1 (veya daha az) oyuncu kald�ysa...
        if (activePlayers.Count <= 1 && checkPlayers > 0)
        {
            PlayerPrefs.SetString("Winner", activePlayers[0]);
            // Oyundaki b�t�n d��manlar� aray�p bir listede tutmak
            var enemies = GameObject.FindGameObjectsWithTag("enemy");
            // Listedeki d��manlara s�ras�yla bakmak
            foreach (GameObject enemy in enemies)
            {
                // Listedeki her d��mana 100 zarar vermek. E�er 100'den fazla can� olan d��manlar varsa bu say�y� ona g�re d�zenleyin!
                enemy.GetComponent<Enemy>().ChangeHealth(500);
            }
            Invoke("EndGame", 3f);
        }
        checkPlayers++;
    }


    public void EndGame()
    {
        // Lobiye geri d�n��
        Debug.Log("MEN�YE D�N�LD�");
        PhotonNetwork.LoadLevel("Lobby");
    }


    // Buton i�in bir fonksiyon
    public void ExitGame()
    {
        PhotonNetwork.LeaveRoom();
    }
    // Oyuncu ��kt���nda �al��acak bir Photon fonksiyonu
    public override void OnLeftRoom()
    {
        // Menu sahnesinin y�klenmesi
        SceneManager.LoadScene(0);
        // Oyuncu listesinin g�ncellenmesi
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
