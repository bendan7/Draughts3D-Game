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

    private List<Square> _moveableSquares = new List<Square>();

    PlayerColor activePlayer = PlayerColor.White;

    private void Awake()
    {

        _boardBuilder = GetComponent<CreateBoardScript>();
        (_board, _gamePieces) = _boardBuilder.BuildNewGameBoard();
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

            _moveableSquares = MarkMoveableSquaresByGPPostion(row, col);
        }
    }

    private void GamePieceDeselect()
    {
        _selectedGP?.Deselect();

        foreach (Square optinalMoves in _moveableSquares)
        {
            optinalMoves.SetAsOptionalMove(false);
        }
    }

    private List<Square> MarkMoveableSquaresByGPPostion(int row, int col)
    {

        List<Square> moveableSquares = new List<Square>();
      
        if (activePlayer == PlayerColor.White)
        {

            if (col + 1 < _boardBuilder.BoardSize &&
                _gamePieces[row + 1, col + 1] == null)
            {

                moveableSquares.Add(_board[row + 1, col + 1]);
                _board[row + 1, col + 1].SetAsOptionalMove(true);
               
            }

            if (col - 1 >= 0 &&
                _gamePieces[row + 1, col - 1] == null)
            {

                moveableSquares.Add(_board[row + 1, col - 1]);
                _board[row + 1, col - 1].SetAsOptionalMove(true);

            }
        }

        return moveableSquares;

    }
}

