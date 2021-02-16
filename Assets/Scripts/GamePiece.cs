using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GamePieceMover))]
public class GamePiece : MonoBehaviour
{

    [HideInInspector]
    public Material SelectedMaterial;

    [HideInInspector]
    public PlayerColor PieceColor;

    [HideInInspector]
    public int Row;

    [HideInInspector]
    public int Col;

    private Material _initMatrial;
    private MeshRenderer _meshRenderer;
    private GamePieceMover _move;

    void Awake()
    {
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        _initMatrial = _meshRenderer.material;
        
        _move = GetComponent<GamePieceMover>();

        if (_initMatrial == null)
        {
            Debug.LogError("Material no found");
        }
    }

    public void MoveToPos(int x, int z)
    {
        _move.MoveTo(x, z);
        Row = x;
        Col = z;
    }

    public (int row, int col) OnSelected()
    {
        _move.Pop();

        _meshRenderer.material = SelectedMaterial;
        return (Row, Col);
    }

    public void OnDeselect()
    {
        _meshRenderer.material = _initMatrial;
    }


    



}
