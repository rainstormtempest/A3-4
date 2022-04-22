using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
public class ShootableBox : MonoBehaviour
{


 


    public FirstPersonController fpc;

    public void Awake()
    {

       
    }

    public void Damage(int damageAmount)
    {
        //dragon.dragon_injure();

        //// 检测总血量是否小于等于0 
        //if (dragon.curblood <= 0)
        //{
        //    fpc.enabled = false;
        //    Cursor.visible = true;
        //    Cursor.lockState = CursorLockMode.Locked;
       
        //}
    }

}
