using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    public static MenuManager instance;

    public bool is_in_game = false, is_config_open = false, is_pause_open = false, is_time_out_menu_open = false;
    public GameObject main_menu, game_ui, config_menu, pause_menu, time_out_menu;

    Coroutine coroutine;

    void Awake()
    {

        instance = this;

    }

    // Update is called once per frame
    void Update()
    {

        if (!is_in_game)
        {

            if (is_config_open)
            {

                if (Input.GetKeyDown(KeyCode.Escape))
                {

                    closeConfigMenu();

                }

            }
            else
            {

                if (Input.GetKeyDown(KeyCode.Escape))
                {

                    openConfigMenu();

                }

            }

        }
        else
        {
            if (!is_time_out_menu_open)
            {

                if (is_pause_open)
                {

                    if (Input.GetKeyDown(KeyCode.Escape))
                    {

                        closePauseMenu();

                    }

                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {

                    openPauseMenu();

                }
                
            }



        }

    }

    public void playButton()
    {

        main_menu.SetActive(false);
        game_ui.SetActive(true);

        GameManager.instance.startGame();

        is_in_game = true;

        AudioManager.instance.playAudio("select");

    }

    public void restartButton()
    {

        if (is_pause_open)
        {

            closePauseMenu();

        }
        else if (is_time_out_menu_open)
        {

            closeTimeOutMenu();

        }

        GameManager.instance.restartGame();

    }

    public void leaveButton()
    {
        if (is_pause_open)
        {

            closePauseMenu();

        }
        else if (is_time_out_menu_open)
        {

            closeTimeOutMenu();

        }

        main_menu.SetActive(true);
        game_ui.SetActive(false);

        GameManager.instance.leaveGame();

        is_in_game = false;

    }

    public void openConfigMenu()
    {

        popOnGameObject(config_menu);

        is_config_open = true;
        AudioManager.instance.playAudio("select");


    }

    public void closeConfigMenu()
    {

        popOffGameObject(config_menu);

        is_config_open = false;
        AudioManager.instance.playAudio("select");

    }

    public void openPauseMenu()
    {

        popOnGameObject(pause_menu);

        GameManager.instance.is_paused = true;

        is_pause_open = true;
        AudioManager.instance.playAudio("select");

    }

    public void closePauseMenu()
    {

        popOffGameObject(pause_menu);

        GameManager.instance.is_paused = false;

        is_pause_open = false;
        AudioManager.instance.playAudio("select");

    }

    public void openTimeOutMenu()
    {

        popOnGameObject(time_out_menu);

        is_time_out_menu_open = true;
        AudioManager.instance.playAudio("select");

    }

    public void closeTimeOutMenu()
    {

        popOffGameObject(time_out_menu);

        is_time_out_menu_open = false;
        AudioManager.instance.playAudio("select");

    }

    void popOnGameObject(GameObject obj_)
    {

        if (coroutine != null)
        {

            StopCoroutine(coroutine);

        }

        if (obj_.GetComponent<Background>() != null)
        {

            obj_.SetActive(true);

            obj_.GetComponent<Background>().body.transform.localScale = Vector3.zero;
            obj_.GetComponent<Background>().body.transform.LeanScale(Vector3.one, 0.5f).setEaseOutElastic();

        }
        else
        {

            obj_.SetActive(true);

            obj_.transform.localScale = Vector3.zero;
            obj_.transform.LeanScale(Vector3.one, 0.5f).setEaseOutElastic();

        }

    }

    void popOffGameObject(GameObject obj_)
    {

        if (coroutine != null)
        {

            StopCoroutine(coroutine);

        }

        coroutine = StartCoroutine(popOff(obj_));

    }

    IEnumerator popOff(GameObject obj_)
    {

        if (obj_.GetComponent<Background>() != null)
        {

            obj_.GetComponent<Background>().body.transform.LeanScale(Vector3.zero, 0.5f).setEaseOutElastic();

            yield return new WaitForSecondsRealtime(0.5f);

            obj_.SetActive(false);

        }
        else
        {

            obj_.transform.localScale = Vector3.zero;
            obj_.transform.LeanScale(Vector3.one, 0.3f).setEaseOutElastic();

            yield return new WaitForSecondsRealtime(0.1f);

            obj_.SetActive(false);

        }

    }
}
