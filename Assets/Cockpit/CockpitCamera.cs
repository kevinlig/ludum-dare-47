using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CockpitCamera : MonoBehaviour
{
    public float speed = 4f;
    public bool isLocked = true;
    public TextMeshProUGUI helperLabel;
    public DeliverButton deliverButton;
    public GameObject autopilotHud;
    Vector2 cameraRotation = Vector2.zero;
    private Camera cockpitCamera;
    private string lastButtonType = "";
    private bool blockClicks = false;
    private bool autopilotOpen = false;
    void Start() {
        Cursor.lockState = CursorLockMode.Confined;
        cockpitCamera = gameObject.GetComponent<Camera>();
    }

    void Update()
    {
        if (isLocked && !autopilotOpen) {
            Look();
            CheckHits();
            if (Input.GetKey(KeyCode.Escape)) {
                isLocked = false;
                Cursor.lockState = CursorLockMode.None;
            }

            if(lastButtonType != "" && !blockClicks && Input.GetMouseButtonDown(0)) {
                HandleClick();
            }
        }

        if (!isLocked && Input.GetMouseButtonDown(0) && !autopilotOpen) {
            isLocked = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    void OnEnable() {
        isLocked = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void OnDisable() {
        blockClicks = false;
    }

    void Look() {
        cameraRotation.y += Input.GetAxis("Mouse X") * speed;
        cameraRotation.x += -Input.GetAxis("Mouse Y") * speed;
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, -15f, 20f);
        cameraRotation.y = Mathf.Clamp(cameraRotation.y, -25f, 25f);
        transform.eulerAngles = new Vector3(cameraRotation.x, cameraRotation.y, 0);
    }

    void CheckHits() {
        RaycastHit hit;
        Ray ray = cockpitCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 12)) {
            GameObject buttonObj = hit.collider.transform.gameObject;
            DashButton buttonData = buttonObj.GetComponent<DashButton>();
            lastButtonType = buttonData.type;
            helperLabel.text = buttonData.helperText;
        } else if (lastButtonType != "") {
            helperLabel.text = "";
            lastButtonType = "";
        }
    }

    void HandleClick() {
        blockClicks = true;
        StartCoroutine(DebounceClicks());

        switch (lastButtonType) {
            case "forward":
                HandleMove();
                break;

            case "reverse":
                GameManager.Instance.MoveByOne(-1);
                break;
            
            case "sextant":
                GameManager.Instance.currentView.SetValueAndForceNotify("sightReduction");
                break;

            case "deliver":
                deliverButton.HandleDelivery();
                break;

            case "autopilot":
                OpenAutopilot();
                break;
        }
    }

    IEnumerator DebounceClicks() {
        yield return new WaitForSeconds(0.8f);
        blockClicks = false;
    }

    void OpenAutopilot() {
        autopilotOpen = true;
        isLocked = false;
        Cursor.lockState = CursorLockMode.None;

        autopilotHud.SetActive(true);
    }
    public void CloseAutopilot() {
        autopilotHud.SetActive(false);

        autopilotOpen = false;
        isLocked = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void HandleMove() {
        float fuel = GameManager.Instance.fuelAvailable.Value;
        if (fuel <= 0) {
            GlobalUI.Instance.SetAlert("[ERROR]: Out of fuel! Wait to recharge before moving.");
            return;
        }
        GameManager.Instance.MoveByOne(1);
    }
}
