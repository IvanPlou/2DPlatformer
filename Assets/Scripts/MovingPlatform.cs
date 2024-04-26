using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _moveDelay = 1.5f;
    [SerializeField] private Transform _target;

    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _startPosition = transform.position;
        _endPosition = _target.position;
        _rb = gameObject.GetComponent<Rigidbody2D>();
        StartCoroutine(MoveCycle());
    }

    private IEnumerator MoveCycle()
    {
        bool isMovingBack = false;
        while(true) 
        {
            yield return new WaitForSeconds(_moveDelay);
            yield return StartCoroutine(MoveCoro(isMovingBack));
            isMovingBack = !isMovingBack;
        }
    }

    private IEnumerator MoveCoro(bool movingBack)
    {
        float totalDuration = Vector3.Distance(_startPosition, _endPosition) / _moveSpeed;
        float direction = movingBack? -1 : 1;
        _rb.velocity = _moveSpeed * direction * (_endPosition - _startPosition).normalized;
        yield return new WaitForSeconds(totalDuration);
        _rb.velocity = Vector2.zero;
    }

}
