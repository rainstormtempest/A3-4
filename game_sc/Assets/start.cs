using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
 

public class start : MonoBehaviour
{
    public Button play_gamebtn;
    public Button exit_gamebtn;

    public Button chooselevelbtn;
    public Button chooseplayerbtn;


    public Button level1btn;
    public Button level2btn;
    public Button level3btn;


    public GameObject choosego;


    public GameObject choose_playergo;
    public Button choose_player_btn1;
    public Button choose_player_btn2;
    public Button choose_player_btn3;
    public Button choose_player_btn4;



    public GameObject choose_weapongo;
    public Button choose_wp_1btn;
    public Button choose_wp_2btn;
    public Button choose_wp_3btn;
    public Button choose_wp_4btn;



    public data data;

    // Start is called before the first frame update
    void Start()
    {


      //  chooselevelbtn.onClick.AddListener(choose_level_m);
       // chooseplayerbtn.onClick.AddListener(choose_player_m);


        if (GameObject.Find("data(Clone)") == null)
        {

          GameObject datago=  Resources.Load<GameObject>("data");
          GameObject go = GameObject.Instantiate<GameObject>(datago, transform.position, Quaternion.identity);

            data = go.GetComponent<data>();

        }
      



        choose_player_btn1.onClick.AddListener(cp1m);
        choose_player_btn2.onClick.AddListener(cp2m);
        choose_player_btn3.onClick.AddListener(cp3m);
        choose_player_btn4.onClick.AddListener(cp4m);


        play_gamebtn.onClick.AddListener(playgamebtnm);
        exit_gamebtn.onClick.AddListener(exitgamebtnm);

        level1btn.onClick.AddListener(level1m);
        level2btn.onClick.AddListener(level2m);
        level3btn.onClick.AddListener(level3m);


        choose_wp_1btn.onClick.AddListener(choose_wp1m);
        choose_wp_2btn.onClick.AddListener(choose_wp2m);
        choose_wp_3btn.onClick.AddListener(choose_wp3m);
        choose_wp_4btn.onClick.AddListener(choose_wp4m);


    }

    private void choose_wp4m()
    {

        data.weapon_index = 4;
        SceneManager.LoadScene(data.level_index);
    }

    private void choose_wp3m()
    {
        data.weapon_index = 3;
        SceneManager.LoadScene(data.level_index);
    }

    private void choose_wp2m()
    {
        data.weapon_index = 2;
        SceneManager.LoadScene(data.level_index);
    }

    private void choose_wp1m()
    {
        data.weapon_index = 1;
        SceneManager.LoadScene(data.level_index);
    }

  

    private void cp1m()
    {

        data.player_index = 1;
        choose_weapongo.SetActive(true);

    }

    private void cp2m()
    {
        data.player_index = 2;
        choose_weapongo.SetActive(true);
    }

    private void cp3m()
    {
        data.player_index = 3;
        choose_weapongo.SetActive(true);
    }

    private void cp4m()
    {
        data.player_index = 4;
        choose_weapongo.SetActive(true);

    }

    private void level1m()
    {
        data.level_index = 1;

        choose_playergo.SetActive(true);

    }

    private void level2m()
    {
        data.level_index = 2;
        choose_playergo.SetActive(true);
    }

    private void level3m()
    {
        data.level_index = 3;
        choose_playergo.SetActive(true);
    }

    private void playgamebtnm()
    {

        choosego.gameObject.SetActive(true);

    }

    private void exitgamebtnm()
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
