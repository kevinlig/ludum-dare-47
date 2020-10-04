using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;

public class SRUserInterface : MonoBehaviour
{
    public GameObject srCamera;
    public RawImage reticle;
    public Texture standardReticleTexture;
    public Texture activeReticleTexture;
    public TextMeshProUGUI planetLabel;

    float playerLatitude;

    StarController activeStar;

    void Start() {
        SubscribeToData();
    }

    void SubscribeToData() {
        GameManager.Instance.activeStar
            .DistinctUntilChanged()
            .Subscribe((StarController _star) => {
                activeStar = _star;
                UpdatePlanet();
            });

        GameManager.Instance.latitude
            .Subscribe((float lat) => {
                playerLatitude = lat;
                UpdatePlanet();
            });
    }

    public void SetReticleState(bool reticleState) {
        Texture reticleImage = standardReticleTexture;
        if (reticleState) {
            reticleImage = activeReticleTexture;
        }
        reticle.texture = reticleImage;
    }

    void UpdatePlanet() {
        if (activeStar == null) {
            SetReticleState(false);
            planetLabel.text = "";
            return;
        }

        SetReticleState(true);

        StarData data = activeStar.data;
        string planetName = data.name.ToUpper();
        float declination = data.declination.Value;
        float altitude = 90 - Mathf.Abs(declination - playerLatitude);
        string displayDeclination = FormatDeclination(declination);
        string displayAltitude = FormatAltitude(altitude);
        string observedAltitude = FormatAltitude(90 - Vector3.Angle(srCamera.transform.position, activeStar.gameObject.transform.position));
        planetLabel.text = string.Format("<size=60><b>{0}</b></size>\n<font-weight=600>Declination:</font-weight> {1}\n<font-weight=600>Observed Altitude:</font-weight> {2}\n<font-weight=600>Corrected Altitude:</font-weight> {3}", planetName, displayDeclination, observedAltitude, displayAltitude);
    }

    string FormatDeclination(float declination) {
        float adjustedDeclination = declination;
        if (Mathf.Abs(declination) > 90) {
            // declination can't exceed 90
            adjustedDeclination = (180 - Mathf.Abs(declination % 180)) * (declination / Mathf.Abs(declination));
        }
        string prefix = "";
        if (adjustedDeclination > 0) {
            prefix = "+";
        }
        return string.Format("{0}{1}", prefix, FormatDegrees(adjustedDeclination));
    }

    string FormatAltitude(float altitude) {
        float adjustedAltitude = Mathf.Abs(altitude) % 180;
        if (adjustedAltitude > 90) {
            // can't exceed 90
            adjustedAltitude = adjustedAltitude - 90;
        }
        return FormatDegrees(adjustedAltitude);
    }

    string FormatDegrees(float degrees) {
        string formatted = "";
        int fullDeg = Mathf.FloorToInt(degrees);
        int minutes = Mathf.RoundToInt((degrees - fullDeg) * 60);
        if (minutes > 0) {
            formatted = string.Format("{0}°{1}'", fullDeg, minutes);
        } else {
            formatted = string.Format("{0}°", fullDeg);
        }
        return formatted;
    }
}
