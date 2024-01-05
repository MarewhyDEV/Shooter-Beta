using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
public class Weapon : MonoBehaviourPunCallbacks
{
    [SerializeField] AudioSource shoot;
    [SerializeField] AudioClip bulletSound, noBulletSound, reloadSound;


    //þarjördeki mermi sayýsý
    protected int ammoCurrent;
    //þarjörün max kapasitesi
    protected int ammoMax;
    //yedekte tutabileceðim mermi miktarý
    protected int ammoBackpack;
    //güncellenecek UI
    [SerializeField] TMP_Text ammoText;
    


    [SerializeField] protected GameObject particle; // Mermi izi
    //kamerayý tutmamýz lazým
    [SerializeField] protected GameObject cam;
    //silahýn modu
    protected bool isAuto = false;
    //atýþlar arasýndaki aralýk ve süreyi zamanlama
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
            ammoBackpack -= eklenecekMermi; // eklediðimiz mermiyi backpackten çýkar

        }
        else
        {
            ammoCurrent += ammoBackpack; // backpackteki mermi gereken mermiden az olduðu için tüm backpackteki mermiyi ekle
            ammoBackpack = 0; // backpacki sýfýra eþitle
        }
    }


    //child(çocuklar) tarafýndan override edilecek
    [PunRPC]
    protected virtual void OnShoot()
    {

    }
}
