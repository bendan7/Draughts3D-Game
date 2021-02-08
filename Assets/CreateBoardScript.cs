using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


[ExecuteInEditMode]
public class CreateBoardScript : MonoBehaviour
{
    private int _boardSize = 8 ;
    public GameObject Cell;
    public GameObject GamePiecePrefab;

    public Material BlackMat;
    public Material WhiteMat;

    public Material SelectedBlack;
    public Material SelectedWhite;

    private GameObject _board;
    private GameObject _gamePieces;



    private void OnEnable()
    {
        _board = GameObject.Find("Board");


        if(_board == null)
        {
            BuildNewGameBoard();
        }

    }


    void BuildNewGameBoard() {

        _board = new GameObject("Board");
        _gamePieces = new GameObject("GamePieces");


        bool isBlackCell = false;

        for (int i = 0; i < _boardSize; i++)
        {
            isBlackCell = !isBlackCell;

            for (int j = 0; j < _boardSize; j++)
            {
                var cell = Instantiate(Cell, _board.transform);

                cell.name = $"{i}:{j}";
                cell.transform.position = new Vector3(j, 0, i);

                if (isBlackCell)
                {
                    if(i == 0 && j == 0)
                    {
                        Debug.Log("!");
                    }
                    cell.GetComponentInChildren<MeshRenderer>().material = BlackMat;
                }
                else
                {
                    cell.GetComponentInChildren<MeshRenderer>().material = WhiteMat;
                }

                
                if (i < 3 && isBlackCell)
                {
                    AddPiece(WhiteMat,j,i);
                }
                else if (i > 4 && isBlackCell)
                {
                    AddPiece(BlackMat,  j,i);
                }


                isBlackCell = !isBlackCell;
            }
        }


        _board.transform.position =new  Vector3(0, -0.05f, 0);
    }

    private void AddPiece(Material color, int i, int j)
    {
        var GamePiece = Instantiate(GamePiecePrefab, _gamePieces.transform);
        GamePiece.name = $"{j}:{i} - {color.name}";
        GamePiece.transform.position = new Vector3(i, 0, j);
        GamePiece.GetComponentInChildren<MeshRenderer>().material = color;


        var selectedMat = color == WhiteMat ? SelectedWhite : SelectedBlack;
        GamePiece.GetComponent<GamePiece>().SelectedMaterial = selectedMat;
    }
}
