using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CockpitCamera : MonoBehaviour
{
    public float speed = 4f;
    public bool isLocked = true;
    public TextMeshProUGUI helperLabel;
    Vector2 cameraRotation = Vector2.zero;
    private Camera cockpitCamera;
    private string lastButtonType = "";
    private bool blockClicks = false;
    void Start() {
        Cursor.lockState = CursorLockMode.Confined;
        cockpitCamera = gameObject.GetComponent<Camera>();
    }

    void Update()
    {
        if (isLocked) {
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

        if (!isLocked && Input.GetMouseButtonDown(0)) {
            isLocked = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
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
        if (lastButtonType.Equals("forward")) {
            GameManager.Instance.MoveByOne(1);
        } else if (lastButtonType.Equals("reverse")) {
            GameManager.Instance.MoveByOne(-1);
        }
    }

    IEnumerator DebounceClicks() {
        yield return new WaitForSeconds(0.8f);
        blockClicks = false;
    }
}
