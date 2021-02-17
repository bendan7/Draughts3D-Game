﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class GamePieceMover : MonoBehaviour
{
    public float journeyTime = 1f;
    public float speed = 2f;

    float _startTime;
    Vector3 _startPos;
    Vector3 _endPos;
    Vector3 centerPoint;
    Vector3 startRelCenter;
    Vector3 endRelCenter;

    private Animation _popAnimation;
    private bool _isMoving = false;
    private TaskCompletionSource<object> _moving = null;

    private void Awake()
    {
        _popAnimation = GetComponentInChildren<Animation>();
    }

    internal void Pop()
    {
        _popAnimation.Play();
    }

    public Task MoveTo(int x, int z)
    {
        _startPos = transform.position;
        _endPos = new Vector3(x, 0, z);
        _startTime = Time.time;
        _moving = new TaskCompletionSource<object>();
        _isMoving = true;

        return _moving.Task;
    }

    void Update()
    {
        if (transform.position == _endPos)
        {
            _isMoving = false;
            _moving?.TrySetResult(null);
        }

        if (_isMoving)
        {
            GetCenter(Vector3.up);

            float fracComplete = (Time.time - _startTime) / journeyTime * speed;
            transform.position = Vector3.Slerp(startRelCenter, endRelCenter, fracComplete * speed);
            transform.position += centerPoint;
        }
    }

    public void GetCenter(Vector3 direction)
    {
        centerPoint = (_startPos + _endPos) * .5f;
        centerPoint -= direction;
        startRelCenter = _startPos - centerPoint;
        endRelCenter = _endPos - centerPoint;
    }
}
