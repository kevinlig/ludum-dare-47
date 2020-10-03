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
    }

    void Look() {
        cameraRotation.y += Input.GetAxis("Mouse X") * speed;
        cameraRotation.x += -Input.GetAxis("Mouse Y") * speed;
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, -90f, 90f);
        cameraRotation.y = Mathf.Clamp(cameraRotation.y, -90, 90f);
        transform.eulerAngles = new Vector3(cameraRotation.x, cameraRotation.y, 0);
    }

    void CheckHits() {
        RaycastHit hit;
        Ray ray = srCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 8)) {
            GameObject starObj = hit.collider.transform.gameObject;
            StarController controller = starObj.GetComponent<StarController>();
            StarData data = controller.data;
            GameManager.Instance.SetActiveStar(data);
        } else {
            GameManager.Instance.SetActiveStar(null);
        }
    }
}
