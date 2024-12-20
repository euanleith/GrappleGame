using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{
    public new bool enabled = false;
    public Transform buttons;
    public Health player;

    void Start() {
        Button resumeButton = buttons.GetChild(0).GetComponent<Button>();
        resumeButton.onClick.AddListener(ResumeOnClick);
        Button retryButton = buttons.GetChild(1).GetComponent<Button>();
        retryButton.onClick.AddListener(RetryOnClick);
        Button respawnButton = buttons.GetChild(2).GetComponent<Button>();
        respawnButton.onClick.AddListener(RespawnOnClick);
        Button quitButton = buttons.GetChild(3).GetComponent<Button>();
        quitButton.onClick.AddListener(QuitOnClick);
    }

    public void ManualUpdate()
    {
        enabled = buttons.gameObject.activeSelf;
        if (Input.GetButtonDown("Menu")) {
            if (!enabled) {
                EnterMenu();
                SetDefaultButton(buttons.GetChild(0).GetComponent<Button>());
            } else {
                ExitMenu();
            }
        }
    }

    // todo move these to the buttons themselves, with parent containing OnClick()?
    void ResumeOnClick() {
        ExitMenu();
    }

    void RetryOnClick() {
        player.Retry(); // todo this allows player to retry just before they're going to get hit, maybe should cause player to take 1 damage... I've removed it for now
        ExitMenu();
    }

    void RespawnOnClick() {
        player.Respawn();
        ExitMenu();
    }

    void QuitOnClick() {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    void EnterMenu() {
        buttons.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    void ExitMenu() {
        buttons.gameObject.SetActive(false);
        Time.timeScale = 1; 
    }

    void SetDefaultButton(Button button) {
        button.Select();
        button.OnSelect(new BaseEventData(EventSystem.current));
    }
}
