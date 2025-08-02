using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event Action OnCollisionGameOver;
    public event Action OnGameFinished;
    public event Action OnFoodEaten;
    public event Action<int> OnScoreChanged;
    public int Score => _score;

    [SerializeField] private GameObject _snakePb;
    [SerializeField] private FoodSpawner _foodSpawner;
    [SerializeField] private Vector2Int _gridSize = new(20, 20);
    [SerializeField] private Vector3 _snakeStartPos = new(5, 5);

    private Snake _snakeInst;
    private int _score;

    public void ResetGame()
    {
        _score = 0;
        OnScoreChanged?.Invoke(_score);
        if (_snakeInst != null) Destroy(_snakeInst.gameObject);
        _snakeInst = Instantiate(_snakePb, _snakeStartPos, Quaternion.identity).GetComponent<Snake>();
        _foodSpawner.PlaceFood(_gridSize, _snakeInst.BodyPos);

    }

    public void Step(int action)
    {
        if (_snakeInst == null) return;
        Vector2Int currentHeading = _snakeInst.Heading;
        Vector2Int newDirection = currentHeading;
        switch (action)
        {
            case 0: break;
            case 1: newDirection = new Vector2Int(-currentHeading.y, currentHeading.x); break;
            case 2: newDirection = new Vector2Int(currentHeading.y, -currentHeading.x); break;
        }

        _snakeInst.SetDirection(newDirection);
        _snakeInst.StepForward();

        if (CheckCollision(_snakeInst.BodyPos[0]))
        {
            OnCollisionGameOver?.Invoke();
            Destroy(_snakeInst.gameObject);
            OnGameFinished?.Invoke();
            return;
        }
        if (!CheckFoodEaten()) return;
        OnFoodEaten?.Invoke();
        _score += 1;
        OnScoreChanged?.Invoke(_score);
        _snakeInst.ExtendBody();
        if (IsGridFull())
        {
            OnGameFinished?.Invoke();
            return;
        }
        _foodSpawner.PlaceFood(_gridSize, _snakeInst.BodyPos);
    }

    public float[] GetGameState()
    {
        Vector2Int head = _snakeInst.BodyPos[0];
        Vector2Int dir = _snakeInst.Heading;

        Vector2Int left = new Vector2Int(-dir.y, dir.x);
        Vector2Int right = new Vector2Int(dir.y, -dir.x);

        Vector2Int pointStraight = head + dir;
        Vector2Int pointLeft = head + left;
        Vector2Int pointRight = head + right;

        bool dangerStraight = CheckCollision(pointStraight);
        bool dangerRight = CheckCollision(pointRight);
        bool dangerLeft = CheckCollision(pointLeft);

        bool dirLeft = dir == Vector2Int.left;
        bool dirRight = dir == Vector2Int.right;
        bool dirUp = dir == Vector2Int.up;
        bool dirDown = dir == Vector2Int.down;

        Vector2Int food = _foodSpawner.FoodPos;
        bool foodLeft = food.x < head.x;
        bool foodRight = food.x > head.x;
        bool foodUp = food.y > head.y;
        bool foodDown = food.y < head.y;

        bool gameFinished = CheckCollision(head);

        float[] state = new float[]
        {
        dangerStraight ? 1f : 0f,
        dangerRight    ? 1f : 0f,
        dangerLeft     ? 1f : 0f,

        dirLeft  ? 1f : 0f,
        dirRight && !gameFinished ? 1f : 0f,
        dirUp    ? 1f : 0f,
        dirDown  ? 1f : 0f,

        foodLeft  ? 1f : 0f,
        foodRight ? 1f : 0f,
        foodUp    ? 1f : 0f,
        foodDown  ? 1f : 0f,
        };
        return state;
    }

    private bool CheckCollision(Vector2Int headPos)
    {
        bool wallHit = headPos.x < 0 || headPos.x >= _gridSize.x || headPos.y < 0 || headPos.y >= _gridSize.y;
        bool selfHit = _snakeInst.CheckSelfCollision(headPos);
        return wallHit || selfHit;
    }

    private bool CheckFoodEaten()
    {
        return _snakeInst.BodyPos[0] == _foodSpawner.FoodPos;
    }

    private bool IsGridFull()
    {
        return _snakeInst.BodyPos.Count == _gridSize.x * _gridSize.y;
    }


}
