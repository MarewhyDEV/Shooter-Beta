using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkEnemy : Enemy
{
    [SerializeField] float speed;

    [SerializeField] float detectionDistance;
    float rotateTimer;
    public override void Move()
    {
        //e�er d��man ve oyuncu aras�ndaki uzakl�k bizim b�ce�in fark edebilece�i menzildeyse
        //ve d��man ve oyuncu aras�ndaki uzakl�k d��man�n sald�r� menzilinden uzak ise

        if(distance < detectionDistance && distance > attackDistance)
        {
            //oyuncuya bak
            transform.LookAt(player.transform);
            //ko�ma animasyonunu �al��t�r.
            anim.SetBool("Run",true);
            rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
        }
        else if(distance > detectionDistance)
        {
            rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
            rotateTimer += Time.deltaTime;
            anim.SetBool("Run", true);
            if(rotateTimer > 10)
            {
                transform.Rotate(new Vector3(0,Random.Range(0,150), 0));
                rotateTimer = 0;
            }
        }
        else
        {
            anim.SetBool("Run", false);
        }
    }

    public override void Attack()
    {
        timer += Time.deltaTime;
        //e�er oyuncu ile d��man aras�ndaki mesafe sald�r� menzili i�erisindeyse ve timer cooldowndan b�y�kse sald�r
        if(distance < attackDistance && timer > cooldown)
        {
            timer = 0; //timer'� s�f�rla

            //oyuncunun can�n� azalt
            player.GetComponent<PlayerController>().ChangeHealth(damage);

            //sald�r� animasyonunu �al��t�r
            anim.SetBool("Attack", true);
        }

        else
        {
            anim.SetBool("Attack", false);
        }
    }
}
