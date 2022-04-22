using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gamemanager : MonoBehaviour
{

    public Transform mintf;
    public Transform maxtf;
     

    public GameObject armogo;

    public void init_armo()
    {

    float x  =  UnityEngine.Random.Range(mintf.position.x, maxtf.position.x);
        float z = UnityEngine.Random.Range(mintf.position.z, maxtf.position.z);


        GameObject.Instantiate<GameObject>(armogo, new Vector3(x, mintf.position.y, z), Quaternion.identity);


    }
    public float armotime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (armotime <= 0)
        {
            armotime = 1.0f;
            init_armo();
        }
        else
        {
            armotime -= Time.deltaTime;
        }


    }
}
