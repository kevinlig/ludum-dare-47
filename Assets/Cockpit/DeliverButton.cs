using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class DeliverButton : MonoBehaviour
{
    DashButton buttonController;
    Material defaultMaterial;
    Color originalColor;
    float fullEmission;
    float lowEmission;
    int currentPos = 0;
    void Start()
    {
        buttonController = GetComponent<DashButton>();
        Renderer renderer = GetComponent<Renderer>();
        defaultMaterial = renderer.material;
        originalColor = defaultMaterial.GetColor("_EmissionColor");
        fullEmission = (originalColor.r + originalColor.g + originalColor.b) / 3f;
        lowEmission = fullEmission / 5f;

        GameManager.Instance.currentNavUnit
            .Subscribe(HandleNavUnit);
    }

    void HandleNavUnit(int pos) {
        currentPos = pos;
        bool isDeliverable = currentPos % 5f == 0;
        int dropboxNum = Mathf.FloorToInt(currentPos / 5f) + 1;

        float emissionAmount = fullEmission;
        string helperText = string.Format("DELIVER to Dropbox {0}", dropboxNum);

        if (!isDeliverable) {
            emissionAmount = lowEmission;
            helperText = "Cannot deliver here";
        }

        Color emissionColor = new Color(originalColor.r * emissionAmount, originalColor.g * emissionAmount, originalColor.b * emissionAmount);
        defaultMaterial.SetColor("_EmissionColor", emissionColor);
        buttonController.helperText = helperText;
    }

    public void HandleDelivery() {
        bool isDeliverable = currentPos % 5f == 0;
        int dropboxNum = Mathf.FloorToInt(currentPos / 5f) + 1;

        if (!isDeliverable) {
            return;
        }

        GameManager.Instance.DeliverTo(dropboxNum);
    }
}
