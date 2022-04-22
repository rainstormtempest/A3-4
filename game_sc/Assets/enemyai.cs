using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MyBehaviorTree;
using BehaviorTree;
using UnityEngine.SceneManagement;


public enum zombletype
{
    bigboos,
    smallboss,
    normal,
    special,

}

public enum targettype
{
    army,
    hero,

}

 


public class enemyai : MonoBehaviour
{
    public float chouhenzhi;
    public LineRenderer line;



    public targettype target_type;
    public AudioSource audio_com;
    public AudioClip die_ac;

    public zombletype zomble_type;
    public GameObject damage_effect;


    public Animator ani;
    public float curblood;
    public float maxblood;


    private float senseDistance = 8;

    private float senseAngle = 90;
    //攻击力
    public float atknumber;



    public float tilizhi;
    public float recover_time;


    public List<GameObject> cur_armylist;

    public GameObject playergo;
    //目标点
    public Vector3 targetpos;

    public NavMeshAgent nma;
    public GameObject targetgo;


    public GameObject bigboos_atk_effectgo;


    public float atktime;
    public bool atkflage;



    public float normal_movespeed;
    public float miss_movespeed;


    public bool is_dftyo;
    public int skillnumber;


    public sc_level1 level1;


    public bool Scream_flage;
    public float Scream_time;


    private  BehaviorTree.Driver driver;
    private Behavior root;



    private void Awake()
    {

        driver = new BehaviorTree.Driver();
        root = InitBehaviorTree();



        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 1:
                level1 = GameObject.Find("gamemanager").GetComponent<sc_level1>();
                break;
            case 2:
                break;
            case 3:
                break;

        }


        skillnumber = 10;

        is_dftyo = false;

        normal_movespeed = 3;
        miss_movespeed = 5;


        atktime = 1;
        atkflage = false;


        playergo = GameObject.FindWithTag("Player");


        ani = GetComponent<Animator>();

        switch (zomble_type)
        {
            case zombletype.bigboos:

                curblood = 100;
                maxblood = 100;
                atknumber = 100;
                break;
            case zombletype.normal:

                curblood = 100;
                maxblood = 100;
                atknumber = 100;
                break;
            case zombletype.smallboss:

                curblood = 100;
                maxblood = 100;
                atknumber = 100;
                break;
            case zombletype.special:

                curblood = 100;
                maxblood = 100;
                atknumber = 100;
                break;

        }


        //获取敌军加入列表

        cur_armylist = new List<GameObject>();

        GameObject army1go = GameObject.FindWithTag("army1");
        GameObject army2go = GameObject.FindWithTag("army2");
        GameObject army3go = GameObject.FindWithTag("army3");
        GameObject army4go = GameObject.FindWithTag("army4");

  
        if (army1go != null)
        {
            cur_armylist.Add(army1go);
        }
        if (army2go != null)
        {
            cur_armylist.Add(army2go);
        }
        if (army3go != null)
        {
            cur_armylist.Add(army3go);
        }
        if (army4go != null)
        {
            cur_armylist.Add(army4go);
        }
      

    }


   

    //听到枪声或者看到玩家
    public void Scream_ani()
    {
        Scream_time = 1.0f;
        Scream_flage = true;
        ani.Play("Scream");

    }

    //受伤
    public void damage(float curblood)
    {
        if (is_dftyo)
        {
            tilizhi -= 10;
        }
        else
        {

            this.curblood -= curblood;
            Debug.LogError("受伤:" + this.curblood);
            zomble_damage_effect();

            if (this.curblood <= 0)
            {

                switch (zomble_type)
                {
                    case zombletype.smallboss:

                        playergo.GetComponent<herocontroller>().shadkle += 1;
                        
                        break;
                         

                }


                //播放挂掉音频
                play_zomble_die_ac();
                Destroy(gameObject);
            }
        }




    }


    //受到伤害生成特效
    public void zomble_damage_effect()
    {

      GameObject go = GameObject.Instantiate<GameObject>(damage_effect,transform.position,Quaternion.identity);
        go.SetActive(true);

    }

    //仇恨数值,指向
    public void  chouhen_hualine()
    {

        line.SetPosition(0, transform.position);
      //  line.SetPosition(1, targetgo.transform.position);


    }




    #region 行为



    State atk()
    {

        //模糊规则
        //与人类距离0或近，则非常期待；
        //与人类距离中等AND生命值高，则非常期待；
       //生命值低 AND 体力值低 AND 与人类距离远，则不期待


        if (atkflage)
        {
            //播放攻击动画
            ani.Play("atk");

            switch (target_type)
            {
                case targettype.army:
                    //让目标掉血
                    targetgo.GetComponent<armyai>().damage(atknumber);
                    break;

                case targettype.hero:
                    targetgo.GetComponent<herocontroller>().damage(atknumber);
                    break;

            }


            atkflage = false;
        }



    }

    State jumpatk()
    {
        //模糊规则
        //与目标距离近或中等 AND 体力值中等或高，则非常期待；
//体力值中等或高，则期待；
//体力值0或低，则不期待。


        if (atkflage)
        {
            //播放攻击动画
            ani.Play("jump_atk");

            switch (target_type)
            {
                case targettype.army:
                    //让目标掉血
                    targetgo.GetComponent<armyai>().damage(atknumber);
                    break;

                case targettype.hero:
                    targetgo.GetComponent<herocontroller>().damage(atknumber);
                    break;

            }


            atkflage = false;
        }



    }


    State miss()
    {

        //加快移动速度

        ani.speed = miss_movespeed;

    }

    State idle()
    {

        //原地等待



    }


    State df()
    {
        //模糊规则
        //生命值低，则非常期待；
        //生命值中等，则期待；
         //生命值高，则不期待。



        if (tilizhi >= 10)
        {
            //进入防御状态
            is_dftyo = true;
        }
   


    }

    State skill()
    {
        //模糊规则
        //生命值低或中等 AND 体力值低或中等，则非常期待；
        //生命值中等 AND 体力值0或低，则期待；
        //生命值高 AND 体力值高，则不期待。




        switch (zomble_type)
        {
            case zombletype.bigboos:

                if (skillnumber >= 2)
                {
                    skillnumber -= 2;

                    for(int i = 0; i < 10; i++)
                    {

                        level1.skill_init_zomble(transform);
                        
                    }
                }


                break;
            case zombletype.smallboss:

                if (skillnumber >= 2)
                {
                    skillnumber -= 2;

                    for (int i = 0; i < 10; i++)
                    {

                        level1.skill_init_zomble(transform);

                    }
                }

                break;

        }


    }


    #endregion



     



    //播放挂掉音频
    public void play_zomble_die_ac()
    {

        audio_com.PlayOneShot(die_ac);

    }





    //发现敌人方式
    public void find_player()
    {


       for(int i = 0; i < cur_armylist.Count; i++)
        {
            if (cur_armylist[i] != null)
            {
                if (cur_armylist[i].GetComponent<armyai>().curblood > 0)
                {

                    //与目标的距离
                    float distance = Vector3.Distance(transform.position, cur_armylist[i].transform.position);

                    // 正前方的向量
                    Vector3 froVec = transform.rotation * Vector3.forward;

                    // 方向向量
                    Vector3 dirVec = cur_armylist[i].transform.position - transform.position;


                    // 求两个向量的夹角
                    float vectAngle = Mathf.Acos(Vector3.Dot(froVec.normalized, dirVec.normalized)) * Mathf.Rad2Deg;

                    //当前对象和玩家的距离 小于 技能的距离
                    if (distance < senseDistance)
                    {
                        //角度小于 技能的角度
                        if (vectAngle <= senseAngle * 0.5f)
                        {
                            //设置为目标
                            targetgo = cur_armylist[i];


                            Scream_ani();
                        }
                    }
                   


                }

            }


        }


       

    }

    


        // Start is called before the first frame update
        void Start()
    {

        nma = GetComponent<NavMeshAgent>();



        //构建行为树





    }


    //恢复体力数值
    public void recover_tilizhi()
    {

        if (recover_time <= 0)
        {
            recover_time = 60;
            switch (zomble_type)
            {
                case zombletype.bigboos:
                    tilizhi += 10;
                    break;
                case zombletype.normal:
                    break;
                case zombletype.smallboss:
                    tilizhi += 10;
                    break;
                case zombletype.special:
                    break;

            }
        }
        else
        {
            recover_time -= Time.deltaTime;
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




    // Update is called once per frame
    void Update()
    {





        //如果有目标,则执行行为树
        //如果没有目标,则不执行行为树,找到目标

        if (targetgo != null)
        {
            //根据模糊逻辑执行行为树

            //僵尸逻辑：
            //攻击 强化攻击 闪避 等待 防御 发动技能

            //前提条件：
            // 攻击    发现玩家
            // 强化攻击  仇恨值中等或者高
            // 防御       体力数值中等或高
            // 发动技能   技能点中等或者多


            driver.Running(root);



        }
        else
        {
            //获取对象
            find_player();


        }





        if (Scream_flage)
        {
            if (Scream_time <= 0)
            {
                Scream_flage = false;
            }
            else
            {
                Scream_time -= Time.deltaTime;
            }

        }




        //体力值小于等于0
        if (tilizhi <= 0)
        {
            //防御状态为假
            is_dftyo = false;
        }

         
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


        chouhen_hualine();
        recover_tilizhi();
    }
}
