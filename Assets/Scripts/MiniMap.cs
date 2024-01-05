using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MiniMap : MonoBehaviourPunCallbacks
{
    // Kayd�rma h�z�
    [SerializeField] private float scrollSpeed = 1f;
    // Minimum zoom seviyesi
    [SerializeField] private float minValue = 10f;
    // Maximum zoom seviyesi
    [SerializeField] private float maxValue = 60f;
    // �u anki zoom seviyesi
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
        // Kayd�rmak durumuna g�re zoom de�i�kenini ayarlamak
        if (scrollDelta > 0)
        {
            currentValue += scrollSpeed;
        }
        else if (scrollDelta < 0)
        {
            currentValue -= scrollSpeed;
        }
        // Kayd�rma de�erinin kodda tan�t�lan minimum ve maksimum yak�nla�t�rma de�erleri aras�nda s�n�rland�r�lmas�
        // Mevcut yak�nla�t�rma de�eri s�n�rlay�c� de�erler aras�nda "s�k��t�r�l�r" (Clamp), yani sonsuza kadar yak�nla�t�r�p uzakla�t�ramay�z
        currentValue = Mathf.Clamp(currentValue, minValue, maxValue);
        gameObject.GetComponent<Camera>().orthographicSize = currentValue;
    }
}
