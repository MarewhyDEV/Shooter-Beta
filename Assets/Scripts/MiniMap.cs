using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MiniMap : MonoBehaviourPunCallbacks
{
    // Kaydýrma hýzý
    [SerializeField] private float scrollSpeed = 1f;
    // Minimum zoom seviyesi
    [SerializeField] private float minValue = 10f;
    // Maximum zoom seviyesi
    [SerializeField] private float maxValue = 60f;
    // Þu anki zoom seviyesi
    private float currentValue;

    private void Start()
    {
        if(!photonView.IsMine)
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Fare tekerlek verilerine bakmak
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        // Kaydýrmak durumuna göre zoom deðiþkenini ayarlamak
        if (scrollDelta > 0)
        {
            currentValue += scrollSpeed;
        }
        else if (scrollDelta < 0)
        {
            currentValue -= scrollSpeed;
        }
        // Kaydýrma deðerinin kodda tanýtýlan minimum ve maksimum yakýnlaþtýrma deðerleri arasýnda sýnýrlandýrýlmasý
        // Mevcut yakýnlaþtýrma deðeri sýnýrlayýcý deðerler arasýnda "sýkýþtýrýlýr" (Clamp), yani sonsuza kadar yakýnlaþtýrýp uzaklaþtýramayýz
        currentValue = Mathf.Clamp(currentValue, minValue, maxValue);
        gameObject.GetComponent<Camera>().orthographicSize = currentValue;
    }
}
