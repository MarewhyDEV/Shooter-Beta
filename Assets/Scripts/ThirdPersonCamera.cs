using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField]
    [Range(0.5f, 2f)]
    float mouseSense = 1;
    [SerializeField]
    [Range(-20, -10)]
    int lookUp = -15;
    [SerializeField]
    [Range(15, 25)]
    int lookDown = 20;

    // Oyuncunun mevcut durumunu takip edecek bir boolean deðiþkeni
    public bool isSpectator = false;
    // Serbest kamera hareket kodu
    [SerializeField] float speed = 50f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        float rotateX = Input.GetAxis("Mouse X") * mouseSense;
        float rotateY = Input.GetAxis("Mouse Y") * mouseSense;
        if(!isSpectator)
        { 
            Vector3 rotCamera = transform.rotation.eulerAngles;
            Vector3 rotPlayer = player.transform.rotation.eulerAngles;

            rotCamera.x = (rotCamera.x > 180) ? rotCamera.x - 360 : rotCamera.x;
            rotCamera.x = Mathf.Clamp(rotCamera.x, lookUp, lookDown);
            rotCamera.x -= rotateY;

            rotCamera.z = 0;
            rotPlayer.y += rotateX;

            transform.rotation = Quaternion.Euler(rotCamera);
            player.transform.rotation = Quaternion.Euler(rotPlayer);
        }
        else
        {
            // Mevcut kamera açýsýna bakalým
            Vector3 rotCamera = transform.rotation.eulerAngles;
            // Farenin hareketine baðlý olarak kameranýn dönüþünü deðiþtirme
            rotCamera.x -= rotateY;
            rotCamera.z = 0;
            rotCamera.y += rotateX;
            transform.rotation = Quaternion.Euler(rotCamera);
            // WASD tuþlarýna basýþlarý okuma 
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            // Kamera hareket vektörünü ayarlama
            Vector3 dir = transform.right * x + transform.forward * z;
            // Kameranýn konumunu deðiþtirme
            transform.position += dir * speed * Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Eðer imleç kitliyse...
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                // Kilidi açalým
                Cursor.lockState = CursorLockMode.None;
            }
            // Diðer durumda...
            else
            {
                // Ýmleci kitleyelim
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}