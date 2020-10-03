using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class StarController : MonoBehaviour
{
    public StarData data;
    private float latitude = 0;
    private float longitude = 0;
    private int distance = 50;
    public GameObject sphere;
    public SphereCollider sphereCollider;
    // Start is called before the first frame update
    void Start()
    {
        SubscribeToData();
    }

    void SubscribeToData() {
        GameManager.Instance.playerLocation
            .Distinct()
            .Subscribe((Vector2 pos) => {
                latitude = pos.y;
                longitude = pos.x;
                PositionStar();
            });

        data.declination
            .Subscribe((float _)  => {
                PositionStar();
            });

        data.hourAngle
            .Subscribe((float _) => {
                PositionStar();
            });
    }

    void PositionStar() {
        // calculate how the star's declination appears to the player from their current planetary position
        float altitude = 90 - (data.declination.Value - latitude);
        if (altitude < 0 || altitude > 180) {
            HideStar();
        } else {
            ShowStar();
        }

        transform.position = Vector3.zero + (Quaternion.Euler(altitude * -1, 0, 0) * Quaternion.Euler(0, data.hourAngle.Value * -1, 0) * transform.forward * distance);
    }

    void HideStar() {
        sphere.SetActive(false);
        sphereCollider.enabled = false;
    }

    void ShowStar() {
        sphere.SetActive(true);
        sphereCollider.enabled = true;
    }

}
