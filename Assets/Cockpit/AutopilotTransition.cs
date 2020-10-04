using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class AutopilotTransition : MonoBehaviour
{
    void OnEnable() {
        Animate();
    }

    void Animate() {
        Observable.Timer(System.TimeSpan.FromSeconds(1.5f))
            .Subscribe((x) => {
                gameObject.SetActive(false);
            });
    }
}
