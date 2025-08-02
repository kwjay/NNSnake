using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    private Label _scoreLabel;


    private void OnEnable()
    {
        var uiDoc = GetComponent<UIDocument>();
        var root = uiDoc.rootVisualElement;
        _scoreLabel = root.Q<Label>("scoreLabel");
        _gameManager.OnScoreChanged += UpdateScore;
    }

    private void OnDisable()
    {
        _gameManager.OnScoreChanged -= UpdateScore;
    }

    private void UpdateScore(int score)
    {
        if (_scoreLabel != null)
        {
            _scoreLabel.text = $"Score: {score}";
        }
    }
}
