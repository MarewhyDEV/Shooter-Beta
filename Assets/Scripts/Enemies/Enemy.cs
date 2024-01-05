using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Canvas'a UI ile eriþiyoruz
using Photon.Pun;
public class Enemy : MonoBehaviourPunCallbacks
{
    [SerializeField] protected int health;
    [SerializeField] protected float attackDistance;
    [SerializeField] protected int damage;
    [SerializeField] protected float cooldown;
    [SerializeField] Image healthBar;
    protected GameObject player;
    protected GameObject[] players;
    protected Animator anim;
    protected Rigidbody rb;

    protected float distance;
    protected float timer;
    bool dead = false;
    float storage;


    public virtual void Move()
    {

    }

    public virtual void Attack()
    {

    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        //players = FindObjectOfType<PlayerController>().gameObject;
        CheckPlayers();
        storage = health;
    }

    void CheckPlayers()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        Invoke("CheckPlayers", 3f);
    }

    private void Update()
    {
        /* ESKÝ KOD 24.11.2023
        //distance = Vector3.Distance(this.transform.position, player.transform.position);

        if(!dead)
        {
            Attack();
        }
        */

        //YENÝ HALÝ 24.11.2023 GÜNÜ YAPILDI

        float closestDistance = Mathf.Infinity; // sonsuz sayý
        foreach (GameObject closestPlayer in players)
        {
            float checkDistance = Vector3.Distance(closestPlayer.transform.position, this.transform.position);
            if (checkDistance < closestDistance)
            { 
                if (closestPlayer.GetComponent<PlayerController>().dead == false)
                {
                    player = closestPlayer;
                    closestDistance = checkDistance;
                }
            }
        }

        if(player != null)
        {
            distance = Vector3.Distance(this.transform.position, player.transform.position);
            if (!dead)
            {
                Attack();
            }
        }
    }

    private void FixedUpdate()
    {
        if(!dead && player != null)
        {
            Move();
        }
    }

    public void GetDamage(int count)
    {
        photonView.RPC("ChangeHealth", RpcTarget.All, count);
    }
    

    [PunRPC]
    public void ChangeHealth(int count)
    {
        health -= count;
        float fillPercent = health / storage; // düþman caný 100 ise bir tam çubuk (1) olacaktýr 50 olursa 0.5 dolu olacaktýr yani yarýsý.
        healthBar.fillAmount = fillPercent; // ondalýk ifadeyi düþman can barýna aktarýyoruz
        if (health <= 0)
        {
            dead = true;
            GetComponent<Collider>().enabled = false; // yaratýk öldüyse collideri kapat çarpmamak için
            //anim.enabled = true;
            anim.SetBool("Die", true);
        }
    }

}
