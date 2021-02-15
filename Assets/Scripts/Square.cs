using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    public int Row;
    public int Col;


    public Color _originalColor;
    private bool _isOptinalMove = false;



    private void Start()
    {
        _originalColor = GetComponentInChildren<MeshRenderer>().material.color;
        
    }

    void Update()
    {
        /*
        if(_isOptinalMove == false)
        {
            return;
        }


        Renderer renderer = GetComponentInChildren<Renderer>();
        Material mat = renderer.material;
        var lerpedColor = Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time, 1));

        mat.color = lerpedColor;
        */
    }


    public void SetAsOptionalMove(bool isOptional)
    {
        if (isOptional)
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.green;

        }
        else
        {
            GetComponentInChildren<MeshRenderer>().material.color = _originalColor;
        }
        
        _isOptinalMove = isOptional;
    }


}
