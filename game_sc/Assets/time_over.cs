using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class time_over : MonoBehaviour
{
    public float time;


    // Start is called before the first frame update
    void Start()
    {

        time = 1;
        gameObject.SetActive(true);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (time <= 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            time -= Time.deltaTime;
        }
        
    }
}
