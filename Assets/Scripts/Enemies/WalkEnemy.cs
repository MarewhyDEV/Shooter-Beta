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
        //eðer düþman ve oyuncu arasýndaki uzaklýk bizim böceðin fark edebileceði menzildeyse
        //ve düþman ve oyuncu arasýndaki uzaklýk düþmanýn saldýrý menzilinden uzak ise

        if(distance < detectionDistance && distance > attackDistance)
        {
            //oyuncuya bak
            transform.LookAt(player.transform);
            //koþma animasyonunu çalýþtýr.
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
        //eðer oyuncu ile düþman arasýndaki mesafe saldýrý menzili içerisindeyse ve timer cooldowndan büyükse saldýr
        if(distance < attackDistance && timer > cooldown)
        {
            timer = 0; //timer'ý sýfýrla

            //oyuncunun canýný azalt
            player.GetComponent<PlayerController>().ChangeHealth(damage);

            //saldýrý animasyonunu çalýþtýr
            anim.SetBool("Attack", true);
        }

        else
        {
            anim.SetBool("Attack", false);
        }
    }
}
