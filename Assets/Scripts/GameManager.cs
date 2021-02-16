using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private CreateBoardScript _boardBuilder;

    private Square[,] _board;
    private GamePiece[,] _gamePieces;

    private GamePiece _selectedGP = null;
    private Square[] _moveableOptions = new Square[2];

    PlayerColor activePlayer = PlayerColor.White;

    private void Awake()
    {

        //DestroyImmediate(GameObject.Find("Board"));
        //DestroyImmediate(GameObject.Find("GamePieces"));

        _boardBuilder = GetComponent<CreateBoardScript>();
        (_board, _gamePieces) = _boardBuilder.BuildNewGameBoard();
    }

    private void Start()
    {
        

        MoveGamePieceToPos(_gamePieces[0, 0], 3, 3);
    }

    private void MoveGamePieceToPos(GamePiece gamePiece, int x, int z)
    {
        _gamePieces[0, 0].MoveToPos(3, 3);

        var gp = _gamePieces[0, 0];
        _gamePieces[0, 0] = null;
        _gamePieces[x, z] = gp;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.transform.parent.tag == "GamePiece")
                {
                    GamePieceSelected(hit);
                }


            }
        }

    }

    private void GamePieceSelected(RaycastHit hit)
    {
        var gamePiece = hit.collider.gameObject.transform.parent.GetComponent<GamePiece>();

        if (gamePiece.PieceColor == activePlayer)
        {
            _selectedGP?.OnDeselect();
            _selectedGP = gamePiece;

            (int row, int col) = _selectedGP.OnSelected();

            MarkMoveableSquaresByGPPostion(row, col);
        }
    }

    private void MarkMoveableSquaresByGPPostion(int row, int col)
    {
        //Deselect previous squres
        foreach(var move in _moveableOptions)
        {
            if(move !=null)
            {
                move.SetAsOptionalMove(false);
            }
            
        }


        if(activePlayer == PlayerColor.White)
        {

            if (col + 1 < _boardBuilder.BoardSize &&
                _gamePieces[row + 1, col + 1] == null)
            {
                _board[row + 1, col + 1].SetAsOptionalMove(true);
                _moveableOptions[0] = _board[row + 1, col + 1];
            }
            else
            {
                _moveableOptions[0] = null;
            }


            if (col - 1 >= 0 &&
                _gamePieces[row + 1, col - 1] == null)
            {
                _board[row + 1, col - 1].SetAsOptionalMove(true);
                _moveableOptions[1] = _board[row + 1, col - 1];
            }
            else
            {
                _moveableOptions[1] = null;
            }

        }

       

    }
}

