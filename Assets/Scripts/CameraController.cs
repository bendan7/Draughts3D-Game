using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float Speed = 1;

    const float MAX_DISTANCE = 179f;
    Camera _camera;
    const float _WantedFieldOfViewBoardSize8 = 54f;
    const float _WantedFieldOfViewBoardSize10 = 60f;
    const float _WantedFieldOfViewBoardSize12 = 65f;

    float _wantedFieldOfView = 54f;

    bool _intro = false;
    bool _outro = false;

    TaskCompletionSource<object> _taskCompletionSource;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _camera.fieldOfView = MAX_DISTANCE;
    }

    public Task PlayIntroEffect(int boardSize)
    {
        if(boardSize == 8)
        {
            _wantedFieldOfView = _WantedFieldOfViewBoardSize8;
        }
        if(boardSize == 10)
        {
            _wantedFieldOfView = _WantedFieldOfViewBoardSize10;
        }
        if (boardSize == 12)
        {
            _wantedFieldOfView = _WantedFieldOfViewBoardSize12;
        }

        _intro = true;
        _taskCompletionSource = new TaskCompletionSource<object>();
        return _taskCompletionSource.Task;
    }

    public Task PlayOutroEffect()
    {
        _outro = true;
        _taskCompletionSource = new TaskCompletionSource<object>();
        return _taskCompletionSource.Task;
    }

    private void FixedUpdate()
    {
        if (_intro)
        {
            if(_wantedFieldOfView < _camera.fieldOfView)
            {
                _camera.fieldOfView -= Speed;
            }
            else
            {
                _intro = false;
                _taskCompletionSource.SetResult(null);
            }
            
        }

        if (_outro)
        {
            if (MAX_DISTANCE > _camera.fieldOfView)
            {
                _camera.fieldOfView += Speed;
            }
            else
            {
                
                _outro = false;
                _taskCompletionSource.SetResult(null);
            }

        }
    }
}
