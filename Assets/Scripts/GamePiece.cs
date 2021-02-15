using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{

    public Material SelectedMaterial;
    public PlayerColor PieceColor;

    public int Row;
    public int Col;

    Rigidbody _rb;
    Material _initMatrial;
    MeshRenderer _meshRenderer;


    
    void Awake()
    {
        _rb = GetComponentInChildren<Rigidbody>();
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        _initMatrial = _meshRenderer.material;

        if (_rb == null)
        {
            Debug.LogError("Rigidbody no found");
        }

        if (_initMatrial == null)
        {
            Debug.LogError("Material no found");
        }
    }



    public (int row, int col) OnSelected()
    {
        _rb.AddForce(0, 100, 0);
        _meshRenderer.material = SelectedMaterial;
        return (Row, Col);
    }

    public void OnDeselect()
    {
        _meshRenderer.material = _initMatrial;
    }



}
