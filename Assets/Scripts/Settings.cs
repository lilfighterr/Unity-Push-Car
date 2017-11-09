using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour {
    public Text lineValue;
    public GameObject slider;
    public GameObject forceToggle;
    public GameObject randomizeToggle;
    public GameObject rehabToggle;
    public GameObject showLineToggle;
    public GameObject evaluationType;
    public GameObject calibrateButton;

    private float drawDistance;
    private bool isRehab;

    private void Start()
    {
        isRehab = PlayerPrefs.GetInt("RehabToggle", 1) == 1 ? true : false;
        slider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("DrawDistance", 0.1f);
        randomizeToggle.GetComponent<Toggle>().isOn = PlayerPrefs.GetInt("RandomizeToggle", 1) == 1 ? true : false;
        showLineToggle.GetComponent<Toggle>().isOn = PlayerPrefs.GetInt("ShowLineToggle", 1) == 1 ? true : false;

        forceToggle.GetComponent<Toggle>().isOn = PlayerPrefs.GetInt("ForceToggle", 1) == 1 ? true : false;
        rehabToggle.GetComponent<Toggle>().isOn = isRehab;
        calibrateButton.GetComponent<Button>().interactable = isRehab;
        forceToggle.GetComponent<Toggle>().interactable = isRehab;
    }

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
        calibrateButton.GetComponent<Button>().interactable = value;
        forceToggle.GetComponent<Toggle>().interactable = value;
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
