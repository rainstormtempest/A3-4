using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using  UnityStandardAssets.Characters.FirstPerson;

public class herocontroller : MonoBehaviour
{

 

    public float injure_curtime;
    public float injure_maxtime;
    public bool injure_flage;

    public FirstPersonController fpc;


    public AudioClip run_ac;
    public AudioClip armo_ac;
    public AudioClip gun_shootac;



    public GameObject weapongo1;
    public GameObject weapongo2;
    public GameObject weapongo3;
    public GameObject weapongo4;


    public GameObject army1;
    public GameObject army2;
    public GameObject army3;
    public GameObject army4;


    public data data;



    public AudioSource audio;

    //当前子弹
    public int cur_armo;
    public int max_armo;

    public float curblood;
    public float maxblood;

    public bool wumiao_meishoudaoshanghai;
    public float wumiao_meishoudaoshanghai_time;

    public bool recove_blood;
    public float recover_time;

    public bool isopen_fire;



    public int doorkey;
    public int ground_key;
    public int shadkle;

    

    public void shoot_sound()
    {
        if (!audio.isPlaying)
        {
            audio.PlayOneShot(gun_shootac);
        }


    }
    public void reload_sound()
    {
        if (!audio.isPlaying)
        {
            audio.PlayOneShot(armo_ac);
        }


    }

    public void run_effect()
    {
        if (!audio.isPlaying)
        {
            audio.PlayOneShot(run_ac);
        }


    }



    private void Awake()
    {

        curblood = 100;
        maxblood = 100;

        doorkey = 0;
        ground_key = 0;
        shadkle = 0;

       data =GameObject.Find("data(Clone)").GetComponent<data>();


        switch (data.weapon_index)
        {
            case 1:
                weapongo1.gameObject.SetActive(true);
                weapongo2.SetActive(false);
                weapongo3.SetActive(false);
                weapongo4.SetActive(false);

                break;
            case 2:
                weapongo2.gameObject.SetActive(true);
                weapongo1.SetActive(false);
                weapongo3.SetActive(false);
                weapongo4.SetActive(false);
                break;
            case 3:
                weapongo3.gameObject.SetActive(true);
                weapongo1.SetActive(false);
                weapongo2.SetActive(false);
                weapongo4.SetActive(false);
                break;
            case 4:
                weapongo4.gameObject.SetActive(true);
                weapongo1.SetActive(false);
                weapongo2.SetActive(false);
                weapongo3.SetActive(false);
                break;

        }


        switch (data.player_index)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;

        }


        injure_flage = false;
        injure_curtime = 0;
        injure_maxtime = 5;

    }



    public void damage(float blood)
    {
        //重新计算时间 恢复血量
        wumiao_meishoudaoshanghai = false;
        wumiao_meishoudaoshanghai_time = 5;

        curblood -= blood;

    }

  

    public void change_armo()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (max_armo >= 30)
            {
                max_armo -= 30;
                cur_armo = 30;
            }
            else
            {
                max_armo = 0;
                cur_armo = max_armo;
            }
        }

      


    }


    public void help_army()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {

        }


    }


    public void open_door()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Collider[] ary1 = Physics.OverlapSphere(transform.position, 1.0f, 1 << LayerMask.NameToLayer("door1"));

            for (int i = 0; i < ary1.Length; i++)
            {
                Debug.LogError("ary1=" + ary1[i].gameObject.name);

                if (ary1[i] != null)
                {
                    ary1[i].GetComponent<Animator>().Play("open_door1");


                }


            }
        }


    }
    public void pick_item()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {

            Collider[] ary = Physics.OverlapSphere(transform.position, 1.0f, 1 << LayerMask.NameToLayer("bullet"));

            for (int i = 0; i < ary.Length; i++)
            {
                if (ary[i] != null)
                {

                    Destroy(ary[i].gameObject);
                    cur_armo += 20;
                }


            }





            Collider[] ary1 = Physics.OverlapSphere(transform.position, 1.0f, 1 << LayerMask.NameToLayer("key"));

            for (int i = 0; i < ary1.Length; i++)
            {
                Debug.LogError("ary1=" + ary1[i].gameObject.name);

                if (ary1[i] != null)
                {
                    key_type mytype = ary1[i].GetComponent<key>().keytype;
                    switch (mytype)
                    {
                        case key_type.groundkey:
                            ground_key += 1;
                            Destroy(gameObject);
                            break;
                        case key_type.doorkey:
                            doorkey += 1;
                            Destroy(gameObject);
                            break;

                    }

                }


            }



        }


    }

    public void open_door2()
    {

        if (doorkey >= 1 && ground_key >= 1)
        {
            data.level2_open = true;
        }



    }

    public void open_door3()
    {
       
            Collider[] ary1 = Physics.OverlapSphere(transform.position, 1.0f, 1 << LayerMask.NameToLayer("door2"));

            for (int i = 0; i < ary1.Length; i++)
            {
                Debug.LogError("ary1=" + ary1[i].gameObject.name);

                if (ary1[i] != null)
                {
                if (shadkle >= 2)
                {
                    data.level3_open = true;
                }

                }


            }
       


    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


        //获取子弹
        pick_item();

        open_door();
        open_door2();
        open_door3();


        




        if (injure_flage)
        {

            
        }
        else
        {

            if (injure_curtime >= injure_maxtime)
            {
                injure_flage = true;
                injure_curtime = 0;

            }
            else
            {
                injure_curtime += Time.deltaTime;

            }  



        }


        if (wumiao_meishoudaoshanghai_time <= 0)
        {
            wumiao_meishoudaoshanghai = true;
        }
        else
        {
            wumiao_meishoudaoshanghai_time -= Time.deltaTime;
        }


        //恢复血
        if (wumiao_meishoudaoshanghai)
        {

            if (recover_time <= 0)
            {
                recover_time = 10;
                curblood += 10;
            }
            else
            {
                recover_time -= Time.deltaTime;
            }


        }


    }
}
