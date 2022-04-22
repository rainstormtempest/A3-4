using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using BehaviorTree;
using MyBehaviorTree;

public class armyai : MonoBehaviour
{

    public TextMesh txt;
    public float need_life_time;
    public bool need_life_flage;
    public AudioSource audio;
    public AudioClip armo_ac;
    public AudioClip gun_shootac;

    public NavMeshAgent nma;

    public float curblood;
    public float maxblood;


    //攻击力
    public float atknumber;

    //目标
    public GameObject targetgo;


    public GameObject recover_blood_effectgo;


    public bool wumiao_meishoudaoshanghai;
    public float wumiao_meishoudaoshanghai_time;

    public bool recove_blood;
    public float recover_time;

    public Animator ani;

    public List<GameObject> armylist;


    public float chouhenzhi;

    //彻底挂掉
    public bool endlife;

    //目标点
    public Vector3 targetpos;

    public float armo;
    public float atktime;
    public bool atkflage;


    public GameObject herogo;

    private BehaviorTree.Driver driver;
    private Behavior root;

    private void Awake()
    {
        driver = new BehaviorTree.Driver();
        root = InitBehaviorTree();

        nma = GetComponent<NavMeshAgent>();

        audio = GetComponent<AudioSource>();

        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 1:
                curblood = 200;
                maxblood = 200;
                break;
                case 2:
                curblood = 300;
                maxblood = 300;
                break;
            case 3:
                curblood = 500;
                maxblood = 500;
                break;

                

        }

        endlife = false;

        atknumber = 30;


        ani = GetComponent<Animator>();


    }






    #region 行为


    //攻击
    State atk()
    {

        //模糊规则
        //与敌人距离0或近，则非常期待；
//与敌人距离中等AND子弹数量中等或高，则非常期待；
//子弹数量0或低，则不期待。


        //目标不为空
        if (targetgo != null)
        {

            //朝目标转向
            transform.LookAt(new Vector3(targetgo.transform.position.x,
                transform.position.y,targetgo.transform.position.z));

            //播放射击动画
            ani.Play("shoot");


            //减少子弹
            armo -= 1;


            atkflage = false;


        }



    }


    public GameObject get_army()
    {

        for(int i = 0; i < armylist.Count; i++)
        {
            if (armylist[i] != null)
            {
                if (armylist[i].GetComponent<armyai>().curblood > 0)
                {
                    return armylist[i];
                }
            }

        }

        return null;

    }


    //支援
    State go()
    {
        //模糊规则
        //子弹数量中等或多 AND与丧尸距离近或中等，则非常期待；
        //子弹数量中等，则期待；
        //子弹数量0或低，则不期待。


        //生命值中等或高且存活同伴数量不为0
     GameObject 同伴 =  get_army();
        if (同伴 != null)
        {
            //播放移动动画,前往同伴
            ani.Play("walk");

            nma.SetDestination(同伴.transform.position);


        }


    }

    //拯救
    State save()
    {

        //拯救
        //与敌人距离中等或远 AND 与濒死同伴距离0或近，则非常期待；
        //生命值高 AND 与濒死同伴距离0或近，则期待；
//生命值高 AND 与敌人距离近或中等，则期待；
//生命值低或中等 AND与濒死同伴距离中等，则不期待。



        //与濒死状态同伴距离0或近或中等
        GameObject 同伴 = get_army();
        if (同伴 != null)
        {

            //播放移动动画,前往同伴

            ani.Play("walk");

            nma.SetDestination(同伴.transform.position);


        }

    }

    //撤退
    State back()
    {
        //与掩体距离为 0 或近
        //躲避
        //与敌人距离近或中等 AND 子弹数量低，则非常期待；
        //与敌人距离中等或远，则不期待；
//生命值中等或高 AND 与同伴的距离0或近，则不期待；
//生命值中等或高 AND 子弹数量中等或多，则不期待

    }

    //躲避
    State miss()
    {

        //生命值低或中等且子弹数量0或少且存活同伴数量为0

        follow_player();

    }

     
    public void roload_bullet()
    {
        if (armo > 0)
        {
            armo += 20;

            //播放装弹音效
            roload_sound();
        }

    }

    public void follow_player()
    {


        nma.SetDestination(herogo.transform.position);


    }

    #endregion



    public void damage(float blood)
    {
        //重新计算时间 恢复血量
        wumiao_meishoudaoshanghai = false;
        wumiao_meishoudaoshanghai_time = 5;

       

        curblood -= blood;

    }


    public void shoot_sound()
    {
        if (!audio.isPlaying)
        {
            audio.PlayOneShot(gun_shootac);
        }


    }


    public void roload_sound()
    {
        if (!audio.isPlaying)
        {
            audio.PlayOneShot(armo_ac);
        }


    }

    //计算救治时间
    public void calculate_need_lifetime()
    {
        if (need_life_flage)
        {
            if (need_life_time <= 0)
            {

                gameObject.SetActive(false);
                endlife = true;

            }
            else
            {
                need_life_time -= Time.deltaTime;
            }


        }


    }


    //初始化行为树
    private Behavior InitBehaviorTree()
    {
        //底层
        //条件行为
        //1.发现家附近有老鼠?
        Behavior Condition_FindMouse = new Behavior();
        Condition_FindMouse.AddEvent(null, FindMouse, null);
        //2.身上花生数量满了?
        Behavior Condition_IsPeanutFull = new Behavior();
        Condition_IsPeanutFull.AddEvent(null, IsPeanutFull, null);
        //3.附近有花生?
        Behavior Condition_FindPeanut = new Behavior();
        Condition_FindPeanut.AddEvent(null, FindPeanut, null);


        //动作行为
        //1.追老鼠
        Behavior Action_MoveToMouse = new Behavior();
        Action_MoveToMouse.AddEvent(null, MoveToMouse, null);
        //2.攻击老鼠
        Behavior Action_AttackMouse = new Behavior();
        Action_AttackMouse.AddEvent(null, AttackMouse, null);
        //3.回家
        Behavior Action_GoHome = new Behavior();
        Action_GoHome.AddEvent(null, GoHome, null);
        //4.放花生
        Behavior Action_PickUpPeanut = new Behavior();
        Action_PickUpPeanut.AddEvent(null, PickUpPeanut, null);
        //5.去花生那里
        Behavior Action_MoveToPeanut = new Behavior();
        Action_MoveToPeanut.AddEvent(null, MoveToPeanut, null);
        //6.拾取花生
        Behavior Action_GetPeanut = new Behavior();
        Action_GetPeanut.AddEvent(null, GetPeanut, null);
        //7.寻找花生
        Behavior Action_MoveAround = new Behavior();
        Action_MoveAround.AddEvent(InitTargetPos, MoveAround, null);

        //模块行为
        Sequence Model_Defence = new Sequence();
        Model_Defence.AddChilds(Condition_FindMouse, Action_MoveToMouse, Action_AttackMouse);
        Sequence Model_MovePeanut = new Sequence();
        Model_MovePeanut.AddChilds(Condition_IsPeanutFull, Action_GoHome, Action_PickUpPeanut);
        Sequence Model_FindPeanut = new Sequence();
        Model_FindPeanut.AddChilds(Condition_FindPeanut, Action_MoveToPeanut, Action_GetPeanut);

        //根节点
        Selector Root = new Selector();
        Root.AddChilds(Model_Defence, Model_MovePeanut, Model_FindPeanut, Action_MoveAround);

        return Root;
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {



        if (targetgo != null)
        {

            driver.Running(root);
        }
        else
        {
            
        }

        //逻辑,无目标, 跟随玩家或者跟随队友
        

        //行为树,前提节点
        //攻击	与敌人距离为0或近或中等。
        //支援 生命值中等或高且存活同伴数量不为0。
        //救治 与濒死状态同伴距离0或近或中等。
         //躲避 与掩体距离为 0 或近。
          //撤退 生命值低或中等且子弹数量0或少且存活同伴数量为0。




        //有目标,行为树









        if (!atkflage)
        {
            if (atktime <= 0)
            {
                atktime = 1;
                atkflage = true;
            }
            else
            {
                atktime -= Time.deltaTime;
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
                //生成恢复血特效

                GameObject go = GameObject.Instantiate<GameObject>(recover_blood_effectgo, transform.position, Quaternion.identity);
                go.SetActive(true);
            }
            else
            {
                recover_time -= Time.deltaTime;
            }


        }


    }
}
