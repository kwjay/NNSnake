using UnityEngine;

public class AgentInterface : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;

    private bool _ateFood = false;
    private bool _hitObstacle = false;
    private bool _gameFinished = false;
    private int _score = 0;
    public void Step(int action)
    {
        _gameManager.Step(action);
    }

    public void StartEpisode()
    {
        ResetFlags();
        _gameManager.ResetGame();
    }

    public Utility.GameStateMessage GetGameStateMessage()
    {

        float[] state = _gameManager.GetGameState();

        Utility.GameStateMessage msg = new()
        {
            state = state,
            done = _gameFinished,
            foodEaten = _ateFood,
            collision = _hitObstacle,
            score = _score
        };
        ResetFlags();
        return msg;
    }

    private void ResetFlags()
    {
        _ateFood = false;
        _hitObstacle = false;
        _gameFinished = false;
    }

    private void OnEnable()
    {
        _gameManager.OnFoodEaten += HandleFoodEaten;
        _gameManager.OnCollisionGameOver += HandleHitObstacle;
        _gameManager.OnGameFinished += HandleGameFinished;
    }

    private void OnDisable()
    {
        _gameManager.OnFoodEaten -= HandleFoodEaten;
        _gameManager.OnCollisionGameOver -= HandleHitObstacle;
        _gameManager.OnGameFinished -= HandleGameFinished;
    }

    private void HandleFoodEaten()
    {
        _ateFood = true;
        _score = _gameManager.Score;
    }
    private void HandleHitObstacle() => _hitObstacle = true;
    private void HandleGameFinished()
    {
        _gameFinished = true;
        _score = _gameManager.Score;
        _gameManager.ResetGame();
    }
}
