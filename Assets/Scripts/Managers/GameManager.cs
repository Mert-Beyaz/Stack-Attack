using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    private void Start()
    {
        Subscribe();
        _ = Init();
    }

    private void Subscribe()
    {
        EventBroker.Subscribe(Events.ON_LEVEL_FAIL, OnFail);
        EventBroker.Subscribe(Events.ON_LEVEL_SUCCESS, OnSuccess);
        EventBroker.Subscribe(Events.ON_LEVEL_RESET, OnLevelReset);
        EventBroker.Subscribe(Events.ON_NEXT_LEVEL, OnNextLevel);
    }

    private async Task Init()
    {
        await Task.Delay(2000);
        LevelManager.Instance.LoadLevel();
        UIManager.Instance.SetLevelText();
        UIManager.Instance.SetTransition(Transition.Intro, 1);
    }

    private void OnFail()
    {
        Time.timeScale = 0;
    }

    private void OnSuccess()
    {
        Time.timeScale = 0;
        SaveSystem.NextLevel();
    }

    private void OnLevelReset()
    {
        LevelManager.Instance.UnloadLevel();
        LevelManager.Instance.LoadLevel();
    }  
    
    private void OnNextLevel()
    {
        LevelManager.Instance.UnloadLevel();
        LevelManager.Instance.LoadLevel();
        UIManager.Instance.SetLevelText();
    }

    private void Unsubscribe()
    {
        EventBroker.UnSubscribe(Events.ON_LEVEL_FAIL, OnFail);
        EventBroker.UnSubscribe(Events.ON_LEVEL_SUCCESS, OnSuccess);
        EventBroker.UnSubscribe(Events.ON_LEVEL_RESET, OnLevelReset);
        EventBroker.UnSubscribe(Events.ON_NEXT_LEVEL, OnNextLevel);
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }
}