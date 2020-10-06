using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject introPanel;

    public void OnClickIntro() {
        menuPanel.SetActive(false);
        introPanel.SetActive(true);
    }

    public void OnClickPlay() {
        SceneManager.LoadScene("MainScene");
    }

    public void OnClickLicense() {

    }

    public void CloseIntro() {
        menuPanel.SetActive(true);
        introPanel.SetActive(false);
    }
}
