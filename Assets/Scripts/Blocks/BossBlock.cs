using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossBlock : MonoBehaviour, ITakeDamage
{
    [SerializeField] private int hp;
    [SerializeField] private int currentlyHp;
    [SerializeField] private TextMeshProUGUI hp_text;
    [SerializeField] private Collider hp_collider;
    [SerializeField] private List<Transform> envBlocks = new();
    [SerializeField] private List<Vector3> envBlockPos = new();

    private void OnEnable()
    {
        hp_collider.enabled = true;
        currentlyHp = hp;
        hp_text.SetText(currentlyHp.ToString());
        for (int i = 0; i < envBlocks.Count; i++)
        {
            envBlocks[i].position = envBlockPos[i];    
        }
    }

    public void TakeDamage(int amount)
    {
        SoundManager.Instance.Play("Damage");
        currentlyHp -= amount;
        hp_text.SetText(currentlyHp.ToString());
        if (currentlyHp <= 0)
        {
            hp_collider.enabled = false;
            EventBroker.Publish(Events.ON_LEVEL_SUCCESS);
            return;
        }
    }

#if UNITY_EDITOR
    [ContextMenu("TakeEnvBlocksPos")]
    public void TakeEnvBlocksPos()
    {
        foreach (Transform t in envBlocks)
        {
            envBlockPos.Add(t.position);
        }
    }
#endif
}
