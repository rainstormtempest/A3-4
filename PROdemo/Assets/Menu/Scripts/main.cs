using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class main : MonoBehaviour
{


    public Button musicbtn;
    public GameObject winlosego;
    public Text winlosetxt;

    public Text score_txt;
    public Text target_score_txt;

    public int score;
    public int target_socre;

    public GameObject intoducego;
    public AudioSource audio;


    public Button exitbtn;

   

    // Start is called before the first frame update
    void Start()
    {
        target_socre = 6;
        musicbtn.onClick.AddListener(musicm);
        exitbtn.onClick.AddListener(exitm);
    }

    private void exitm()
    {
        SceneManager.LoadScene(0);
    }

    private void musicm()
    {
        if (audio.isPlaying)
        {
            audio.Stop();
        }
        else
        {
            audio.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {

        score_txt.text = "score:" + score;
        target_score_txt.text = "t_score:" + target_socre;

    }
}
