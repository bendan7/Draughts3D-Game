using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{

    public Material SelectedMaterial;
    public PlayerColor PieceColor;

    public int Row;
    public int Col;

    private Material _initMatrial;
    private MeshRenderer _meshRenderer;
    private Animation _animation;



    void Awake()
    {
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        _initMatrial = _meshRenderer.material;
        _animation = GetComponentInChildren<Animation>();

        

        if (_initMatrial == null)
        {
            Debug.LogError("Material no found");
        }
    }



    public (int row, int col) OnSelected()
    {
        _animation.Play();
        _meshRenderer.material = SelectedMaterial;
        return (Row, Col);
    }

    public void OnDeselect()
    {
        _meshRenderer.material = _initMatrial;
    }



}
