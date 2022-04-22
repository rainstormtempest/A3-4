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


    //������
    public float atknumber;

    //Ŀ��
    public GameObject targetgo;


    public GameObject recover_blood_effectgo;


    public bool wumiao_meishoudaoshanghai;
    public float wumiao_meishoudaoshanghai_time;

    public bool recove_blood;
    public float recover_time;

    public Animator ani;

    public List<GameObject> armylist;


    public float chouhenzhi;

    //���׹ҵ�
    public bool endlife;

    //Ŀ���
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






    #region ��Ϊ


    //����
    State atk()
    {

        //ģ������
        //����˾���0�������ǳ��ڴ���
//����˾����е�AND�ӵ������еȻ�ߣ���ǳ��ڴ���
//�ӵ�����0��ͣ����ڴ���


        //Ŀ�겻Ϊ��
        if (targetgo != null)
        {

            //��Ŀ��ת��
            transform.LookAt(new Vector3(targetgo.transform.position.x,
                transform.position.y,targetgo.transform.position.z));

            //�����������
            ani.Play("shoot");


            //�����ӵ�
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


    //֧Ԯ
    State go()
    {
        //ģ������
        //�ӵ������еȻ�� AND��ɥʬ��������еȣ���ǳ��ڴ���
        //�ӵ������еȣ����ڴ���
        //�ӵ�����0��ͣ����ڴ���


        //����ֵ�еȻ���Ҵ��ͬ��������Ϊ0
     GameObject ͬ�� =  get_army();
        if (ͬ�� != null)
        {
            //�����ƶ�����,ǰ��ͬ��
            ani.Play("walk");

            nma.SetDestination(ͬ��.transform.position);


        }


    }

    //����
    State save()
    {

        //����
        //����˾����еȻ�Զ AND �����ͬ�����0�������ǳ��ڴ���
        //����ֵ�� AND �����ͬ�����0��������ڴ���
//����ֵ�� AND ����˾�������еȣ����ڴ���
//����ֵ�ͻ��е� AND�����ͬ������еȣ����ڴ���



        //�����״̬ͬ�����0������е�
        GameObject ͬ�� = get_army();
        if (ͬ�� != null)
        {

            //�����ƶ�����,ǰ��ͬ��

            ani.Play("walk");

            nma.SetDestination(ͬ��.transform.position);


        }

    }

    //����
    State back()
    {
        //���������Ϊ 0 ���
        //���
        //����˾�������е� AND �ӵ������ͣ���ǳ��ڴ���
        //����˾����еȻ�Զ�����ڴ���
//����ֵ�еȻ�� AND ��ͬ��ľ���0��������ڴ���
//����ֵ�еȻ�� AND �ӵ������еȻ�࣬���ڴ�

    }

    //���
    State miss()
    {

        //����ֵ�ͻ��е����ӵ�����0�����Ҵ��ͬ������Ϊ0

        follow_player();

    }

     
    public void roload_bullet()
    {
        if (armo > 0)
        {
            armo += 20;

            //����װ����Ч
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
        //���¼���ʱ�� �ָ�Ѫ��
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

    //�������ʱ��
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

        //�߼�,��Ŀ��, ������һ��߸������
        

        //��Ϊ��,ǰ��ڵ�
        //����	����˾���Ϊ0������еȡ�
        //֧Ԯ ����ֵ�еȻ���Ҵ��ͬ��������Ϊ0��
        //���� �����״̬ͬ�����0������еȡ�
         //��� ���������Ϊ 0 �����
          //���� ����ֵ�ͻ��е����ӵ�����0�����Ҵ��ͬ������Ϊ0��




        //��Ŀ��,��Ϊ��









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


        //�ָ�Ѫ
        if (wumiao_meishoudaoshanghai)
        {

            if (recover_time <= 0)
            {
                recover_time = 10;
                curblood += 10;
                //���ɻָ�Ѫ��Ч

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
