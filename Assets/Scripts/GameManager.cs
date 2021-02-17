using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameBoardCreator))]
public class GameManager : MonoBehaviour
{

    private GameBoardCreator _boardBuilder;

    private Square[,] _board;
    private GamePiece[,] _gamePieces;

    private GamePiece _selectedGP = null;

    private List<Square> _moveableSquares = new List<Square>();
    private List<GamePiece> _eatablePieces = new List<GamePiece>();

    PlayerColor activePlayer = PlayerColor.White;

    private void Awake()
    {
        _boardBuilder = GetComponent<GameBoardCreator>();
        (_board, _gamePieces) = _boardBuilder.BuildNewGameBoard();
    }

    private void Start()
    {
        MoveGamePieceToSquare(_gamePieces[5, 1], _board[4, 2]);
        MoveGamePieceToSquare(_gamePieces[5, 5], _board[4, 4]);

        MoveGamePieceToSquare(_gamePieces[2, 2], _board[3, 3]);
    }

    private void MoveGamePieceToSquare(GamePiece gamePiece, Square square)
    {
        var startPos = gamePiece.GetPostion();

        gamePiece.MoveToPos(square.Col,square.Row );

        // Update the new postion in the game pieces 2d array
        _gamePieces[(int)startPos.x, (int)startPos.y] = null;
        _gamePieces[square.Row, square.Col] = gamePiece;

        GamePieceDeselect();
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

                if (hit.collider.gameObject.transform.parent.tag == "Square")
                {
                    SquareSelected(hit);
                }

            }
        }

    }

    private void SquareSelected(RaycastHit hit)
    {
        var selectedSquare = hit.transform.parent.GetComponent<Square>();

        if (_moveableSquares.Contains(selectedSquare))
        {

            Debug.Log($"Move GP {_selectedGP.Row}:{_selectedGP.Col}  TO  {selectedSquare.Row}:{selectedSquare.Col}");   
            MoveGamePieceToSquare(_selectedGP, selectedSquare);

        }
        
    }

    private void GamePieceSelected(RaycastHit hit)
    {
        GamePieceDeselect();

        var newSelectedGamePiece = hit.transform.parent.GetComponent<GamePiece>();

        if (newSelectedGamePiece.PieceColor == activePlayer)
        {
            _selectedGP?.Deselect();

            _selectedGP = newSelectedGamePiece;

            (int row, int col) = _selectedGP.Select();

            MarkMoveableSquaresByGPPostion(row, col);
        }
    }

    private void GamePieceDeselect()
    {
        _selectedGP?.Deselect();

        foreach (Square optinalMoves in _moveableSquares)
        {
            optinalMoves.SetAsOptionalMove(false);
        }

        foreach (GamePiece pieces in _eatablePieces)
        {
            pieces.SetAsEatable(false);
        }
    }

    private void MarkMoveableSquaresByGPPostion(int row, int col)
    {

        List<Square> moveableSquares = new List<Square>();
        List<GamePiece> eatablePieces = new List<GamePiece>();
        var boardSize = _boardBuilder.BoardSize;


        if (activePlayer == PlayerColor.White)
        {

            if (col + 1 < boardSize &&
                _gamePieces[row + 1, col + 1] == null)
            {

                moveableSquares.Add(_board[row + 1, col + 1]);

               
            } else if(
                   col + 2 < boardSize &&
                   _gamePieces[row + 2, col + 2] == null &&
                   _gamePieces[row + 1, col + 1].PieceColor == PlayerColor.Black)
            {

                eatablePieces.Add(_gamePieces[row + 1, col + 1]);
                moveableSquares.Add(_board[row + 2, col + 2]);
            }




            if (col - 1 >= 0 &&
                _gamePieces[row + 1, col - 1] == null)
            {
                moveableSquares.Add(_board[row + 1, col - 1]);
            }
            else if(
                col - 2 >= 0 &&
                _gamePieces[row + 2, col - 2] == null &&
                _gamePieces[row + 1, col - 1].PieceColor == PlayerColor.Black)
            {
                moveableSquares.Add(_board[row + 2, col - 2]);
                eatablePieces.Add(_gamePieces[row + 1, col - 1]);
            }
        }

        foreach(var moveable in moveableSquares)
        {
            moveable.SetAsOptionalMove(true);
        }

        foreach (var pieces in eatablePieces)
        {
            pieces.SetAsEatable(true);
        }

        _moveableSquares = moveableSquares;
        _eatablePieces = eatablePieces;

    }
}

