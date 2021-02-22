using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float Speed = 1;

    const float MAX_DISTANCE = 179f;
    Camera _camera;
    float _wantedFieldOfView;
    bool _intro = false;
    bool _outro = false;
    TaskCompletionSource<object> _taskCompletionSource;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _wantedFieldOfView = _camera.fieldOfView;
        _camera.fieldOfView = MAX_DISTANCE;
    }

    public Task PlayIntroEffect()
    {
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
