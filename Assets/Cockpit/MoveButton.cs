using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class MoveButton : MonoBehaviour
{
    Material defaultMaterial;
    Color originalColor;
    float fullEmission;
    float lowEmission;
    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        defaultMaterial = renderer.material;
        originalColor = defaultMaterial.GetColor("_EmissionColor");
        fullEmission = (originalColor.r + originalColor.g + originalColor.b) / 3f;
        lowEmission = fullEmission / 5f;

        GameManager.Instance.fuelAvailable
            .Subscribe(HandleButtonLight);
    }

    void HandleButtonLight(int fuelAmount) {
        float emissionAmount = fullEmission;

        if (fuelAmount <= 0) {
            emissionAmount = lowEmission;
        }

        Color emissionColor = new Color(originalColor.r * emissionAmount, originalColor.g * emissionAmount, originalColor.b * emissionAmount);
        defaultMaterial.SetColor("_EmissionColor", emissionColor);
    }
}
