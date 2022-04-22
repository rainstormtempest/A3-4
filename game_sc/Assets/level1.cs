using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class level1 : MonoBehaviour
{

    public Image bloodimg;
    public Text blood_txt;

    public Text armo_txt;

    public GameObject ai_info;

    public Text item1_txt;
    public Text item2_txt;
    public Text item3_txt;

    public GameObject escgo;


    public Button esc_back_gamebtn;
    public Button esc_restart_gamebtn;
    public Button esc_back_main_gamebtn;
    public Button esc_exit_gamebtn;


    public GameObject tipsgo;
    public float tips_time;


    public herocontroller herogo;




    // Start is called before the first frame update
    void Start()
    {



        herogo = GameObject.FindWithTag("Player").GetComponent<herocontroller>();


        tips_time = 3;

        esc_back_gamebtn.onClick.AddListener(esc_backgame_m);
        esc_restart_gamebtn.onClick.AddListener(esc_restart_game_m);
        esc_back_main_gamebtn.onClick.AddListener(esc_back_main_game_m);
        esc_exit_gamebtn.onClick.AddListener(esc_exitgame_m);






    }

    private void esc_backgame_m()
    {
        Debug.LogError("按下了back按钮");
        escgo.SetActive(false);


    }

    private void esc_restart_game_m()
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);


    }

    private void esc_back_main_game_m()
    {

        SceneManager.LoadScene(1);


    }

    private void esc_exitgame_m()
    {

        Application.Quit();

    }



    public void add_ai_info()
    {

         




    }


    // Update is called once per frame
    void Update()
    {



        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 1:

                item1_txt.text = "doorkey:" + herogo.doorkey;
                item2_txt.text = "groundkey:" + herogo.ground_key;
                item3_txt.gameObject.SetActive(false);

                break;
            case 2:
                item1_txt.text = "shadkle:" + herogo.shadkle;
                item2_txt.gameObject.SetActive(false);
                item3_txt.gameObject.SetActive(false);
                break;
            case 3:
                item1_txt.gameObject.SetActive(false);
                item2_txt.gameObject.SetActive(false);
                item3_txt.gameObject.SetActive(false);
                break;


        }



        armo_txt.text = "armo:" + herogo.cur_armo + "/" + herogo.max_armo;

        bloodimg.fillAmount = herogo.curblood / herogo.maxblood;

        blood_txt.text = herogo.curblood + "/" + herogo.maxblood;

        


        if (tips_time <= 0)
        {
            tipsgo.SetActive(false);
        }
        else
        {
            tips_time -= Time.deltaTime;
        }


        if (Input.GetKeyDown(KeyCode.Tab))
        {

            if (ai_info.activeSelf)
            {
                ai_info.SetActive(false);
            }
            else
            {
                ai_info.SetActive(true);
            }


        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (escgo.activeSelf)
            {
                escgo.SetActive(false);
            }
            else
            {
                escgo.SetActive(true);
            }

        }

        
    }
}
