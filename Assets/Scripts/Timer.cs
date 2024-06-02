using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private static readonly Time instance = new Time();

    public static Time Instance
    {
        get { return instance; }
    }
    int day;
    int month;
    int year;
    int gameSpeed = 0;
    int prevGameSpeed = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            if (prevGameSpeed == 0)
            {
                prevGameSpeed = gameSpeed;
                gameSpeed = 0;
            }
            else
            {
                gameSpeed = prevGameSpeed;
                prevGameSpeed = 0;
            }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
            gameSpeed = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            gameSpeed = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            gameSpeed = 3;
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            gameSpeed = 4;
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            gameSpeed = 5;
        else if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            if (gameSpeed < 5)
                gameSpeed++;
        }
        else if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            if (gameSpeed > 0)
                gameSpeed--;
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        else gameSpeed = (int)transform.Find("Time Speed Slider").GetComponent<Slider>().value;
        transform.Find("Time Speed Slider").GetComponent<Slider>().value = gameSpeed;

        if (gameSpeed == 0)
            Time.timeScale = 0;
        else if (gameSpeed == 1)
            Time.timeScale = 0.025f;
        else if (gameSpeed == 2)
            Time.timeScale = 0.05f;
        else if (gameSpeed == 3)
            Time.timeScale = 0.1f;
        else if (gameSpeed == 4)
            Time.timeScale = 0.2f;
        else if (gameSpeed == 5)
            Time.timeScale = 0.4f;

    }

    private void FixedUpdate()
    {
        DateCalculation();
        transform.Find("Time Text").GetComponent<TextMeshProUGUI>().text = day + ":" + month + ":" + year;
    }

    void Awake()
    {

        day = 1;
        month = 1;
        year = 2024;
    }

    void DateCalculation()
    {
        day++;
        if (day > 30)
        {
            day = 1;
            month++;
        }
        if (month > 12)
        {
            month = 1;
            year++;
        }
    }
}
