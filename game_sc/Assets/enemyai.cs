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
    //������
    public float atknumber;



    public float tilizhi;
    public float recover_time;


    public List<GameObject> cur_armylist;

    public GameObject playergo;
    //Ŀ���
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


        //��ȡ�о������б�

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


   

    //����ǹ�����߿������
    public void Scream_ani()
    {
        Scream_time = 1.0f;
        Scream_flage = true;
        ani.Play("Scream");

    }

    //����
    public void damage(float curblood)
    {
        if (is_dftyo)
        {
            tilizhi -= 10;
        }
        else
        {

            this.curblood -= curblood;
            Debug.LogError("����:" + this.curblood);
            zomble_damage_effect();

            if (this.curblood <= 0)
            {

                switch (zomble_type)
                {
                    case zombletype.smallboss:

                        playergo.GetComponent<herocontroller>().shadkle += 1;
                        
                        break;
                         

                }


                //���Źҵ���Ƶ
                play_zomble_die_ac();
                Destroy(gameObject);
            }
        }




    }


    //�ܵ��˺�������Ч
    public void zomble_damage_effect()
    {

      GameObject go = GameObject.Instantiate<GameObject>(damage_effect,transform.position,Quaternion.identity);
        go.SetActive(true);

    }

    //�����ֵ,ָ��
    public void  chouhen_hualine()
    {

        line.SetPosition(0, transform.position);
      //  line.SetPosition(1, targetgo.transform.position);


    }




    #region ��Ϊ



    State atk()
    {

        //ģ������
        //���������0�������ǳ��ڴ���
        //����������е�AND����ֵ�ߣ���ǳ��ڴ���
       //����ֵ�� AND ����ֵ�� AND ���������Զ�����ڴ�


        if (atkflage)
        {
            //���Ź�������
            ani.Play("atk");

            switch (target_type)
            {
                case targettype.army:
                    //��Ŀ���Ѫ
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
        //ģ������
        //��Ŀ���������е� AND ����ֵ�еȻ�ߣ���ǳ��ڴ���
//����ֵ�еȻ�ߣ����ڴ���
//����ֵ0��ͣ����ڴ���


        if (atkflage)
        {
            //���Ź�������
            ani.Play("jump_atk");

            switch (target_type)
            {
                case targettype.army:
                    //��Ŀ���Ѫ
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

        //�ӿ��ƶ��ٶ�

        ani.speed = miss_movespeed;

    }

    State idle()
    {

        //ԭ�صȴ�



    }


    State df()
    {
        //ģ������
        //����ֵ�ͣ���ǳ��ڴ���
        //����ֵ�еȣ����ڴ���
         //����ֵ�ߣ����ڴ���



        if (tilizhi >= 10)
        {
            //�������״̬
            is_dftyo = true;
        }
   


    }

    State skill()
    {
        //ģ������
        //����ֵ�ͻ��е� AND ����ֵ�ͻ��еȣ���ǳ��ڴ���
        //����ֵ�е� AND ����ֵ0��ͣ����ڴ���
        //����ֵ�� AND ����ֵ�ߣ����ڴ���




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



     



    //���Źҵ���Ƶ
    public void play_zomble_die_ac()
    {

        audio_com.PlayOneShot(die_ac);

    }





    //���ֵ��˷�ʽ
    public void find_player()
    {


       for(int i = 0; i < cur_armylist.Count; i++)
        {
            if (cur_armylist[i] != null)
            {
                if (cur_armylist[i].GetComponent<armyai>().curblood > 0)
                {

                    //��Ŀ��ľ���
                    float distance = Vector3.Distance(transform.position, cur_armylist[i].transform.position);

                    // ��ǰ��������
                    Vector3 froVec = transform.rotation * Vector3.forward;

                    // ��������
                    Vector3 dirVec = cur_armylist[i].transform.position - transform.position;


                    // �����������ļн�
                    float vectAngle = Mathf.Acos(Vector3.Dot(froVec.normalized, dirVec.normalized)) * Mathf.Rad2Deg;

                    //��ǰ�������ҵľ��� С�� ���ܵľ���
                    if (distance < senseDistance)
                    {
                        //�Ƕ�С�� ���ܵĽǶ�
                        if (vectAngle <= senseAngle * 0.5f)
                        {
                            //����ΪĿ��
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



        //������Ϊ��





    }


    //�ָ�������ֵ
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


    //��ʼ����Ϊ��
    private Behavior InitBehaviorTree()
    {
        //�ײ�
        //������Ϊ
        //1.���ּҸ���������?
        Behavior Condition_FindMouse = new Behavior();
        Condition_FindMouse.AddEvent(null, FindMouse, null);
        //2.���ϻ�����������?
        Behavior Condition_IsPeanutFull = new Behavior();
        Condition_IsPeanutFull.AddEvent(null, IsPeanutFull, null);
        //3.�����л���?
        Behavior Condition_FindPeanut = new Behavior();
        Condition_FindPeanut.AddEvent(null, FindPeanut, null);


        //������Ϊ
        //1.׷����
        Behavior Action_MoveToMouse = new Behavior();
        Action_MoveToMouse.AddEvent(null, MoveToMouse, null);
        //2.��������
        Behavior Action_AttackMouse = new Behavior();
        Action_AttackMouse.AddEvent(null, AttackMouse, null);
        //3.�ؼ�
        Behavior Action_GoHome = new Behavior();
        Action_GoHome.AddEvent(null, GoHome, null);
        //4.�Ż���
        Behavior Action_PickUpPeanut = new Behavior();
        Action_PickUpPeanut.AddEvent(null, PickUpPeanut, null);
        //5.ȥ��������
        Behavior Action_MoveToPeanut = new Behavior();
        Action_MoveToPeanut.AddEvent(null, MoveToPeanut, null);
        //6.ʰȡ����
        Behavior Action_GetPeanut = new Behavior();
        Action_GetPeanut.AddEvent(null, GetPeanut, null);
        //7.Ѱ�һ���
        Behavior Action_MoveAround = new Behavior();
        Action_MoveAround.AddEvent(InitTargetPos, MoveAround, null);

        //ģ����Ϊ
        Sequence Model_Defence = new Sequence();
        Model_Defence.AddChilds(Condition_FindMouse, Action_MoveToMouse, Action_AttackMouse);
        Sequence Model_MovePeanut = new Sequence();
        Model_MovePeanut.AddChilds(Condition_IsPeanutFull, Action_GoHome, Action_PickUpPeanut);
        Sequence Model_FindPeanut = new Sequence();
        Model_FindPeanut.AddChilds(Condition_FindPeanut, Action_MoveToPeanut, Action_GetPeanut);

        //���ڵ�
        Selector Root = new Selector();
        Root.AddChilds(Model_Defence, Model_MovePeanut, Model_FindPeanut, Action_MoveAround);

        return Root;
    }




    // Update is called once per frame
    void Update()
    {





        //�����Ŀ��,��ִ����Ϊ��
        //���û��Ŀ��,��ִ����Ϊ��,�ҵ�Ŀ��

        if (targetgo != null)
        {
            //����ģ���߼�ִ����Ϊ��

            //��ʬ�߼���
            //���� ǿ������ ���� �ȴ� ���� ��������

            //ǰ��������
            // ����    �������
            // ǿ������  ���ֵ�еȻ��߸�
            // ����       ������ֵ�еȻ��
            // ��������   ���ܵ��еȻ��߶�


            driver.Running(root);



        }
        else
        {
            //��ȡ����
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




        //����ֵС�ڵ���0
        if (tilizhi <= 0)
        {
            //����״̬Ϊ��
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
