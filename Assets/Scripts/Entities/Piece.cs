using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(GamePieceMover))]
[RequireComponent(typeof(MeshRenderer))]
public class Piece : MonoBehaviour
{
    [HideInInspector]
    public PlayerColor Color;

    [HideInInspector]
    public Material SelectedMaterial;

    [HideInInspector]
    public Material Eatable;

    public int Row;

    public int Col;

    private Material _initMatrial;
    private MeshRenderer _meshRenderer;
    private GamePieceMover _gamePieceMover;

    void Awake()
    {
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        _initMatrial = _meshRenderer.material;   
        _gamePieceMover = GetComponent<GamePieceMover>();

    }

    public Task MoveToPos(int col, int row)
    {
        Row = row;
        Col = col;
        return _gamePieceMover.MoveTo(col, row);
    }

    public void MarkAsSelected(bool isSelected)
    {
        if (isSelected)
        {
            _gamePieceMover.Pop();
            _meshRenderer.material = SelectedMaterial;
        }
        else
        {
            _meshRenderer.material = _initMatrial;
        }

    }

    public void SetAsEatable(bool isEatable)
    {

        if (isEatable)
        {
            _meshRenderer.material = Eatable;
        }
        else
        {
            _meshRenderer.material = _initMatrial;
        }
    }

    public Vector2 GetPostion()
    {
        return new Vector2(Row, Col);
    }

}
