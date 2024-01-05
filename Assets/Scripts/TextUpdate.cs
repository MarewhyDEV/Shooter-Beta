using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class TextUpdate : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] TMP_Text playerNickName;
    int health = 100;

    private void Start()
    {
        if(photonView.IsMine)
        {
            playerNickName.text = photonView.Controller.NickName + "\n" + "Health: " + health.ToString();
            photonView.RPC("RotateName", RpcTarget.Others);
        }
    }

    // can deðerine bakmak (ve arayüzde göstermek)
    public void SetHealth(int newHealth)
    {
        // can deðerini güncellemek
        health = newHealth;
        // UI yazýsýný güncellemek
        // Photon'dan kullanýcý adýný almak, alt satýra geçmek, ve can deðerini görüntülemek
        playerNickName.text = photonView.Controller.NickName + "\n" + "Health: " + health.ToString();
    }

    [PunRPC]
    public void RotateName()
    {
        playerNickName.GetComponent<RectTransform>().localScale = new Vector3(-1, 1, 1);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
        }
        else
        {
            health = (int)stream.ReceiveNext();
            playerNickName.text = photonView.Controller.NickName + "\n" + "Health: " + health.ToString();
        }
    }
}
