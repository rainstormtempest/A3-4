using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hero : MonoBehaviour
{

    public Camera myCamera;//在Inspector面板中赋值

    private Rigidbody myRigidbody;

    public float speed = 5;           //人物运动速度

    public float rotateSpeed = 20;//镜头旋转速度

    private Vector3 offset;

    private float cameraRotate;//记录当前摄像机的旋转角度

    public Animator ani;


    public main main;

    private void Start()

    {

        main = GameObject.Find("Canvas").GetComponent<main>();

        ani = GetComponent<Animator>();

        myRigidbody = this.GetComponent<Rigidbody>();

        offset = this.transform.position;//记录人物初始位置

    }


    private void Update()
    {

        Collider[] ary = Physics.OverlapSphere(transform.position, 1.0f, 1 << LayerMask.NameToLayer("item"));

         for (int i = 0; i < ary.Length; i++)
        {
            if (ary[i] != null)
            {
                Debug.LogError("name=" + ary[i].gameObject.name);
                main.score += 1;
                if (main.score >= main.target_socre)
                {
                    main.winlosego.SetActive(true);
                    main.winlosetxt.text = "WIN";
                }
                Destroy(ary[i].gameObject);
            }


        }

    }


    private void FixedUpdate()

    {





        //人物当前运动速度

        Vector3 vel = myRigidbody.velocity;

        //获取WASD

        float v = Input.GetAxis("Vertical");

        float h = Input.GetAxis("Horizontal");





        if (Mathf.Abs(h) > 0.05f || Mathf.Abs(v) > 0.05f)//判断：如果不是误触键盘则执行下一语句
        {

            //修改人物的运动状态

            float sr = Mathf.Sin(cameraRotate);

            float cr = Mathf.Cos(cameraRotate);

            myRigidbody.velocity =

              new Vector3((v * sr + h * cr) * speed, vel.y, (v * cr - h * sr) * speed);//不能乘Time.deltaTime！否则速度太慢。

            //运动动画......





         //   ani.Play("walk");


            //WASD改变人物朝向

            this.transform.rotation = Quaternion.LookRotation(new Vector3((v * sr + h * cr), 0, (v * cr - h * sr)));



        }
        else
        {


          //  ani.Play("idle");

        }

    }



    private void LateUpdate()
    {


        //摄像机跟随人物移动，移动量=人物当前位置-人物上一位置

        myCamera.transform.position += this.transform.position - offset;

        offset = this.transform.position;

        if (Input.GetKeyDown(KeyCode.Space))
        {

            myRigidbody.velocity = new Vector3(myRigidbody.velocity.x, 10, myRigidbody.velocity.z);

        }

        //摄像机视角旋转

        float mouseX = Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;

        float mouseY = Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;

        myCamera.transform.RotateAround(this.transform.position, Vector3.up, mouseX);



        //记录摄像机当前旋转角度，并转换为弧度

        cameraRotate = myCamera.transform.eulerAngles.y * Mathf.Deg2Rad;





    }
}
