using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;

public class GameBoardController : MonoBehaviour, BoardController
{

    GameManager _gameManager;
    GameObject _board;
    int _boardSize;
    Square[,] _squares;
    Piece[,] _pieces;
    GameLogic _logic;
    Piece _selectedPiece = null;
    List<Path> _moveablePaths = new List<Path>();


    internal GameLogic Init(GameObject board,int boardSize, Square[,] squares, Piece[,] pieces, PlayerColor startColor)
    {

        _board = board;
        _boardSize = boardSize;
        _squares = squares;
        _pieces = pieces;
        _boardSize = boardSize;
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("GameManager doesn't found");
        }

        _logic = new GameLogic(this, _gameManager, _boardSize, startColor);

        return _logic;
    }

    void Update()
    {

        if (_logic?.BoardState == GameState.WaitForAction && Input.GetButtonDown("Fire1"))
        {

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {

                if (hit.collider.gameObject.transform.parent.tag == "GamePiece")
                {
                    SelectePiece(hit);
                }

                if (hit.collider.gameObject.transform.parent.tag == "Square")
                {
                    SelecteSquare(hit);
                }

            }
        }

    }

    void SelecteSquare(RaycastHit hit)
    {

        var selectedSquare = hit.transform.parent.GetComponent<Square>();

        if (_moveablePaths == null || _moveablePaths.Count == 0)
        {
            return;
        }

        Path selectedPath = _moveablePaths[0];
        var length = selectedPath.moveableSquares.Count;
        
        foreach (var path in _moveablePaths)
        {
            var lastSqureInPath = path.moveableSquares[path.moveableSquares.Count - 1];

            if (lastSqureInPath == selectedSquare)
            {
                _ = MovePieceInPath(_selectedPiece, path);
                return;
            }

        }

    }

    void SelectePiece(RaycastHit hit)
    {

        var newSelectedGamePiece = hit.transform.parent.GetComponent<Piece>();

        if (newSelectedGamePiece.Color == _logic.ActivePlayer)
        {

            DeselectPiece();

            _selectedPiece = newSelectedGamePiece;

            _selectedPiece.MarkAsSelected(true);

            var paths = _logic.GetMoveablePaths(_selectedPiece);

            MarkPaths(paths);

            _moveablePaths = paths;
        }
    }

    void RemovePieceFromBoard(Piece eatenPiece)
    {
        Debug.Log($"{eatenPiece.name} remove from board");

        _pieces[eatenPiece.Row, eatenPiece.Col] = null;

        Destroy(eatenPiece.gameObject);
    }

    void MarkPaths(List<Path> paths)
    {
        foreach(var path in paths)
        {
            foreach (var moveable in path.moveableSquares)
            {
                moveable.SetAsOptionalMove(true);
            }

            foreach (var pieces in path.eatablePieces)
            {
                pieces?.SetAsEatable(true);
            }
        }

    }

    Task MoveGamePiece(Piece gamePiece , Square square)
    {

        var startPos = gamePiece.GetPostion();

        Task move = gamePiece.MoveToPos(square.Col, square.Row);

        // Update the new postion in the game pieces 2d array
        _pieces[startPos.x, startPos.y] = null;
        _pieces[square.Row, square.Col] = gamePiece;

        return move;
    }

    void OnDestroy()
    {
        Destroy(_board);
    }

    public Piece GetPieceAtPos(Vector2Int pos)
    {
        return _pieces[pos.x, pos.y];
    }

    public Square GetSquareAtPos(Vector2Int pos)
    {
        return _squares[pos.x, pos.y];
    }

    public void DeselectPiece()
    {

        if (_selectedPiece == null)
        {
            return;
        }

        _selectedPiece.MarkAsSelected(false);

        foreach (var path in _moveablePaths)
        {
            foreach (Square optinalMoves in path.moveableSquares)
            {
                optinalMoves.SetAsOptionalMove(false);
            }

            foreach (Piece pieces in path.eatablePieces)
            {
                if (pieces?.gameObject != null)
                {
                    pieces.SetAsEatable(false);
                }

            }
        }

        _moveablePaths = null;
        _selectedPiece = null;

    }

    public async Task MovePieceInPath(Piece gamePiece, Path path)
    {
        _logic.BoardState = GameState.PiecesMoving;


        foreach (var square in path.moveableSquares)
        {
            await MoveGamePiece(gamePiece, square);
        }

        foreach (var piece in path.eatablePieces)
        {
            RemovePieceFromBoard(piece);
        }

        if (_logic.IsNeedBecomeSuperPiece(gamePiece))
        {
            gamePiece.UpgardeToSuperPiece();
        }

        
        if (_logic.IsWin(gamePiece.Color))
        {
            DeselectPiece();
            _gameManager.SetWinner(gamePiece.Color);
            Debug.Log($"{gamePiece.Color } Win");
        }
        else
        {
            _logic.NextPlayer();
        }
         
    }

    public List<Piece> GetAllPieceInColor(PlayerColor PlayerColor)
    {
        List<Piece> pieces = new List<Piece>();
        foreach (var piece in _pieces)
        {
            if (piece != null && piece.Color == PlayerColor)
            {
                pieces.Add(piece);
            }
        }

        return pieces;
    }

    public void DestroyBoard()
    {
        Destroy(this);
    }
}

