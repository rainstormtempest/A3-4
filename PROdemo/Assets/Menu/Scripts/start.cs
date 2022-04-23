using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class start : MonoBehaviour
{

    public Button btn1;
    public Button btn2;
    public Button btn3;
    public Button btn4;

    public Button helpbtn;

    public GameObject helpgo;

    // Start is called before the first frame update
    void Start()
    {
        btn1.onClick.AddListener(btn1m);
        btn2.onClick.AddListener(btn2m);
        btn3.onClick.AddListener(btn3m);
        btn4.onClick.AddListener(btn4m);

        helpbtn.onClick.AddListener(helpbtnm);
    }

    private void helpbtnm()
    {

        if (helpgo.activeSelf)
        {
            helpgo.SetActive(false);
        }
        else
        {
            helpgo.SetActive(true);
        }

    }

    private void btn4m()
    {
        SceneManager.LoadScene(4);
    }

    private void btn3m()
    {
        SceneManager.LoadScene(3);
    }

    private void btn2m()
    {
        SceneManager.LoadScene(2);
    }

    private void btn1m()
    {
        SceneManager.LoadScene(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
