using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Pistol : Weapon, IPunObservable
{
    private void Start()
    {
        coolDown = 0.2f;
        isAuto = false;
        ammoCurrent = 6; // ba�lang��ta 6  mermi
        ammoMax = 6; // max mermi
        ammoBackpack = 36; // yedek olarak max 36 mermi ta��yabilir.
    }


    protected override void OnShoot()
    {
        Vector3 rayStartPosition = new Vector3(Screen.width / 2, Screen.height / 2, 0); // I��n ba�lang�� noktas� ekran�n ortas�
        Ray ray = cam.GetComponent<Camera>().ScreenPointToRay(rayStartPosition); // I��n olu�turma
        RaycastHit hit; // �arpt���m�z obje

        if(Physics.Raycast(ray, out hit))
        {
            GameObject bullet = Instantiate(particle, hit.point, hit.transform.rotation);
            if (hit.collider.CompareTag("enemy"))
            {
                // 10 say�s�n� istedi�iz �ekilde de�i�tirebilirsiniz bir merminin verece�i zarar� belirtiyor
                hit.collider.gameObject.GetComponent<Enemy>().GetDamage(10);
            }
            // �izgiler i�indeki kod kurs d��� eklendi 3.12.2023
            else if(hit.collider.CompareTag("Player")) // e�er oyuncuya s�kt�ysak
            {
                var enemyHealth = hit.collider.GetComponent<PlayerController>();
                enemyHealth.photonView.RPC("ChangeHealth",RpcTarget.All, 10);
                //hit.collider.gameObject.GetComponent<PlayerController>().ChangeHealth(10); // oyuncunun can�n� 10 azalt
            }
            //
            Destroy(bullet, 1f);
        }
    }

    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //stream.SendNext();
        }
        else
        {

        }
    }
    
}
