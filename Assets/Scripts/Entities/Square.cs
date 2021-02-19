using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    public int Row;
    public int Col;

    [HideInInspector]
    public Material SelectedMaterial;

    private Material _originalColor;

    private void Start()
    {
        _originalColor = GetComponentInChildren<MeshRenderer>().material;
        
    }


    public void SetAsOptionalMove(bool isOptional)
    {
        if (isOptional)
        {
            GetComponentInChildren<MeshRenderer>().material = SelectedMaterial;
        }
        else
        {
            GetComponentInChildren<MeshRenderer>().material = _originalColor;
        }

    }

    internal Vector2 GetPostion()
    {
        return new Vector2(Row, Col);
    }
}
