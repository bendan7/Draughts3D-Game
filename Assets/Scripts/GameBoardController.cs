using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;



public class GameBoardController : MonoBehaviour
{

    public PlayerColor ActivePlayer = PlayerColor.White;

    BoardState _boardState = BoardState.WaitForAction;
    GameManager _gameManager;

    GameObject _board;
    int _boardSize;
    Square[,] _squares;
    Piece[,] _pieces;
    

    Piece _selectedPiece = null;
    List<Path> _moveablePaths = new List<Path>();

    List<Piece> _opponentPieces = new List<Piece>();

    internal void Init(GameObject board,int boardSize, Square[,] squares, Piece[,] pieces)
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

        foreach (var piece in _pieces)
        {
            if (piece != null && piece.Color == PlayerColor.Black)
            {
                _opponentPieces.Add(piece);
            }
        }
    }

    void Update()
    {

        if (_boardState == BoardState.WaitForAction && Input.GetButtonDown("Fire1"))
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

    private void SelecteSquare(RaycastHit hit)
    {

        var selectedSquare = hit.transform.parent.GetComponent<Square>();

        if (_moveablePaths == null)
        {
            return;
        }

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

    private void SelectePiece(RaycastHit hit)
    {

        var newSelectedGamePiece = hit.transform.parent.GetComponent<Piece>();

        if (newSelectedGamePiece.Color == ActivePlayer)
        {

            DeselectPiece();

            _selectedPiece = newSelectedGamePiece;

            _selectedPiece.MarkAsSelected(true);

            var paths = GetPaths(_selectedPiece.Color, _selectedPiece.GetPostion());

            MarkPaths(paths);

            _moveablePaths = paths;
        }
    }

    private void DeselectPiece()
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

    private async Task MovePieceInPath(Piece gamePiece, Path path)
    {
        _boardState = BoardState.PiecesMoving;

        List<Piece> eaten = new List<Piece>();

        foreach (var square in path.moveableSquares)
        {
            
            if (TryGetEatenPiece(gamePiece, square , out Piece eatenPiece))
            {
                eaten.Add(eatenPiece);
            }
            
            await MoveGamePiece(gamePiece, square);
        }

        foreach (var piece in eaten)
        {
            RemovePieceFromBoard(piece);
        }

        if(IsWinPath(path) == false)
        {
            NextPlayer();
        }
        else
        {
            DeselectPiece();
            Debug.Log(gamePiece.Color + " WINNNNNN");
            //ACT 
        }
        
        
    }

    private bool IsWinPath(Path path)
    {
        var lastSquareInPath = path.moveableSquares[path.moveableSquares.Count - 1];
        if (lastSquareInPath.Row == 0 || lastSquareInPath.Row == _boardSize-1)
        {
            return true;
        }

        return false;
    }

    private void RemovePieceFromBoard(Piece eatenPiece)
    {
        Debug.Log($"{eatenPiece.name} been eat");

        _pieces[eatenPiece.Row, eatenPiece.Col] = null;
        _opponentPieces.Remove(eatenPiece);

        Destroy(eatenPiece.gameObject);
    }

    private void NextPlayer()
    {

        
        DeselectPiece();
        
        ActivePlayer = ActivePlayer == PlayerColor.Black ? PlayerColor.White : PlayerColor.Black;

        _gameManager.SetActivePlayer(ActivePlayer);


        if (ActivePlayer == PlayerColor.Black)
        {
            Debug.Log("Auto Opponent Play");
            _ = AutoOpponentAsync();
            
        }
        else
        {
            _boardState = BoardState.WaitForAction;
        }
 
    }

    private async Task AutoOpponentAsync()
    {

        Piece selectedPiece = null;
        Path bestPath= null;

        bool isFirst = true;

        var rand = UnityEngine.Random.Range(0, _opponentPieces.Count);

        for(int i =0; i < _opponentPieces.Count; i ++)
        {
            var randIndex = (i + rand) % _opponentPieces.Count;
            var piece = _opponentPieces[randIndex];

            var paths = GetPaths(piece.Color, piece.GetPostion());
            foreach(var path in paths)
            {
                if (isFirst)
                {
                    selectedPiece = piece;
                    bestPath = path;
                    isFirst = false;
                }

                if (IsWinPath(path))
                {
                    selectedPiece = piece;
                    bestPath = path;
                    break;
                }

                if(path.eatablePieces.Count > bestPath.eatablePieces.Count)
                {

                    selectedPiece = piece;
                    bestPath = path;
                }
            }
        }

        await MovePieceInPath(selectedPiece, bestPath);

    }

    private bool TryGetEatenPiece(Piece Eater, Square moveTo, out Piece eatenPiece)
    {

        (var Add, var Sub , var GreaterThan) = GetSwapOperatorByColor(Eater.Color);

        if (GreaterThan( moveTo.Col, Eater.Col + 1 ))
        {
            //Try eat right side from white perspective

            eatenPiece = _pieces[Add(Eater.Row, 1), Add(Eater.Col, 1)];
            return eatenPiece != null;
        }
        else if (GreaterThan( Eater.Col - 1, moveTo.Col))
        {

            // Try eat left side from white perspective
            eatenPiece = _pieces[Add(Eater.Row, 1), Sub(Eater.Col, 1)];
            return eatenPiece != null;
        }

        eatenPiece = null;
        return false;
    }

    private void MarkPaths(List<Path> paths)
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

    /// <returns>Return list of all possible paths to move </returns>
    private List<Path> GetPaths(PlayerColor playerColor,
                                Vector2 fromPos,
                                Path previousPath = null,
                                List<Path> allPaths = null)
    {
 
        allPaths = allPaths == null ? new List<Path>() : allPaths;

        int col = (int)fromPos.y;
        int row = (int)fromPos.x;

        // Same logic for both side by swapping the operators
        (var Add, var Sub, var GreaterThan) = GetSwapOperatorByColor(playerColor);


        // Right Side
        if (IsPosInBoard(Add, col, 1) && IsPosInBoard(Add, row, 1))
        {
            var rightPath = previousPath == null ? new Path() : new Path(previousPath);

            // Just Move
            if (_pieces[Add(row, 1), Add(col, 1)] == null && previousPath == null)
            {
                rightPath.moveableSquares.Add(_squares[Add(row, 1), Add(col, 1)]);
                allPaths.Add(rightPath);
            }

            // Eat opponent
            else
            if (IsPosInBoard(Add, col, 2) &&
                IsPosInBoard(Add, row, 2) &&
                _pieces[Add(row, 2), Add(col, 2)] == null &&
                _pieces[Add(row, 1), Add(col, 1)] != null &&
                _pieces[Add(row, 1), Add(col, 1)].Color != playerColor)
            {

                rightPath.eatablePieces.Add(_pieces[Add(row, 1), Add(col, 1)]);
                rightPath.moveableSquares.Add(_squares[Add(row, 2), Add(col, 2)]);
                allPaths.Add(rightPath);

                GetPaths(playerColor, new Vector2(Add(row, 2), Add(col, 2)) , rightPath, allPaths);;
            }

                
        }


        // Left Side
        if (IsPosInBoard(Sub, col, 1) && IsPosInBoard(Add, row, 1))
        {
            var leftPath = previousPath == null ? new Path() : new Path(previousPath);

            // Just Move
            if (_pieces[Add(row, 1), Sub(col, 1)] == null && previousPath == null)
            {
                leftPath.moveableSquares.Add(_squares[Add(row, 1), Sub(col, 1)]);
                allPaths.Add(leftPath);    
            }

            // Eat opponent
            else
            if (IsPosInBoard(Sub,col,2) &&
                IsPosInBoard(Add,row,2) &&
                _pieces[Add(row, 2), Sub(col, 2)] == null &&
                _pieces[Add(row, 1), Sub(col, 1)] != null && 
                _pieces[Add(row, 1), Sub(col, 1)].Color != playerColor)
            {

                leftPath.eatablePieces.Add(_pieces[Add(row, 1), Sub(col, 1)]);
                leftPath.moveableSquares.Add(_squares[Add(row, 2), Sub(col, 2)]);
                allPaths.Add(leftPath);

                GetPaths(playerColor, new Vector2(Add(row, 2), Sub(col, 2)), leftPath, allPaths);
            }
        }


            return allPaths;
    }


    private bool IsPosInBoard(Func<int, int, int> op, int a, int b)
    {
       

        return op(a, b) >= 0 && op(a, b) < _boardSize;
    }

    private Task MoveGamePiece(Piece gamePiece , Square square)
    {

        var startPos = gamePiece.GetPostion();

        Task move = gamePiece.MoveToPos(square.Col, square.Row);

        // Update the new postion in the game pieces 2d array
        _pieces[(int)startPos.x, (int)startPos.y] = null;
        _pieces[square.Row, square.Col] = gamePiece;

        return move;
    }

    private (Func<int, int, int>, Func<int, int, int>, Func<int, int, bool>)
        GetSwapOperatorByColor(PlayerColor playerColor)
    {

        Func<int, int, int> add;
        Func<int, int, int> sub;
        Func<int, int, bool> GreaterThan;

        if (ActivePlayer == PlayerColor.White)
        {
            add = (x, y) => { return x + y; };
            sub = (x, y) => { return x - y; };
            GreaterThan = (x, y) => { return x > y; };
        }
        else
        {
            add = (x, y) => { return x - y; };
            sub = (x, y) => { return x + y; };
            GreaterThan = (x, y) => { return x < y; };
        }

        return (add,sub, GreaterThan);
    }

    private void OnDestroy()
    {
        Destroy(_board);
    }
}

