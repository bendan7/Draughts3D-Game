using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GamePieceMover))]
[RequireComponent(typeof(MeshRenderer))]
public class GamePiece : MonoBehaviour
{
    [HideInInspector]
    public PlayerColor PieceColor;

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

    public void MoveToPos(int col, int row)
    {
        _gamePieceMover.MoveTo(col, row);
        Row = row;
        Col = col;
    }

    public (int row, int col) Select()
    {

        _gamePieceMover.Pop();

        _meshRenderer.material = SelectedMaterial;

        return (Row, Col);
    }

    public void Deselect()
    {
        _meshRenderer.material = _initMatrial;
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
