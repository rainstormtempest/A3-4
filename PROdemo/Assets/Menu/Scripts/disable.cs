using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disable : MonoBehaviour
{
    public float time;

    // Start is called before the first frame update
    void Start()
    {
        time = 3;
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
