using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;

public class EndController : MonoBehaviour
{
    public TextMeshProUGUI label;
    int missing = 0;
    private string template = "<size=48>{0}</size>\n{1}";    

    void Start() {
        GameManager.Instance.deliveriesRemaining
            .Subscribe(HandleEnd);
    }

    void HandleEnd(int remaining) {
        bool success = remaining == 0;
        missing = remaining;

        if (success) {
            ShowSuccess();
        } else {
            ShowFailure();
        }
    }

    void ShowSuccess() {
        int elapsedTime = GameManager.Instance.totalTime - (int)GameManager.Instance.timeRemaining.Value;
        string detail = string.Format("You delivered all your packages in {0}!", FormatTime(elapsedTime));
        string output = string.Format(template, "SUCCESS", detail);

        label.text = output;
    }

    void ShowFailure() {
        string plural = "";
        if (missing != 1) {
            plural = "s";
        }
        string detail = string.Format("You did not deliver {0} package{1}.", missing, plural);
        string output = string.Format(template, "FAILURE", detail);

        label.text = output;
    }

    string FormatTime(int seconds) {
        string output = "";
        int minutes = Mathf.FloorToInt(seconds / 60f);
        long sec = seconds % 60;
        string padding = "";
        if (sec < 10) {
            padding = "0";
        }

        output = string.Format("{0}:{1}{2}", minutes, padding, sec);

        return output;
    }
}
