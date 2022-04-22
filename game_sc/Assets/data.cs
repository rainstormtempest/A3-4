using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class data : MonoBehaviour
{
    //关卡索引 1 2 3 
    public int level_index;

    //士兵索引 1 2 3 4 
    public int player_index;


    public int weapon_index;

    public bool level1_open;
    public bool level2_open;
    public bool level3_open;

    // Start is called before the first frame update
    void Start()
    {

        level1_open = true;
        level2_open = true;
        level3_open = true;


        DontDestroyOnLoad(gameObject);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
