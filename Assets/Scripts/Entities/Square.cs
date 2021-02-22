using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    public int Row;
    public int Col;

    Material _selectedMaterial;
    Material _color;
    MeshRenderer _meshRenderer;

    private void Start()
    {
        _color = GetComponentInChildren<MeshRenderer>().material;
        
    }

    internal void SetAsOptionalMove(bool isOptional)
    {
        if (isOptional)
        {
            _meshRenderer.material = _selectedMaterial;
        }
        else
        {
            _meshRenderer.material = _color;
        }
    }

    internal Vector2 GetPostion()
    {
        return new Vector2(Row, Col);
    }

    internal void Init(int row, int col, Material squareMat, Material squareMoveableMat)
    {

        gameObject.name = $"{row}:{col}";
        gameObject.transform.position = new Vector3(col, 0,row );

        Row = row;
        Col = col;
        _selectedMaterial = squareMoveableMat;

        _color = squareMat;

        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        _meshRenderer.material = _color;


    }
}
