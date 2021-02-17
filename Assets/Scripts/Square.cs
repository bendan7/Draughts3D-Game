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
