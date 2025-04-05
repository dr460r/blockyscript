using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public LevelLoader levelLoader;
    public GameObject pnlMain;
    public GameObject pnlStartGame;
    public GameObject pnlMaps;
    public GameObject pnlInfo;
    public GameObject pnlInfoSideBar;
    public GameObject[] infoViews;
    public Button[] infoButtons;

    void Start()
    {
        ChooseInfoCategory(infoButtons[0]);
    }

    public void OpenMapSelectionMenu() 
    {
        levelLoader.Reset();
        pnlMain.SetActive(false);
        pnlMaps.SetActive(true);
        pnlStartGame.SetActive(false);
    }

    public void ChooseMap(string name)
    {
        levelLoader.SetMapName(name);
        pnlStartGame.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        pnlMain.SetActive(true);
        pnlMaps.SetActive(false);
        pnlStartGame.SetActive(false);
        levelLoader.ResetAndLoadForHome();
    }

    public void StartGame() 
    {
        pnlMaps.SetActive(false);
        pnlStartGame.SetActive(false);
        pnlMain.SetActive(true);
        levelLoader.LoadMap();
    }

    public void QuitGame()
    {
        Application.Quit();
    }


    public void ChooseInfoCategory(Button btn)
    {
        for (int i = 0; i < infoButtons.Length; i++) {
            if (infoButtons[i] == btn) {
                infoViews[i].SetActive(true);
            }
            else {
                infoViews[i].SetActive(false);
            }
        }

        Color darkCol = pnlInfoSideBar.GetComponent<Image>().color;
        Color lightCol = pnlInfo.GetComponent<Image>().color;
        foreach (Button b in infoButtons) {
            b.targetGraphic.color = darkCol;
            b.interactable = true;
        }
        btn.targetGraphic.color = lightCol;
        btn.interactable = false;
    }

    public void OpenInfoMenu() 
    {
        levelLoader.Reset();
        pnlMain.SetActive(false);
        pnlInfo.SetActive(true);
    }

    public void CloseInfoMenu()
    {
        pnlInfo.SetActive(false);
        ReturnToMainMenu();
    }
}