using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    PlayerColor _activePlayer = PlayerColor.Black;
    GameState _gameState = GameState.WaitForAction;

    private void Awake()
    {
        _boardBuilder = GetComponent<GameBoardCreator>();
        (_board, _gamePieces) = _boardBuilder.BuildNewGameBoard();
    }

    private void Start()
    {
        
        //Demo Moves
        /*
        _ = MoveGamePieceToSquareAsync(_gamePieces[5, 1], _board[4, 2]);
        _ = MoveGamePieceToSquareAsync(_gamePieces[5, 5], _board[4, 4]);

        _ = MoveGamePieceToSquareAsync(_gamePieces[2, 2], _board[3, 3]);
        _ = MoveGamePieceToSquareAsync(_gamePieces[0, 0], _board[0, 0]);
        */

    }

    private async Task MoveGamePieceToSquareAsync(GamePiece gamePiece, Square square)
    {
        _gameState = GameState.PiecesMoving;

        var startPos = gamePiece.GetPostion();

        GamePiece eatenPiece = TryGetEatenPiece(gamePiece, square);

        Task move = gamePiece.MoveToPos(square.Col, square.Row);

        
        if (eatenPiece)
        {
            _eatablePieces.Remove(eatenPiece);
        }


        // Update the new postion in the game pieces 2d array
        _gamePieces[(int)startPos.x, (int)startPos.y] = null;
        _gamePieces[square.Row, square.Col] = gamePiece;

        GamePieceDeselect();

        await move;

        if (eatenPiece)
        {
            Debug.Log($"eated: {eatenPiece.name}");
            Destroy(eatenPiece.gameObject);
        }


        NextPlayer();

    }

    private void NextPlayer()
    {
        _activePlayer = _activePlayer == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;
        

        if(_activePlayer == PlayerColor.Black)
        {
            //AutoPlay();
            _gameState = GameState.WaitForAction;
        }
        else
        {
            _gameState = GameState.WaitForAction;
        }
 
    }

    private void AutoPlay()
    {
        Debug.Log("Computer turn");

        /*
        foreach (GamePiece piece in _opponentPieces)
        {

            Square square = IsCanEat(piece);

            if (square)
            {
                piece.Select();
                _ = MoveGamePieceToSquareAsync(piece, square);
                return;
            }
        }

        foreach (GamePiece piece in _opponentPieces)
        {

            Square square = IsCanMove(piece);

            if (square)
            {
                piece.Select();
                _ = MoveGamePieceToSquareAsync(piece, square);
                return;
            }
        }
        */

    }

    private GamePiece TryGetEatenPiece(GamePiece Eater, Square moveTo)
    {

        foreach(var piece in _eatablePieces)
        {

            if(_activePlayer == PlayerColor.White)
            {
                if (Eater.Col < moveTo.Col)
                {
                    // eat left
                    return _gamePieces[Eater.Row + 1, Eater.Col + 1];
                }
                else
                {
                    return _gamePieces[Eater.Row + 1, Eater.Col - 1];

                }
            }
            else
            {

                if (Eater.Col < moveTo.Col)
                {
                    return _gamePieces[Eater.Row - 1, Eater.Col + 1];
                }
                else
                {
                    return _gamePieces[Eater.Row - 1, Eater.Col - 1];

                }
            }



        }


        return null;
    }

    void Update()
    {

        if (_gameState == GameState.WaitForAction && Input.GetButtonDown("Fire1"))
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

            Debug.Log($"Move GP {_selectedGP.name}  TO  {selectedSquare.Row}:{selectedSquare.Col}");   
            _ = MoveGamePieceToSquareAsync(_selectedGP, selectedSquare);

        }
        
    }

    private void GamePieceSelected(RaycastHit hit)
    {

        GamePieceDeselect();

        var newSelectedGamePiece = hit.transform.parent.GetComponent<GamePiece>();

        if (newSelectedGamePiece.Color == _activePlayer)
        {

            
            _selectedGP = newSelectedGamePiece;
            (int row, int col) = _selectedGP.Select();

            (_moveableSquares,_eatablePieces) = GetOptinalMoves(_selectedGP);
            

            MarkSquares(_moveableSquares);
            MarkEatablePieces(_eatablePieces);

        }
    }



    private (List<Square>,List<GamePiece>) GetOptinalMoves(GamePiece selectedGP)
    {

        int col = selectedGP.Col;
        int row = selectedGP.Row;

        Func<int, int, int> Add;
        Func<int, int, int> Sub;

        if (_activePlayer == PlayerColor.White)
        {
            Add = (x, y) => { return x + y; };
            Sub = (x, y) => { return x - y; };
        }
        else
        {
            Add = (x, y) => { return x - y; };
            Sub = (x, y) => { return x + y; };
        }


        List<Square> moveableSquares = new List<Square>();
        List<GamePiece> eatablePieces = new List<GamePiece>();
        var boardSize = _boardBuilder.BoardSize;



        if (true)
        {

            if (Add(col, 1) < boardSize &&
                _gamePieces[Add(row, 1), Add(col, 1)] == null)
            {

                moveableSquares.Add(_board[Add(row, 1), Add(col, 1)]);


            }
            else if (
                 Add(col, 2) < boardSize &&
                 _gamePieces[Add(row, 2), Add(col, 2)] == null &&
                 _gamePieces[Add(row, 1), Add(col, 1)].Color != selectedGP.Color)
            {

                eatablePieces.Add(_gamePieces[Add(row, 1), Add(col, 1)]);
                moveableSquares.Add(_board[Add(row, 2), Add(col, 2)]);
            }



            if (Sub(col, 1) >= 0 && Sub(col, 1) < boardSize &&
                _gamePieces[Add(row, 1), Sub(col, 1)] == null)
            {
                moveableSquares.Add(_board[Add(row, 1), Sub(col, 1)]);
            }
            else if (
                Sub(col, 2) >= 0 && Sub(col, 2) < boardSize &&
                _gamePieces[Add(row, 2), Sub(col, 2)] == null &&
                _gamePieces[Add(row, 1), Sub(col, 1)].Color != selectedGP.Color)
            {
                moveableSquares.Add(_board[Add(row, 2), Sub(col, 2)]);
                eatablePieces.Add(_gamePieces[Add(row, 1), Sub(col, 1)]);
            }
        }



        return (moveableSquares,eatablePieces);
    }




    private void MarkEatablePieces(List<GamePiece> eatablePieces)
    {
        foreach (var pieces in eatablePieces)
        {
            pieces.SetAsEatable(true);
        }
    }

    private void MarkSquares(List<Square> moveableSquares)
    {
        foreach (var moveable in moveableSquares)
        {
            moveable.SetAsOptionalMove(true);
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



    /*

    private void MarkMoveableSquaresByGPPostion(int row, int col)
    {

        List<Square> moveableSquares = new List<Square>();
        List<GamePiece> eatablePieces = new List<GamePiece>();
        var boardSize = _boardBuilder.BoardSize;


        if (_activePlayer == PlayerColor.White)
        {

            if (col + 1 < boardSize &&
                _gamePieces[row + 1, col + 1] == null)
            {

                moveableSquares.Add(_board[row + 1, col + 1]);

               
            } else if(
                   col + 2 < boardSize &&
                   _gamePieces[row + 2, col + 2] == null &&
                   _gamePieces[row + 1, col + 1].Color == PlayerColor.Black)
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
                _gamePieces[row + 1, col - 1].Color == PlayerColor.Black)
            {
                moveableSquares.Add(_board[row + 2, col - 2]);
                eatablePieces.Add(_gamePieces[row + 1, col - 1]);
            }
        }





        _moveableSquares = moveableSquares;
        _eatablePieces = eatablePieces;

    }

    */
}

