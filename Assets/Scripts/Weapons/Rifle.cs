using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : Pistol
{
    void Start()
    {
        coolDown = 0.1f;

        isAuto = true;

        ammoCurrent = 30;
        ammoMax = 30;
        ammoBackpack = 60;
    }
}
