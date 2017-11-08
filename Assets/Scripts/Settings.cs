using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour {
    public Text lineValue;
    public GameObject slider;

    private float drawDistance;

	public void ForceToggle(bool value)
    {
        int isOn = value ? 1 : 0;
        PlayerPrefs.SetInt("ForceToggle", isOn);
    }

    public void RandomizeToggle(bool value)
    {
        int isOn = value ? 1 : 0;
        PlayerPrefs.SetInt("RandomizeToggle", isOn);
    }

    public void ShowLineToggle(bool value)
    {
        int isOn = value ? 1 : 0;
        slider.GetComponent<Slider>().interactable = value;
        if (value)
        {
            lineValue.CrossFadeAlpha(1f, 0.1f, false);
        }
        else
        {
            lineValue.CrossFadeAlpha(0.5f, 0.1f, false);
        }
        PlayerPrefs.SetInt("ShowLineToggle", isOn);
    }
    public void RehabToggle(bool value)
    {
        int isOn = value ? 1 : 0;
        PlayerPrefs.SetInt("RehabToggle", isOn);
    }

    public void LineSliderValue(float value)
    {
        drawDistance = value;
        lineValue.text = value.ToString("F2");
    }

    public void LoadMenu()
    {
        PlayerPrefs.SetFloat("DrawDistance", drawDistance);
        SceneManager.LoadScene("Menu");
    }
}
