using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
public class TutorialController : MonoBehaviour
{
    public MenuController menuController;
    public GameObject[] pages;
    public IntReactiveProperty currentPage = new IntReactiveProperty(0);

    void Start() {
        currentPage
            .Subscribe((pageIdx) => {
                int idx = 0;
                foreach(GameObject page in pages) {
                    bool active = false;
                    if (idx == pageIdx) {
                        active = true;
                    }
                    page.SetActive(active);

                    idx++;
                }
            });
            
    }

    void OnEnable() {
        currentPage.SetValueAndForceNotify(0);
    }


    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (currentPage.Value + 1 >= pages.Length) {
                menuController.CloseIntro();
            } else {
                currentPage.SetValueAndForceNotify(currentPage.Value + 1);
            }
        }
    }


}
