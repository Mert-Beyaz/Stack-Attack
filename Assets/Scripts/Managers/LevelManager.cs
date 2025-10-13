using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoSingleton<LevelManager>
{
    [SerializeField] private List<GameObject> levelHolderList = new();
    [SerializeField] private int currentLevel;

    public void LoadLevel()
    {
        currentLevel = SaveSystem.LoadLevel();
        if (levelHolderList.Count <= currentLevel) currentLevel %= levelHolderList.Count;
        levelHolderList[currentLevel].SetActive(true);
    }

    public void UnloadLevel()
    {
        PoolManager.Instance.ReturnAllActiveObjects();
        levelHolderList[currentLevel].SetActive(false);
    }
}
