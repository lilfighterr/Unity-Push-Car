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
    public GameObject directionToggle;
    public GameObject handleToggle;
    public GameObject rehabToggle;
    public GameObject showLineToggle;
    public GameObject evaluationType;
    public GameObject calibrateButton;
    public GameObject rehabDropDown;
    public GameObject evaluationDropDown;
    public GameObject timeInput;
    public GameObject lengthInput;
    public GameObject errorText;
    

    private float drawDistance;
    private bool isRehab;
    private int evalType;
    private Dropdown rehabDropScript;
    private Dropdown evalDropScript;
    private InputField timeScript;
    private InputField lengthScript;

    private void Start()
    {
        //Initialize values
        isRehab = PlayerPrefs.GetInt("RehabToggle", 0) == 1 ? true : false;
        slider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("DrawDistance", 0.1f);
        randomizeToggle.GetComponent<Toggle>().isOn = PlayerPrefs.GetInt("RandomizeToggle", 1) == 1 ? true : false;
        directionToggle.GetComponent<Toggle>().isOn = PlayerPrefs.GetInt("DirectionToggle", 1) == 1 ? true : false;
        handleToggle.GetComponent<Toggle>().isOn = PlayerPrefs.GetInt("HandleToggle", 1) == 1 ? true : false;
        showLineToggle.GetComponent<Toggle>().isOn = PlayerPrefs.GetInt("ShowLineToggle", 1) == 1 ? true : false;

        forceToggle.GetComponent<Toggle>().isOn = PlayerPrefs.GetInt("ForceToggle", 1) == 1 ? true : false;
        rehabToggle.GetComponent<Toggle>().isOn = isRehab;
        calibrateButton.GetComponent<Button>().interactable = isRehab;
        forceToggle.GetComponent<Toggle>().interactable = isRehab;

        rehabDropScript = rehabDropDown.GetComponent<Dropdown>();
        rehabDropScript.value = PlayerPrefs.GetInt("IPAddressIndex", 0);

        evalDropScript = evaluationDropDown.GetComponent<Dropdown>();
        evalDropScript.value = PlayerPrefs.GetInt("EvalType", 0);
        timeScript = timeInput.GetComponent<InputField>();
        lengthScript = lengthInput.GetComponent<InputField>();
        if (evalDropScript.value == 0)
        {
            lengthScript.text = PlayerPrefs.GetFloat("Length", 40).ToString("F2");
            lengthInput.SetActive(true);
        }
        else
        {
            timeScript.text = PlayerPrefs.GetFloat("Time", 30).ToString("F2");
            timeInput.SetActive(true);
        }
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

    public void DirectionToggle(bool value)
    {
        int isOn = value ? 1 : 0;
        PlayerPrefs.SetInt("DirectionToggle", isOn);
    }

    public void HandleToggle(bool value)
    {
        int isOn = value ? 1 : 0;
        PlayerPrefs.SetInt("HandleToggle", isOn);
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
        rehabDropDown.GetComponent<Dropdown>().interactable = value;
        PlayerPrefs.SetInt("RehabToggle", isOn);
    }

    public void LineSliderValue(float value)
    {
        drawDistance = value;
        lineValue.text = value.ToString("F2");
    }

    public void IpDropdown(int value)
    {
        string chosenOption = rehabDropScript.captionText.text;
        PlayerPrefs.SetString("IPAddress", chosenOption);
        PlayerPrefs.SetInt("IPAddressIndex", value);
    }

    public void EvalDropDown(int value)
    {
        if (value == 0) //length
        {
            timeInput.SetActive(false);
            lengthInput.SetActive(true);
        }
        else //time
        {
            timeInput.SetActive(true);
            lengthInput.SetActive(false);
        }
        evalType = value;
        PlayerPrefs.SetInt("EvalType", value);
    }

    public void LoadMenu()
    {
        PlayerPrefs.SetFloat("DrawDistance", drawDistance);
        if (InputFieldChecks())
        {
            SceneManager.LoadScene("Menu");
        }
    }

    public void LoadCalibrate()
    {
        PlayerPrefs.SetFloat("DrawDistance", drawDistance);
        if (InputFieldChecks())
        {
            SceneManager.LoadScene("Calibrate");
        }

    }

    private bool InputFieldChecks()
    {
        bool result;
        if (evalType == 0) //fixed length
        {
            float length;
            result = float.TryParse(lengthScript.text, out length);
            if (result)
            {
                PlayerPrefs.SetFloat("Length", length);
            }
            else
            {
                errorText.SetActive(true);
            }
        }
        else //fixed time
        {
            float time;
            result = float.TryParse(timeScript.text, out time);
            if (result)
            {
                PlayerPrefs.SetFloat("Time", time);
            }
            else
            {
                errorText.SetActive(true);
            }
        }
        return result;
    }
}
