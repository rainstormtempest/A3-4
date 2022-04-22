using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sc_level1 : MonoBehaviour
{
    public Transform mintf;
    public Transform maxtf;


    public List<GameObject> zomble_list;

    private void Awake()
    {
        
         

    }

    public void init_zomble()
    {

        float x = UnityEngine.Random.Range(mintf.position.x, maxtf.position.x);
        float z = UnityEngine.Random.Range(mintf.position.z, maxtf.position.z);

      int index = UnityEngine.Random.Range(0, zomble_list.Count);
        GameObject zomble_go = zomble_list[index];

        GameObject.Instantiate<GameObject>(zomble_go, new Vector3(x, mintf.position.y, z), Quaternion.identity);


    }




    public void skill_init_zomble(Transform tf)
    {

        float x = UnityEngine.Random.Range(tf.position.x-5, tf.position.x+5);
        float z = UnityEngine.Random.Range(tf.position.z-5, tf.position.z+5);

        int index = UnityEngine.Random.Range(0, zomble_list.Count);
        GameObject zomble_go = zomble_list[index];

        GameObject.Instantiate<GameObject>(zomble_go, new Vector3(x, mintf.position.y, z), Quaternion.identity);


    }




    public float zomble_time;



    //大门钥匙
    public GameObject door_key;

    //生成大门钥匙集中点
    public List<GameObject> door_ket_int_key;

    // Start is called before the first frame update
    void Start()
    {

      int index = UnityEngine.Random.Range(0, door_ket_int_key.Count);

       GameObject.Instantiate<GameObject>(door_key, door_ket_int_key[index].transform.position, Quaternion.identity);
       

    }




    // Update is called once per frame
    void Update()
    {

        if (zomble_time <= 0)
        {
            zomble_time = 1.0f;
            init_zomble();
        }
        else
        {
            zomble_time -= Time.deltaTime;
        }




    }
}
