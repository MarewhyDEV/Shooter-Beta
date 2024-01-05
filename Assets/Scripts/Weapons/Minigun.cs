using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Minigun : Weapon
{
    private void Start()
    {
        coolDown = 0.1f;

        isAuto = true;
        ammoCurrent = 100;
        ammoMax = 100;
        ammoBackpack = 200;
    }

    protected override void OnShoot()
    {
        Vector3 rayStartPosition = new Vector3(Screen.width / 2, Screen.height / 2, 0); // Işın başlangıç noktası ekranın ortası
        Vector3 tepme = new Vector3(Random.Range(-35, 35), Random.Range(-35, 35), Random.Range(-35, 35));
        Ray ray = cam.GetComponent<Camera>().ScreenPointToRay(rayStartPosition+tepme); // Işın oluşturma
        RaycastHit hit; // �arpt���m�z obje

        if (Physics.Raycast(ray, out hit))
        {
            GameObject bullet = Instantiate(particle, hit.point, hit.transform.rotation);
            if (hit.collider.CompareTag("enemy"))
            {
                // 10 sayısını istediğiz şekilde değiştirebilirsiniz bir merminin vereceği zararı belirtiyor
                hit.collider.gameObject.GetComponent<Enemy>().GetDamage(5);
            }
            // çizgiler içerisindeki kod kurs dışı eklenmiştir 3.12.2023
            else if (hit.collider.CompareTag("Player")) // eğer oyuncuya sıktıysak
            {
                //hit.collider.gameObject.GetComponent<PlayerController>().ChangeHealth(5); // oyuncunun canını 5 azalt
                var enemyHealth = hit.collider.GetComponent<PlayerController>();
                enemyHealth.photonView.RPC("ChangeHealth", RpcTarget.All, 10);
            }
            //
            Destroy(bullet, 1f);
        }
    }
}
