using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;   

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    public enum Weapons
    {
        None,
        Pistol,
        Rifle,
        MiniGun
    }

    Weapons weapons = Weapons.None;

    TextUpdate textUpdate;

    [SerializeField] Image pistolUI, rifleUI, miniGunUI,cursor;
    [SerializeField] AudioSource characterSounds;
    [SerializeField] AudioClip jump;
    [SerializeField] float movementSpeed = 5.0f;
    [SerializeField] float jumpForce = 7.0f;
    [SerializeField] float shiftSpeed = 10.0f;
    [SerializeField] GameObject rifle, pistol, miniGun;
    [SerializeField] GameObject console;
    [SerializeField] GameObject damageUi;
    bool consoleEnabled = false;
    bool hesoyam;
    [SerializeField] TMP_Text consoleText;
    [SerializeField] TMP_Text inputConsoleText;
    [SerializeField] GameObject hesoyamText;
    float currentSpeed;
    Rigidbody rb;
    Vector3 direction;
    bool isGrounded = false;
    Animator anim;
    public float stamina = 5f;
    bool isRifle, isPistol, isMiniGun;
    //[SerializeField] TMP_Text staminatext;
    int health;
    public bool dead=false;
    GameManager gameManager;
    private void Start()
    {
        //nickname.text = PhotonNetwork.NickName;

        gameManager = FindObjectOfType<GameManager>();
        gameManager.ChangePlayersList();
        currentSpeed = movementSpeed;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        health = 100;
        textUpdate = GetComponent<TextUpdate>();
        ChangeHealth(15);
        if(!photonView.IsMine)
        {
            // Kamerayý oyuncunun Hierarchysinde bulma ve devre dýþý býrakma
            transform.Find("Main Camera").gameObject.SetActive(false);
            transform.Find("Canvas").gameObject.SetActive(false);
            // PlayerController kodunu devre dýþý býrakma
            this.enabled = false;
        }
    }
    private void Update()
    {
        //staminatext.text = "Stamina: " + stamina.ToString("f1");
        float moveHorizontal = Input.GetAxis("Horizontal");
        //Debug.Log("Move Horizontal: " + moveHorizontal.ToString());
        float moveVertical = Input.GetAxis("Vertical");
        //Debug.Log("Move Vertical: " + moveVertical.ToString());
        direction = new Vector3(moveHorizontal,0,moveVertical);
        direction = transform.TransformDirection(direction);
        if (stamina > 5)
        {
            stamina = 5;
        }
        else if(stamina < 0)
        {
            stamina = 0;
        }
        if(direction.x != 0 || direction.z != 0)
        {
            anim.SetBool("Run", true);
            if(!characterSounds.isPlaying && isGrounded)
            {
                characterSounds.Play();
            }
        }
        else
        {
            anim.SetBool("Run", false);
            characterSounds.Stop();
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
            characterSounds.Stop();
            AudioSource.PlayClipAtPoint(jump, transform.position);
            anim.SetBool("Jump", true);
            isGrounded = false;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if(stamina > 0f)
            {
                stamina -= Time.deltaTime;
                currentSpeed = shiftSpeed;
            }
            else
            {
                currentSpeed = movementSpeed;
            }
        }
        else if(!Input.GetKey(KeyCode.LeftShift))
        {
            stamina += Time.deltaTime;
            currentSpeed = movementSpeed;
        }


        if(Input.GetKeyDown(KeyCode.K))
        {
            if(!consoleEnabled)
            {
                console.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                consoleEnabled = true;
                Debug.Log("Log: Console enabled");
            }
            else
            {
                console.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                consoleEnabled = false;
                Debug.Log("Log: Console disabled");
            }
        }

        if(Input.GetKeyDown(KeyCode.Alpha1) && isPistol)
        {
            //ChooseWeapon(Weapons.Pistol);
            photonView.RPC("ChooseWeapon", RpcTarget.All, Weapons.Pistol);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && isRifle)
        {
            //ChooseWeapon(Weapons.Rifle);
            photonView.RPC("ChooseWeapon", RpcTarget.All, Weapons.Rifle);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && isMiniGun)
        {
            //ChooseWeapon(Weapons.MiniGun);
            photonView.RPC("ChooseWeapon", RpcTarget.All, Weapons.MiniGun);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            //ChooseWeapon(Weapons.None);
            photonView.RPC("ChooseWeapon", RpcTarget.All, Weapons.None);
        }

        if (weapons != Weapons.None)
        {
            cursor.enabled = true;
        }
        else
        {
            cursor.enabled = false;
        }
    }
    public void Hesoyam()
    {
        hesoyam = true;
        hesoyamText.SetActive(true);
    }

    public void Send()
    {
        //Alanýn içinde herhangi bir metin yoksa, hiçbir þey yapmayýz
        if (string.IsNullOrWhiteSpace(inputConsoleText.text)) { return; }
        // Bir oyuncu Enter düðmesine basarsa
        if (Input.GetKeyDown(KeyCode.Return))
        {

            // Sunucudaki tüm oyuncular için ShowMessage yöntemini çaðýrýyoruz
            // Oyuncunun kullanýcý adýný ve InputField'ýna yazdýðý tüm metni çýktý olarak vermemiz gerekiyor
            ShowMessage(inputConsoleText.text);
            // InputField'daki metni temizleme
            inputConsoleText.text = string.Empty;
        }
    }
    public void ShowMessage(string code)
    {
        if(code == "hesoyam")
        {
            Hesoyam();
        }
        else
        {
            Debug.Log("hesoyam yazýlmamýþ");
        }
        consoleText.text += "\n";
        consoleText.text += code;
        Debug.Log(code);

    }
    [PunRPC]
    public void ChangeHealth(int count)
    {
        if(!hesoyam)
        {
            health -= count;
            textUpdate.SetHealth(health);
            damageUi.SetActive(true);
            Invoke("RemoveDamageUi", 0.1f);
            if (health <= 0)
            {
                //öldük
                anim.SetBool("Die", true);
                ChooseWeapon(Weapons.None);
                gameManager.ChangePlayersList();
                transform.Find("Main Camera").GetComponent<ThirdPersonCamera>().isSpectator = true;
                this.enabled = false;
                dead = true;
            }
        }
    }

    void RemoveDamageUi()
    {
        damageUi.SetActive(false);
    }

    //mesela girdi olarak Weapons.Pistol ise pistol animi çalýþsýn
    [PunRPC]
    public void ChooseWeapon(Weapons weapon)
    {
        anim.SetBool("Pistol", weapon == Weapons.Pistol);
        anim.SetBool("Assault", weapon == Weapons.Rifle);
        anim.SetBool("MiniGun", weapon == Weapons.MiniGun);
        anim.SetBool("NoWeapon", weapon == Weapons.None);
        pistol.SetActive(weapon == Weapons.Pistol);
        rifle.SetActive(weapon == Weapons.Rifle);
        miniGun.SetActive(weapon == Weapons.MiniGun);
        weapons = weapon;
    }


    /*
    public void ChooseWeapon(string weapons)
    {
        switch (weapons)
        {
            case "Pistol":
                anim.SetBool("Pistol", true);
                anim.SetBool("Rifle", false);
                anim.SetBool("MiniGun", false);
                anim.SetBool("NoWeapon", false);
                rifle.SetActive(false);
                pistol.SetActive(true);
                miniGun.SetActive(false);
                break;
            case "Rifle":
                anim.SetBool("Pistol", false);
                anim.SetBool("Rifle", true);
                anim.SetBool("MiniGun", false);
                anim.SetBool("NoWeapon", false);
                rifle.SetActive(true);
                pistol.SetActive(false);
                miniGun.SetActive(false);
                break;
            case "MiniGun":
                anim.SetBool("Pistol", false);
                anim.SetBool("Rifle", false);
                anim.SetBool("MiniGun", true);
                anim.SetBool("NoWeapon", false);
                rifle.SetActive(false);
                pistol.SetActive(false);
                miniGun.SetActive(true);
                break;
            case "NoWeapon":
                anim.SetBool("Pistol", false);
                anim.SetBool("Rifle", false);
                anim.SetBool("MiniGun", false);
                anim.SetBool("NoWeapon", true);
                rifle.SetActive(false);
                pistol.SetActive(false);
                miniGun.SetActive(false);
                break;
        }
    }
    */

    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + direction * currentSpeed * Time.deltaTime);
        //alttakine bakacazzz
        //rb.velocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    }

    private void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
        anim.SetBool("Jump", false);
    }
    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch(other.gameObject.tag)
        {
            case "Pistol":
                if(!isPistol)
                {
                    isPistol = true;
                    ChooseWeapon(Weapons.Pistol);
                    pistolUI.color = Color.white;
                }
                break;
            case "Rifle":
                if (!isRifle)
                {
                    isRifle = true;
                    ChooseWeapon(Weapons.Rifle);
                    rifleUI.color = Color.white;
                }
                break;
            //minigun ödevi
            case "miniGun":
                if (!isMiniGun)
                {
                    isMiniGun = true;
                    ChooseWeapon(Weapons.MiniGun);
                    miniGunUI.color = Color.white;
                }
                break;
            default:
                break;
        }
        Destroy(other.gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
           stream.SendNext(health);
        } 
        else
        {
            health = (int)stream.ReceiveNext();
        }
    }
}
