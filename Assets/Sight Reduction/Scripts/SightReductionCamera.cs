using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightReductionCamera : MonoBehaviour
{
    public float speed = 4f;
    public bool isLocked = true;
    Vector2 cameraRotation = Vector2.zero;
    private Camera srCamera;
    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        srCamera = gameObject.GetComponent<Camera>();
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
        }

        if (!isLocked && Input.GetMouseButtonDown(0)) {
            isLocked = true;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (Input.GetKey(KeyCode.X)) {
            GameManager.Instance.currentView
                .SetValueAndForceNotify("cockpit");
        }
    }

    void OnEnable() {
        isLocked = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Look() {
        cameraRotation.y += Input.GetAxis("Mouse X") * speed;
        cameraRotation.x += -Input.GetAxis("Mouse Y") * speed;
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, -90f, 100f);
        // cameraRotation.y = Mathf.Clamp(cameraRotation.y, -120, 120f);
        transform.eulerAngles = new Vector3(cameraRotation.x, cameraRotation.y, 0);
    }

    void CheckHits() {
        RaycastHit hit;

        if (Physics.Raycast(srCamera.transform.position, srCamera.transform.forward, out hit, Mathf.Infinity, 1 << 8)) {
            GameObject starObj = hit.collider.transform.gameObject;
            StarController controller = starObj.GetComponent<StarController>();
            GameManager.Instance.SetActiveStar(controller);
        } else {
            GameManager.Instance.SetActiveStar(null);
        }
    }
}
