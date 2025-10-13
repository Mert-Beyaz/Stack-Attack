using DG.Tweening;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class BlockGroups : MonoBehaviour
{
    [SerializeField] private List<Transform> pointList = new();
    [SerializeField] private List<GameObject> objList = new();

    [Header("Group Type")]
    [SerializeField] private BlockGroupType groupType;

    [Header("Variables")]
    [SerializeField] private float verticalMovingValues;
    [SerializeField] private float horizontalMovingValues;
    [SerializeField] private float movingTime = 3;
    [SerializeField] private int movingDelay = 3;
    [SerializeField] private bool isMoving;

    private CancellationTokenSource _cts;

    private void OnEnable()
    {
        _cts = new CancellationTokenSource();
        _ = Init(_cts.Token);
    }

    private async Task Init(CancellationToken token)
    {
        try
        {
            await Task.Delay(2000, token);

            if (token.IsCancellationRequested) return;

            PutBlocks();

            if (!isMoving) return;

            await Task.Delay(movingDelay * 500);
            switch (groupType)
            {
                case BlockGroupType.Vertical:
                    transform.DOLocalMoveZ(verticalMovingValues, movingTime)
                        .SetEase(Ease.Linear)
                        .SetLoops(-1, LoopType.Yoyo);
                    break;

                case BlockGroupType.Horizontal:
                    transform.DOLocalMoveX(horizontalMovingValues, movingTime)
                        .SetEase(Ease.Linear)
                        .SetLoops(-1, LoopType.Yoyo);
                    break;

                case BlockGroupType.Circle:
                    foreach (var obj in objList)
                        _ = MoveObjectInCircle(obj, token);
                    break;
            }
        }
        catch (TaskCanceledException)
        {
            
        }
    }

    private void PutBlocks()
    {
        foreach (var item in pointList)
        {
            var block = PoolManager.Instance.GetObject(PoolType.Block);
            block.transform.position = item.position;
            block.transform.rotation = item.rotation;
            block.transform.SetParent(transform);
            objList.Add(block);
        }
    }

    private async Task MoveObjectInCircle(GameObject obj, CancellationToken token)
    {
        try
        {
            int index = objList.IndexOf(obj);
            int currentIndex = index;

            while (!token.IsCancellationRequested)
            {
                int nextIndex = (currentIndex + 1) % pointList.Count;
                Vector3 targetPos = pointList[nextIndex].position;

                await obj.transform.DOMove(targetPos, movingTime)
                    .SetEase(Ease.Linear)
                    .AsyncWaitForCompletion();

                currentIndex = nextIndex;
            }
        }
        catch (TaskCanceledException)
        {
           
        }
    }

    private void OnDisable()
    {
        _cts?.Cancel();
        objList.Clear();
        DOTween.KillAll();
    }
}

public enum BlockGroupType
{
    Vertical,
    Horizontal,
    Circle,
}
