using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
public class Weapon : MonoBehaviourPunCallbacks
{
    [SerializeField] AudioSource shoot;
    [SerializeField] AudioClip bulletSound, noBulletSound, reloadSound;


    //�arj�rdeki mermi say�s�
    protected int ammoCurrent;
    //�arj�r�n max kapasitesi
    protected int ammoMax;
    //yedekte tutabilece�im mermi miktar�
    protected int ammoBackpack;
    //g�ncellenecek UI
    [SerializeField] TMP_Text ammoText;
    


    [SerializeField] protected GameObject particle; // Mermi izi
    //kameray� tutmam�z laz�m
    [SerializeField] protected GameObject cam;
    //silah�n modu
    protected bool isAuto = false;
    //at��lar aras�ndaki aral�k ve s�reyi zamanlama
    protected float coolDown = 0f;
    protected float timer = 0f;

    private void Start()
    {
        timer = coolDown;
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            timer += Time.deltaTime;
            if (Input.GetMouseButton(0))
            {
                Shoot();
            }
            AmmoTextUpdate();
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (ammoCurrent != ammoMax || ammoBackpack != 0)
                {
                    Invoke("Reload", 1f);
                    shoot.PlayOneShot(reloadSound);
                }
            }
        }
    }

    
    
    public void Shoot()
    {
        if(Input.GetMouseButtonDown(0) || isAuto)
        {
            if(timer > coolDown)
            {
                if(ammoCurrent > 0)
                {
                    timer = 0f;
                    OnShoot();
                    ammoCurrent--;
                    shoot.PlayOneShot(bulletSound);
                    shoot.pitch = Random.Range(1f, 1.5f);
                }
                else
                {
                    shoot.PlayOneShot(noBulletSound);
                }
            }
        }
    }

    private void AmmoTextUpdate()
    {
        ammoText.text = ammoCurrent + "/" + ammoBackpack;
    }

    private void Reload()
    {
        int eklenecekMermi = ammoMax - ammoCurrent;
        if(ammoBackpack >= eklenecekMermi) // backpackteki mermi gereken mermiden fazlaysa
        {
            ammoCurrent += eklenecekMermi; // gerekli mermiyi ekle
            ammoBackpack -= eklenecekMermi; // ekledi�imiz mermiyi backpackten ��kar

        }
        else
        {
            ammoCurrent += ammoBackpack; // backpackteki mermi gereken mermiden az oldu�u i�in t�m backpackteki mermiyi ekle
            ammoBackpack = 0; // backpacki s�f�ra e�itle
        }
    }


    //child(�ocuklar) taraf�ndan override edilecek
    [PunRPC]
    protected virtual void OnShoot()
    {

    }
}
