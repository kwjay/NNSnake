using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    public List<Vector2Int> BodyPos => _bodyPos;
    public Vector2Int Heading => _heading;

    [SerializeField] private GameObject _bodyPb;

    private List<Transform> _bodyVisuals = new();
    private List<Vector2Int> _bodyPos = new();
    private Vector2Int _heading = Vector2Int.right;

    public void StepForward()
    {
        AdjustPositions();
        SyncVisuals();
    }

    public void SetDirection(Vector2Int direction)
    {
        _heading = direction;
    }

    private void AdjustPositions()
    {
        Vector2Int headPos = _bodyPos[0] + _heading;

        for (int i = _bodyPos.Count - 1; i > 0; i--)
        {
            _bodyPos[i] = _bodyPos[i - 1];
        }
        _bodyPos[0] = headPos;
    }

    private void SyncVisuals()
    {
        for (int i = 0; i < _bodyVisuals.Count; i++)
        {
            _bodyVisuals[i].position = new Vector3(_bodyPos[i].x, _bodyPos[i].y, 0);
        }
    }

    public void ExtendBody()
    {
        Vector2Int tailPos = _bodyPos[^1];
        _bodyPos.Add(tailPos);
        GameObject body = Instantiate(_bodyPb, new Vector3(tailPos.x, tailPos.y, 0), _bodyPb.transform.rotation);
        _bodyVisuals.Add(body.transform);
    }

    public bool CheckSelfCollision(Vector2Int headPos)
    {
        for (int i = 1; i < _bodyPos.Count; i++)
        {
            if (_bodyPos[i] == headPos)
                return true;
        }

        return false;
    }

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {

        _bodyPos.Add(Vector2Int.RoundToInt((Vector2)transform.position));
        _bodyVisuals.Add(transform);
        ExtendBody();
    }

    private void OnDestroy()
    {
        
        for (int i = 1; i < _bodyVisuals.Count; i++)
            if (_bodyVisuals[i] != null) Destroy(_bodyVisuals[i].gameObject);
        _bodyPos.Clear();
        _bodyVisuals.Clear();
    }

}
