using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{
    public Transform buttons;
    public Health player;

    private void Start() {
        Button resumeButton = buttons.GetChild(0).GetComponent<Button>();
        resumeButton.onClick.AddListener(ResumeOnClick);
        AddPointerEnterListener(resumeButton);
        Button retryButton = buttons.GetChild(1).GetComponent<Button>();
        retryButton.onClick.AddListener(RetryOnClick);
        AddPointerEnterListener(retryButton);
        Button respawnButton = buttons.GetChild(2).GetComponent<Button>();
        respawnButton.onClick.AddListener(RespawnOnClick);
        AddPointerEnterListener(respawnButton);
        Button quitButton = buttons.GetChild(3).GetComponent<Button>();
        quitButton.onClick.AddListener(QuitOnClick);
        AddPointerEnterListener(quitButton);
    }

    public void Update()
    {
        if (Input.GetButtonDown("Menu")) {
            if (!IsEnabled()) {
                EnterMenu();
                SetSelectedButton(buttons.GetChild(0).GetComponent<Button>());
            } else {
                ExitMenu();
            }
        }
    }

    public bool IsEnabled() {
        return buttons.gameObject.activeSelf;
    }

    // todo move these to the buttons themselves, with parent containing OnClick()?
    private void ResumeOnClick() {
        ExitMenu();
    }

    private void RetryOnClick() {
        player.Retry(); // todo this allows player to retry just before they're going to get hit, maybe should cause player to take 1 damage... I've removed it for now
        ExitMenu();
    }

    private void RespawnOnClick() {
        player.Respawn();
        ExitMenu();
    }

    private void QuitOnClick() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void EnterMenu() {
        buttons.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    private void ExitMenu() {
        buttons.gameObject.SetActive(false);
        Time.timeScale = 1; 
    }

    private void SetSelectedButton(Button button) {
        DeselectCurrentButton();
        button.Select();
        button.OnSelect(new BaseEventData(EventSystem.current));
    }

    private void DeselectCurrentButton() {
        GameObject currentButton = EventSystem.current.currentSelectedGameObject;
        if (currentButton != null) {
            currentButton.GetComponent<Button>()?.OnDeselect(new BaseEventData(EventSystem.current));
        }
    }

    private void AddPointerEnterListener(Button button) {
        EventTrigger eventTrigger = button.gameObject.GetComponent<EventTrigger>();
        if (eventTrigger == null) {
            eventTrigger = button.gameObject.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry = new EventTrigger.Entry {
            eventID = EventTriggerType.PointerEnter
        };
        entry.callback.AddListener(_ => SetSelectedButton(button));
        eventTrigger.triggers.Add(entry);
    }
}
