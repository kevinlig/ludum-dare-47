using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;

public class DashboardController : MonoBehaviour
{
    public Texture blueBox;
    public Texture yellowBox;
    public Texture inactiveBox;
    public RawImage[] deliveryBoxImages;
    public TextMeshProUGUI[] deliveryBoxLabels;

    public RawImage[] fuelImages;
    public TextMeshProUGUI[] fuelLabels;

    public TextMeshProUGUI mainTimer;
    public TextMeshProUGUI fuelTimer;

    private bool fuelTimerRunning = false;
    
    void Start() {
        GameManager.Instance.deliveryDestinations
            .Subscribe((Dictionary<int, int> deliveries) => {
                HandleDeliveries(deliveries);
            });

        GameManager.Instance.fuelAvailable
            .Subscribe(HandleFuel);

        StartMainTimer();
    }

    void HandleDeliveries(Dictionary<int, int> deliveries) {
        List<int> destinations = deliveries.Keys.ToList().OrderBy((o) => o).ToList();
        for (int i = 0; i < 5; i++) {
            if (destinations.Count <= i) {
                deliveryBoxImages[i].texture = inactiveBox;
                deliveryBoxLabels[i].text = "";
            } else {
                deliveryBoxImages[i].texture = blueBox;
                deliveryBoxLabels[i].text = string.Format("{0}", destinations[i]);
            }
        }
    }

    void HandleFuel(int currentFuel) {
        for (int i = 0; i < 3; i++) {
            RawImage currentImage = fuelImages[i];
            TextMeshProUGUI currentLabel = fuelLabels[i];

            currentImage.texture = inactiveBox;
            currentLabel.text = "";

            if (i < currentFuel) {
                currentImage.texture = yellowBox;
                currentLabel.text = string.Format("SEG {0}", i + 1);
            }
        }

        if (currentFuel < 3) {
            StartFuelTimer(3 - currentFuel);
        } else {
            fuelTimer.gameObject.SetActive(false);
        }
    }

    void StartMainTimer() {
        StartCoroutine(RunMainTimer());
    }

    void StartFuelTimer(int segments) {
        if (fuelTimerRunning) {
            return;
        }

        fuelTimer.gameObject.SetActive(true);
        StartCoroutine(RunFuelTimer(segments));
    }

    IEnumerator RunMainTimer() {
        int seconds = 60 * 5;
        while (seconds >= 0) {
            int minutes = Mathf.FloorToInt(seconds / 60f);
            int sec = seconds % 60;
            string padding = "";
            if (sec < 10) {
                padding = "0";
            }
            mainTimer.text = string.Format("{0}:{1}{2}", minutes, padding, sec);
            yield return new WaitForSeconds(1.0f);
            seconds -= 1;
        };
    }

    IEnumerator RunFuelTimer(int segments) {
        int seconds = 20 * segments;
        fuelTimerRunning = true;
        while (seconds >= 0) {
            int minutes = Mathf.FloorToInt(seconds / 60f);
            int sec = seconds % 60;
            string padding = "";
            if (sec < 10) {
                padding = "0";
            }
            fuelTimer.text = string.Format("{0}:{1}{2}", minutes, padding, seconds);
            yield return new WaitForSeconds(1.0f);
            seconds -= 1;
            if (seconds % 20 == 0) {
                GameManager.Instance.UpdateFuel(GameManager.Instance.fuelAvailable.Value + 1);
            }
        };

        fuelTimer.gameObject.SetActive(false);
    }
}
