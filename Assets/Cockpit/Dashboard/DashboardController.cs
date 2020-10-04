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
    
    void Start() {
        GameManager.Instance.deliveryDestinations
            .Subscribe((Dictionary<int, int> deliveries) => {
                HandleDeliveries(deliveries);
            });

        GameManager.Instance.fuelAvailable
            .Subscribe(HandleFuel);

        GameManager.Instance.timeRemaining
            .Subscribe(HandleGameTimer);

        GameManager.Instance.fuelTimeRemaining
            .Subscribe(HandleFuelTimer);
    }

    void HandleDeliveries(Dictionary<int, int> deliveries) {
        List<int> destinations = deliveries.Keys.ToList().OrderBy((o) => o).ToList();
        for (int i = 0; i < 5; i++) {
            int deliveredValue = 0;
            int boxNumber = 0;
            if (destinations.Count > i) {
                boxNumber = destinations[i];
                deliveredValue = deliveries[boxNumber];
            }
            if (boxNumber == 0 || deliveredValue == 0) {
                deliveryBoxImages[i].texture = inactiveBox;
                if (boxNumber == 0) {
                    deliveryBoxLabels[i].text = "";
                } else {
                    deliveryBoxLabels[i].text = string.Format("{0}", boxNumber);
                }
            } else {
                deliveryBoxImages[i].texture = blueBox;
                deliveryBoxLabels[i].text = string.Format("{0}", boxNumber);
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
    }


    void HandleGameTimer(long seconds) {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        long sec = seconds % 60;
        string padding = "";
        if (sec < 10) {
            padding = "0";
        }
        mainTimer.text = string.Format("{0}:{1}{2}", minutes, padding, sec);
    }

    void HandleFuelTimer(long seconds) {
        if (seconds == 0) {
            fuelTimer.gameObject.SetActive(false);
        } else {
            fuelTimer.gameObject.SetActive(true);
        }
        int minutes = Mathf.FloorToInt(seconds / 60f);
        long sec = seconds % 60;
        string padding = "";
        if (sec < 10) {
            padding = "0";
        }
        fuelTimer.text = string.Format("{0}:{1}{2}", minutes, padding, sec);
    }
}
