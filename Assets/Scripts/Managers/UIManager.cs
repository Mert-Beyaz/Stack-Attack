using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using System;
using UnityEngine.Events;
using Solo.MOST_IN_ONE;

public class UIManager : MonoSingleton<UIManager>
{
    [Header("Panels")]
    [SerializeField] private GameObject startPanel = null;
    [SerializeField] private GameObject inGamePanel = null;
    [SerializeField] private GameObject winPanel = null;
    [SerializeField] private GameObject losePanel = null;

    [Space, Header("Start Panel")]
    [SerializeField] private TextMeshProUGUI levelText = null;
    [SerializeField] private Button startButton = null;

    [Space, Header("Buttons")]
    [SerializeField] private Button holdToStartButton = null;
    [SerializeField] private Button nextLevelButton = null;
    [SerializeField] private Button restartButton = null;

    [Header("Player UI's")]
    [SerializeField] private List<Image> hpImgList = new();
    [SerializeField] private Image levelProgressBar = null;
    private Tween levelProgressBarTween = null;

    [Header("Transition")]
    [SerializeField] private Image transition;
    [SerializeField][Range(0f, 5f)] int waitSecond;

    void Start()
    {
        Subscribe();

        // Buttons Assign
        startButton.onClick.AddListener(() => OnClickStartButton());
        holdToStartButton.onClick.AddListener(() => OnClickHoldToStartButton());
        nextLevelButton.onClick.AddListener(() => OnClickNextLevelButton());
        restartButton.onClick.AddListener(() => OnClickRestartButton());

        transition.gameObject.SetActive(true);
        OnMainMenu();
    }

    private void Subscribe()
    {
        EventBroker.Subscribe(Events.ON_LEVEL_START, OnPlay);
        EventBroker.Subscribe(Events.ON_LEVEL_FAIL, OnFail);
        EventBroker.Subscribe(Events.ON_LEVEL_SUCCESS, OnSuccess);
        EventBroker.Subscribe(Events.ON_LEVEL_RESET, OnMainMenu);
        EventBroker.Subscribe(Events.ON_NEXT_LEVEL, OnMainMenu);
        EventBroker.Subscribe<int>(Events.SET_HP, SetHP);
        EventBroker.Subscribe<float>(Events.ON_PLAY_PROGRESS_BAR, OnPlayProgressBar);
    }

    private void OnPlay()
    {
        inGamePanel.SetActive(true);
        startPanel.SetActive(false);
        winPanel.SetActive(false);
        losePanel.SetActive(false);
    }   

    private void OnMainMenu()
    {
        startPanel.SetActive(true);
        inGamePanel.SetActive(false);
        winPanel.SetActive(false);
        losePanel.SetActive(false);
    }
    
    private void OnFail()
    {
        Most_HapticFeedback.Generate(Most_HapticFeedback.HapticTypes.Failure);
        SoundManager.Instance.Play("Fail");
        inGamePanel.SetActive(false);
        startPanel.SetActive(false);
        winPanel.SetActive(false);
        losePanel.SetActive(true);
    }

    private void OnSuccess()
    {
        Most_HapticFeedback.Generate(Most_HapticFeedback.HapticTypes.Success);
        SoundManager.Instance.Play("Success");
        inGamePanel.SetActive(false);
        startPanel.SetActive(false);
        winPanel.SetActive(true);
        losePanel.SetActive(false);
    }    
    
    private void OnLevelReset()
    {
        SetTransition(Transition.InOutro, 2, () => {
            EventBroker.Publish(Events.ON_LEVEL_RESET);
        });
    }   
    
    private void OnNextLevel()
    {
        SetTransition(Transition.InOutro, 2, () => {
            EventBroker.Publish(Events.ON_NEXT_LEVEL);
        });
    }

    private void OnReturnMainMenu(bool isSuccess)
    {
        Time.timeScale = 1;
        levelProgressBarTween?.Kill();
        levelProgressBar.fillAmount = 0;

        if (isSuccess) OnNextLevel();
        else OnLevelReset();

    }

    #region Buttons

    private void OnClickStartButton()
    {
        Most_HapticFeedback.Generate(Most_HapticFeedback.HapticTypes.Selection);
        holdToStartButton.gameObject.SetActive(true);
        SetTransition(Transition.InOutro, 2, () => {
            EventBroker.Publish(Events.ON_LEVEL_START);
        });
    }

    private void OnClickHoldToStartButton()
    {
        Most_HapticFeedback.Generate(Most_HapticFeedback.HapticTypes.Selection);
        holdToStartButton.gameObject.SetActive(false);
        EventBroker.Publish(Events.ON_FIRST_TOUCH);
    }

    private void OnClickNextLevelButton()
    {
        Most_HapticFeedback.Generate(Most_HapticFeedback.HapticTypes.Selection);
        OnReturnMainMenu(true);
    }

    private void OnClickRestartButton()
    {
        Most_HapticFeedback.Generate(Most_HapticFeedback.HapticTypes.Selection);
        OnReturnMainMenu(false);
    }

    #endregion

    private void SetHP(int hpAmount)
    {
        int hpCounter = 0;
        foreach (var item in hpImgList)
        {
            if (hpCounter < hpAmount) item.gameObject.SetActive(true);
            else item.gameObject.SetActive(false);
            hpCounter++;
        }
    }

    public void SetLevelText()
    {
        levelText.SetText("LEVEL " + (SaveSystem.LoadLevel() + 1).ToString());
    }

    private void OnPlayProgressBar(float time)
    {
        levelProgressBarTween = levelProgressBar.DOFillAmount(1, time);
    }

    #region Transition
    public void SetTransition(Transition transitionEnum, int duration, UnityAction action = null)
    {
        _ = TransitionAnim(transitionEnum, duration, action);
    }

    private async Task TransitionAnim(Transition transitionType, int duration, UnityAction action = null)
    {
        switch (transitionType)
        {
            case Transition.Intro:

                transition.raycastTarget = true;
                transition.DOFade(0f, duration);
                await Task.Delay(duration * 1000);
                transition.raycastTarget = false;
                break;

            case Transition.InOutro:
                transition.raycastTarget = true;
                transition.DOFade(1f, duration / 2f).OnComplete(async () =>
                {
                    action?.Invoke();
                    await Task.Delay(waitSecond * 1000);
                    transition.DOFade(0f, duration / 2f).OnComplete(() =>
                    {
                        transition.raycastTarget = false;

                    });
                });
                break; 

            case Transition.Outro:
                transition.raycastTarget = false;
                transition.DOFade(0f, duration);
                await Task.Delay(duration * 1000);
                break;
        }
    }

    #endregion

    private void Unsubscribe()
    {
        EventBroker.UnSubscribe(Events.ON_LEVEL_START, OnPlay);
        EventBroker.UnSubscribe(Events.ON_LEVEL_FAIL, OnFail);
        EventBroker.UnSubscribe(Events.ON_LEVEL_SUCCESS, OnSuccess);
        EventBroker.UnSubscribe(Events.ON_LEVEL_RESET, OnMainMenu);
        EventBroker.UnSubscribe(Events.ON_NEXT_LEVEL, OnMainMenu);
        EventBroker.UnSubscribe<int>(Events.SET_HP, SetHP);
        EventBroker.UnSubscribe<float>(Events.ON_PLAY_PROGRESS_BAR, OnPlayProgressBar);
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }
}

public enum Transition
{
    Intro,
    Outro,
    InOutro,
}
