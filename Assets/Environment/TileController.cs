using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Pixelplacement;

public class TileController : MonoBehaviour
{
    Vector3 originalPosition;
    int prevNavUnit = 0;

    void Start()
    {
        originalPosition = transform.position;
        SubscribeToData();
    }

    void SubscribeToData() {
        GameManager.Instance.currentNavUnit
            .DistinctUntilChanged()
            .Subscribe((int navUnit) => {
                int change = navUnit - prevNavUnit;
                if (Mathf.Abs(change) == 1) {
                    AnimateTiles(change);
                }

                prevNavUnit = navUnit;
            });
    }


    void AnimateTiles(int change) {
        Vector3 newPos = originalPosition + new Vector3(0, 0, -2 * change);
        Tween.Position(transform, newPos, 0.7f, 0, null, Tween.LoopType.None, null, () => {
            transform.position = originalPosition;
        });
    }
}
