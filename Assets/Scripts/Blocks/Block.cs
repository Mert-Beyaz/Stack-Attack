using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Block : MonoBehaviour, ITakeDamage
{
    [SerializeField] private int hp;
    [SerializeField] private int currentlyHp;
    [SerializeField] private TextMeshProUGUI hp_text;
    [SerializeField] private List<GameObject> objList = new();

    private int _objCounter = 0;

    private void OnEnable()
    {
        foreach (var obj in objList)
        {
            obj.SetActive(true);
        }
        currentlyHp = hp;
        hp_text.SetText(currentlyHp.ToString());
        _objCounter = 0;
    }

    public void TakeDamage(int amount)
    {
        SoundManager.Instance.Play("Damage");
        currentlyHp -= amount;
        hp_text.SetText(currentlyHp.ToString());
        if (currentlyHp <= 0)
        {
            PoolManager.Instance.ReturnObject(gameObject);
            return;
        }
        objList[_objCounter].SetActive(false);
        _objCounter++;
    }
}
