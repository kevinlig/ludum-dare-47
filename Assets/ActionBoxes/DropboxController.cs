using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;
using Pixelplacement;

public class DropboxController : MonoBehaviour
{
    public TextMeshPro frontLabel;
    public TextMeshPro backLabel;

    public GameObject nextBox = null;
    public GameObject prevBox = null;
    public int boxOffset = 0;
    public int boxNumber = 1;
    private int prevNavUnit = 0;
    private Vector3 originalPosition;
    private Vector3 nextPos;
    private Vector3 prevPos;
    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        if (nextBox != null) {
            nextPos = nextBox.transform.position;
        }
        if (prevBox != null) {
            prevPos = prevBox.transform.position;
        }
        UpdateNumber();
        SubscribeToData();
    }

    void SubscribeToData() {
        GameManager.Instance.currentNavUnit
            .Subscribe((int navUnit) => {
                if (navUnit % 5 == 0 || Mathf.Abs(navUnit - prevNavUnit) > 1) {
                    boxNumber = Mathf.FloorToInt((navUnit + (boxOffset * 5)) / 5) + 1;
                }
                AnimateToNavUnit(prevNavUnit, navUnit);
            });
    }

    void AnimateToNavUnit(int prev, int next) {
        int change = next - prev;
        prevNavUnit = next;
        if (Mathf.Abs(change) != 1) {
            // don't animate
            transform.position = originalPosition;
            UpdateNumber();
            return;
        }

        Vector3 animPos;
        float lerpFraction = (next % 5f) / 5f;
        if (lerpFraction == 0) {
            lerpFraction = 1f;
        }

        if (change > 0 && nextBox != null) {
            // moving forward
            animPos = Vector3.Lerp(originalPosition, nextPos, lerpFraction);
            Tween.Position(transform, animPos, 0.75f, 0, null, Tween.LoopType.None, null, FinishAnimation);
        }
        else if (change < 0 && prevBox != null) {
            // moving backward
            animPos = Vector3.Lerp(prevPos, originalPosition, lerpFraction);
            Tween.Position(transform, animPos, 0.75f, 0, null, Tween.LoopType.None, null, FinishAnimation);
        }
    }

    void FinishAnimation() {
        if ((prevNavUnit % 5f) == 0) {
            // restore to original position
            transform.position = originalPosition;
        }

        UpdateNumber();
    }

    void UpdateNumber() {
        int navUnit = GameManager.Instance.currentNavUnit.Value;
        frontLabel.text = string.Format("{0}", boxNumber);
        backLabel.text = string.Format("{0}", boxNumber);
    }
}
