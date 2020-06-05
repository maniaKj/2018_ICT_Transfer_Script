using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause_Window : MonoBehaviour {

    [SerializeField] GameObject[] menu_Select_Windows = new GameObject[2];

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++) menu_Select_Windows[i] = transform.GetChild(i).gameObject;
        menu_Select_Windows[0].SetActive(false);
        menu_Select_Windows[1].SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if(menu_Select_Windows[0].activeSelf || menu_Select_Windows[1].activeSelf)
            {
                Button_Resume();
            }
            else
            {
                menu_Select_Windows[0].SetActive(true);
                menu_Select_Windows[1].SetActive(false);

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0;
            }
        }
    }

    public void Button_Resume()
    {
        menu_Select_Windows[0].SetActive(false);
        menu_Select_Windows[1].SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
    }

    public void Button_Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Button_Show_Level_Select_Window()
    {
        menu_Select_Windows[0].SetActive(false);
        menu_Select_Windows[1].SetActive(true);
    }

    public void Button_MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void Button_Level_Select_Window_Back()
    {
        menu_Select_Windows[0].SetActive(true);
        menu_Select_Windows[1].SetActive(false);
    }

    public void Button_Move_To_Level(int num)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(num);
    }
}
