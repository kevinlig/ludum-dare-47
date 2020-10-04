using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Pixelplacement;
using UnityEngine.UI;
using TMPro;

public class GlobalUI : Singleton<GlobalUI>
{
    public TextMeshProUGUI alertLabel;
    public RawImage presby;

    public StringReactiveProperty alertMessage = new StringReactiveProperty("");

    void Start() {
        alertMessage
            .DistinctUntilChanged()
            .Subscribe((string message) => {
                alertLabel.text = message;

                if (!message.Equals("")) {
                    ClearMessage();
                }
            });

        GameManager.Instance.timeRemaining
            .Where((x) => x == 45)
            .Subscribe((x) => {
                ShowAd();
            });
    }

    void ClearMessage() {
        Observable.Timer(System.TimeSpan.FromSeconds(3f))
            .Subscribe((x) => {
                SetAlert("");
            });
    }

    void ShowAd() {
        Tween.AnchoredPosition(presby.rectTransform, new Vector2(20, 0), 0.4f, 0);
        Observable.Timer(System.TimeSpan.FromSeconds(8f))
            .Subscribe((x) => {
                Tween.AnchoredPosition(presby.rectTransform, new Vector2(-300, 0), 0.4f, 0);
            });
    }

    public void SetAlert(string message) {
        alertMessage.SetValueAndForceNotify(message);
    }

}
