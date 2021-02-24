using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(PieceMover))]
[RequireComponent(typeof(MeshRenderer))]
public class Piece : MonoBehaviour
{
    public float heightIncAsKing = 2f;

    // hide in the inspector
    public PlayerColor Color;
    public int Row;
    public int Col;
    public bool IsSuperPiece = false;

    Material _selectedMat;
    Material _eatableMat;
    Material _initMatrial;
    MeshRenderer _meshRenderer;
    PieceMover _gamePieceMover;

    void Awake()
    { 
        _gamePieceMover = GetComponent<PieceMover>();
    }

    internal Task MoveToPos(int col, int row)
    {
        this.Row = row;
        this.Col = col;
        return _gamePieceMover.MoveTo(col, row);
    }

    internal void MarkAsSelected(bool isSelected)
    {
        if (isSelected)
        {
            _gamePieceMover.Pop();
            _meshRenderer.material = _selectedMat;
        }
        else
        {
            _meshRenderer.material = _initMatrial;
        }

    }

    internal void SetAsEatable(bool isEatable)
    {

        if (isEatable)
        {
            _meshRenderer.material = _eatableMat;
        }
        else
        {
            _meshRenderer.material = _initMatrial;
        }
    }

    internal Vector2Int GetPostion()
    {
        return new Vector2Int(Row, Col);
    }

    internal void Init(int row, int col, PlayerColor color, Material material, Material selectedMat, Material eatablePiece)
    {
        Row = row;
        Col = col;
        Color = color;
        _selectedMat = selectedMat;
        _eatableMat = eatablePiece;


        gameObject.name = $"{row}:{col} - {color}";
        gameObject.transform.position = new Vector3(col, 0, row);
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        _meshRenderer.material = material;
        _initMatrial = material;

        gameObject.SetActive(true);
    }

    internal void UpgardeToSuperPiece()
    {
        
        if(IsSuperPiece == false)
        {
            _gamePieceMover.Pop();
            _gamePieceMover.Pop();
            IsSuperPiece = true;
            var scale = transform.localScale;
            transform.localScale = new Vector3(scale.x, scale.y + heightIncAsKing, scale.z);
        }

    }
}
