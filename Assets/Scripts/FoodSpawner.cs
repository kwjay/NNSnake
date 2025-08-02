using System.Collections.Generic;
using System;
using UnityEngine;


public class FoodSpawner : MonoBehaviour
{
    public Vector2Int FoodPos => _foodPos;

    [SerializeField] private GameObject _foodPb;

    private GameObject _foodInst;
    private Vector2Int _foodPos;

    public void PlaceFood(Vector2Int gridSize, List<Vector2Int> blockedPos)
    {
        if (_foodInst == null) Initialize();
        _foodPos = GetAvailablePosition(gridSize, blockedPos);
        _foodInst.transform.position = new Vector3(_foodPos.x, _foodPos.y, 0);
    }

    private Vector2Int GetAvailablePosition(Vector2Int gridSize, List<Vector2Int> blockedPos)
    {
        List<Vector2Int> availablePos = new();

        for (int x = 1; x < gridSize.x; x++)
        {
            for (int y = 1; y < gridSize.y; y++)
            {
                Vector2Int pos = new(x, y);

                if (blockedPos.Contains(pos))
                    continue;
                availablePos.Add(pos);
            }
        }
        return availablePos[UnityEngine.Random.Range(0, availablePos.Count)];
    }

    private void Initialize()
    {
        _foodInst = Instantiate(_foodPb, Vector3.zero, Quaternion.identity, transform);
    }
}
