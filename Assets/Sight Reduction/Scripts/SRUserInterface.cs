using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;

public class SRUserInterface : MonoBehaviour
{
    public RawImage reticle;
    public Texture standardReticleTexture;
    public Texture activeReticleTexture;
    public TextMeshProUGUI planetLabel;

    StarData activeStar;

    void Start() {
        SubscribeToData();
    }

    void SubscribeToData() {
        GameManager.Instance.activeStar
            .DistinctUntilChanged()
            .Subscribe((StarData _star) => {
                activeStar = _star;
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
        planetLabel.text = string.Format("<b>{0}</b>", activeStar.name.ToUpper());
    }
}
