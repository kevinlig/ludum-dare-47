﻿using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;

public class AutopilotController : MonoBehaviour
{
    public TextMeshProUGUI validationLabel;
    public TMP_InputField inputField;
    public CockpitCamera cockpitCamera;

    public GameObject transitionAnim;

    private Regex inputRegex = new Regex("[^0-9.]");
    private Regex validationRegex = new Regex("^[0-9]*(\\.[0-9]+)?$");

    void Start() {
        validationLabel.text = "";
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            cockpitCamera.CloseAutopilot();
        } else if (Input.GetKeyDown(KeyCode.Return)) {
            SubmitValue();
        }
    }

    void OnEnable() {
        inputField.text = "";
        validationLabel.text = "";
    }

    public void OnTextChange(string input) {
        inputField.text = inputRegex.Replace(input, "");
    }

    void SubmitValue() {
        string input = inputField.text;
        float amount = 0f;
        if (!validationRegex.IsMatch(input)) {
            validationLabel.text = "[ERROR]: Invalid input.";
            return;
        }

        amount = float.Parse(input);
        float maxDistance = GameManager.Instance.fuelAvailable.Value * 5f;
        if (amount > maxDistance) {
            validationLabel.text = "[ERROR]: Insufficient fuel.";
            return;
        }

        if (amount <= 0f) {
            validationLabel.text = "[ERROR]: Invalid input.";
            return;
        }

        if (amount + GameManager.Instance.latitude.Value > 90f) {
            validationLabel.text = "[ERROR]: This is beyond your delivery area.";
            return;
        }

        GameManager.Instance.FastTravel(amount);
        transitionAnim.SetActive(true);
         Observable.Timer(System.TimeSpan.FromSeconds(1.5f))
            .Subscribe((x) => {
                cockpitCamera.CloseAutopilot();
            });
    }
}
