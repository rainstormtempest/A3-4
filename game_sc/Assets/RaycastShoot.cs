using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastShoot : MonoBehaviour
{


    // 设置枪击带来的伤害值
    public int gunDamage = 1;

    // 设置两次枪击的间隔时间
    public float fireRate = 0.25f;

    // 设置玩家可以射击的Unity单位
    public float weaponRange = 50f;

    // 设置枪击为物体带来的冲击力
    public float hitForce = 100f;

    // GunEnd游戏对象
    public Transform gunEnd;

    // FPS相机
    private Camera fpsCam;

    // 设置射击轨迹显示的时间
    private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);

    // 枪击音效
    private AudioSource gunAudio;

    // 射击轨迹
  //  private LineRenderer laserLine;

    // 玩家上次射击后的间隔时间
    private float nextFire;


    public herocontroller hc;


    void Start()
    {


        hc= GameObject.FindWithTag("Player").GetComponent<herocontroller>();

        // 获取LineRenderer组件
        //   laserLine = GetComponent<LineRenderer>();

        // 获取AudioSource组件
        gunAudio = GetComponent<AudioSource>();

        // 获取Camera组件
        fpsCam = GetComponentInParent<Camera>();

    }


    void Update()
    {

        if (hc.cur_armo <= 0)
        {
            return;
        }

        // 检测是否按下射击键以及射击间隔时间是否足够
        if (Input.GetButtonDown("Fire1") && Time.time > nextFire)
        {

            // 射击之后更新间隔时间
            nextFire = Time.time + fireRate;

            // 启用ShotEffect携程控制射线显示及隐藏
            StartCoroutine(ShotEffect());

            // 在相机视口中心创建向量
            Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

            // 声明RaycastHit存储射线射中的对象信息
            RaycastHit hit;

            // 将射击轨迹起点设置为GunEnd对象的位置
          //  laserLine.SetPosition(0, gunEnd.position);

            // 检测射线是否碰撞到对象
            if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, weaponRange))
            {
                // 将射击轨迹终点设置为碰撞发生的位置
               // laserLine.SetPosition(1, hit.point);



                // 获取被射中对象上的ShootableBox组件
                enemyai health = hit.collider.GetComponent<enemyai>();

                // 如果组件存在
                if (health != null)
                {
                    // 调用组件的Damage函数计算伤害
                    health.damage(10);

                }

                // 检测被射中的对象是否存在rigidbody组件
                if (hit.rigidbody != null)
                {
                    // 为被射中的对象添加作用力
                    hit.rigidbody.AddForce(-hit.normal * hitForce);
                }
            }
            else
            {
                // 如果未射中任何对象，则将射击轨迹终点设为相机前方的武器射程最大距离处
              //  laserLine.SetPosition(1, rayOrigin + (fpsCam.transform.forward * weaponRange));
            }
        }
    }


    private IEnumerator ShotEffect()
    {
        // 播放音效
        gunAudio.Play();

        // 显示射击轨迹
      //  laserLine.enabled = true;

        // 等待0.07秒
        yield return shotDuration;

        // 等待结束后隐藏轨迹
       // laserLine.enabled = false;
    }

}
