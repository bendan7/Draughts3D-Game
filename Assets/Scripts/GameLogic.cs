using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameLogic
{

    public PlayerColor ActivePlayer { get; set; } = PlayerColor.White;
    public GameState BoardState { get; set; } = GameState.WaitForAction;

    GameManager _gameManager;
    BoardController _bc;
    int _boardSize;
    bool _useAutoOpponent= false;

    public GameLogic(BoardController boardController, GameManager gameManager, int boardSize, PlayerColor StartPlayer, bool useAutoOpponent)
    {
        _gameManager = gameManager;
        _bc = boardController;
        _boardSize = boardSize;
        _useAutoOpponent = useAutoOpponent;
        ActivePlayer = StartPlayer;
    }

    /// <returns>Return list of all possible paths to move </returns>
    public List<Path> GetMoveablePaths(Piece piece)
    {
        if (piece.IsSuperPiece)
        {
            return GetSuperPiecePaths(piece);
        }
        else
        {
            return GetPaths(piece.Color, piece.GetPostion());
        }
    }

    List<Path> GetPaths(PlayerColor playerColor, Vector2Int fromPos,Path previousPath = null, List<Path> allPaths = null)
    {


        allPaths = allPaths == null ? new List<Path>() : allPaths;

        int col = fromPos.y;
        int row = fromPos.x;

        // Same logic for both players by swapping the operators
        // This section write from the white perspective
        bool isBlack = playerColor == PlayerColor.Black;
        var operatorsSet = GetOperatorSet(isBlack);

        var Add = operatorsSet.add;
        var Sub = operatorsSet.sub;


        // Forwoard Right
        // pos1 - one step to the right
        // pos2 - two steps to the right
        Vector2Int pos1 = new Vector2Int(Add(row, 1), Add(col, 1));
        Vector2Int pos2 = new Vector2Int(Add(row, 2), Add(col, 2));

        if (IsPosInBoard(pos1))
        {
            var rightPath = previousPath == null ? new Path() : new Path(previousPath);

            // Move
            if (_bc.GetPieceAtPos(pos1) == null && previousPath == null)
            {
                
                rightPath.moveableSquares.Add(_bc.GetSquareAtPos(pos1));
                allPaths.Add(rightPath);
            }

            // Eat opponent
            else
            if (IsPosInBoard(pos2) &&
                _bc.GetPieceAtPos(pos2) == null &&
                _bc.GetPieceAtPos(pos1) != null &&
                _bc.GetPieceAtPos(pos1).Color != playerColor)
            {

                rightPath.eatablePieces.Add(_bc.GetPieceAtPos(pos1));
                rightPath.moveableSquares.Add(_bc.GetSquareAtPos(pos2));

                allPaths.Add(rightPath);

                GetPaths(playerColor, new Vector2Int(Add(row, 2), Add(col, 2)), rightPath, allPaths); ;
            }

        }


        // Forwoard Left
        pos1 = new Vector2Int(Add(row, 1), Sub(col, 1));
        pos2 = new Vector2Int(Add(row, 2), Sub(col, 2));

        if (IsPosInBoard(pos1))
        {
            var leftPath = previousPath == null ? new Path() : new Path(previousPath);

            // Move
            if (_bc.GetPieceAtPos(pos1) == null && previousPath == null)
            {
                leftPath.moveableSquares.Add(_bc.GetSquareAtPos(pos1));
                allPaths.Add(leftPath);
            }

            // Eat opponent
            else
            if (IsPosInBoard(pos2) &&
                _bc.GetPieceAtPos(pos2) == null &&
                _bc.GetPieceAtPos(pos1) != null &&
                _bc.GetPieceAtPos(pos1).Color != playerColor)
            {

                leftPath.eatablePieces.Add(_bc.GetPieceAtPos(pos1));
                leftPath.moveableSquares.Add(_bc.GetSquareAtPos(pos2));
                allPaths.Add(leftPath);

                GetPaths(playerColor, new Vector2Int(Add(row, 2), Sub(col, 2)), leftPath, allPaths);
            }
        }


        return allPaths;
    }

    internal void DestroyGame()
    {
        _bc.DestroyBoard();
    }

    List<Path> GetSuperPiecePaths(Piece piece)
    {
        var allPaths = new List<Path>();
        var directions = new List<Direction> { Direction.Left, Direction.Right };
        var operatorsSets = new List<Operators>() { GetOperatorSet(), GetOperatorSet(true) };

        Vector2Int pos = Vector2Int.zero;
        Vector2Int pos1 = Vector2Int.zero; ;
        Vector2Int pos2 = Vector2Int.zero; ;

        foreach (var set in operatorsSets)
        {
            Func<int, int, int> add = set.add;
            Func<int, int, int> sub = set.sub;


            foreach (var direction in directions)
            {

                // pos - piece posion
                // pos1 - 1 step to the dir
                // pos2 - 2 steps to the dir

                if (direction == Direction.Left)
                {
                    // Forwoard Right -------------> 
                    pos = new Vector2Int(piece.Row, piece.Col);
                    pos1 = new Vector2Int(add(piece.Row, 1), add(piece.Col, 1));
                    pos2 = new Vector2Int(add(piece.Row, 2), add(piece.Col, 2));
                }
                if (direction == Direction.Right)
                {
                    // <------------ Forwoard Left
                    pos = new Vector2Int(piece.Row, piece.Col);
                    pos1 = new Vector2Int(add(piece.Row, 1), sub(piece.Col, 1));
                    pos2 = new Vector2Int(add(piece.Row, 2), sub(piece.Col, 2));
                }

                Path prevPath = null;

                while (IsPosInBoard(pos1))
                {

                    if (_bc.GetPieceAtPos(pos1) == null)
                    {
                        var path = new Path(prevPath);
                        path.moveableSquares.Add(_bc.GetSquareAtPos(pos1));
                        allPaths.Add(path);

                        if (prevPath != null)
                        {
                            TryAddAnglePaths(prevPath, pos1, direction, piece.Color, allPaths);
                        }
                    }

                    else
                    if (_bc.GetPieceAtPos(pos1).Color == piece.Color)
                    {
                        break;
                    }

                    else
                    if (IsPosInBoard(pos2) &&
                       _bc.GetPieceAtPos(pos2) == null)
                    {

                        //EAT
                        var path = new Path(prevPath);
                        path.moveableSquares.Add(_bc.GetSquareAtPos(pos));
                        path.moveableSquares.Add(_bc.GetSquareAtPos(pos2));
                        path.eatablePieces.Add(_bc.GetPieceAtPos(pos1));

                        allPaths.Add(path);
                        prevPath = path;
                    }

                    else
                    {
                        break;
                    }


                    if (direction == Direction.Left)
                    {
                        pos = new Vector2Int(add(pos.x, 1), add(pos.y, 1));
                        pos1 = new Vector2Int(add(pos1.x, 1), add(pos1.y, 1));
                        pos2 = new Vector2Int(add(pos2.x, 1), add(pos2.y, 1));
                    }
                    if (direction == Direction.Right)
                    {
                        pos = new Vector2Int(add(pos.x, 1), sub(pos.y, 1));
                        pos1 = new Vector2Int(add(pos1.x, 1), sub(pos1.y, 1));
                        pos2 = new Vector2Int(add(pos2.x, 1), sub(pos2.y, 1));
                    }

                }


            }
        }




        return allPaths;
    }

    void TryAddAnglePaths(Path prevPath, Vector2Int startPostion, Direction dir, PlayerColor playerColor, List<Path> paths)
    {
        var operatorsSets = new List<Operators>() { GetOperatorSet(), GetOperatorSet(true) };

        foreach (var set in operatorsSets)
        {
            Func<int, int, int> add = set.add;
            Func<int, int, int> sub = set.sub;

            bool allReadyEat = false;
            Path upPrevPath = prevPath;
            Vector2Int pos = pos = startPostion;
            Vector2Int pos1 = Vector2Int.one;
            Vector2Int pos2 = Vector2Int.one;

            if (dir == Direction.Left)
            {
                pos = startPostion;
                pos1 = new Vector2Int(add(pos.x, 1), sub(pos.y, 1));
                pos2 = new Vector2Int(add(pos.x, 2), sub(pos.y, 2));
            }

            if (dir == Direction.Right)
            {
                pos = startPostion;
                pos1 = new Vector2Int(add(pos.x, 1), add(pos.y, 1));
                pos2 = new Vector2Int(add(pos.x, 2), add(pos.y, 2));
            }

            while (IsPosInBoard(pos1))
            {
                if (_bc.GetPieceAtPos(pos1) == null && allReadyEat)
                {
                    var newPath = new Path(upPrevPath);
                    newPath.moveableSquares.Add(_bc.GetSquareAtPos(pos1));
                    paths.Add(newPath);

                    var opsitSide = dir == Direction.Left ? Direction.Right : Direction.Left;
                    TryAddAnglePaths(upPrevPath, pos1, opsitSide, playerColor, paths);
                }

                else
                if (IsPosInBoard(pos2) &&
                   _bc.GetPieceAtPos(pos1) != null &&
                   _bc.GetPieceAtPos(pos1).Color != playerColor &&
                   _bc.GetPieceAtPos(pos2) == null)
                {
                    allReadyEat = true;

                    var newPath = new Path(upPrevPath);
                    newPath.moveableSquares.Add(_bc.GetSquareAtPos(pos));
                    newPath.moveableSquares.Add(_bc.GetSquareAtPos(pos2));
                    newPath.eatablePieces.Add(_bc.GetPieceAtPos(pos1));
                    upPrevPath = newPath;

                    paths.Add(newPath);
                }


                if (dir == Direction.Left)
                {
                    pos = new Vector2Int(add(pos.x, 1), sub(pos.y, 1));
                    pos1 = new Vector2Int(add(pos1.x, 1), sub(pos1.y, 1));
                    pos2 = new Vector2Int(add(pos2.x, 1), sub(pos2.y, 1));
                }
                if (dir == Direction.Right)
                {
                    pos = new Vector2Int(add(pos.x, 1), add(pos.y, 1));
                    pos1 = new Vector2Int(add(pos1.x, 1), add(pos1.y, 1));
                    pos2 = new Vector2Int(add(pos2.x, 1), add(pos2.y, 1));
                }
            }

        }
    }

    async Task AutoOpponentAsync()
    {

        var opponentPieces = _bc.GetAllPieceInColor(PlayerColor.Black);

        Piece selectedPiece = null;
        Path bestPath = null;

        bool isFirst = true;

        var rand = UnityEngine.Random.Range(0, opponentPieces.Count);

        for (int i = 0; i < opponentPieces.Count; i++)
        {
            var randIndex = (i + rand) % opponentPieces.Count;
            var piece = opponentPieces[randIndex];

            var paths = GetPaths(piece.Color, piece.GetPostion());
            foreach (var path in paths)
            {
                if (isFirst)
                {
                    selectedPiece = piece;
                    bestPath = path;
                    isFirst = false;
                }

                if (path.eatablePieces.Count > bestPath.eatablePieces.Count)
                {
                    selectedPiece = piece;
                    bestPath = path;
                }
            }
        }

        await _bc.MovePieceInPath(selectedPiece, bestPath);

    }

    internal bool IsWin(PlayerColor playerColor)
    {
        var opponentColor = playerColor == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;

        var opponentPieces = _bc.GetAllPieceInColor(opponentColor);
        if(opponentPieces.Count == 0)
        {
            return true;
        }

        return false;
    }

    internal bool IsNeedBecomeSuperPiece(Piece gamePiece)
    {
        if (gamePiece.Row == 0 && gamePiece.Color == PlayerColor.Black)
        {
            return true;
        }
        else if (gamePiece.Row == _boardSize - 1 && gamePiece.Color == PlayerColor.White)
        {
            return true;
        }

        return false;
    }

    bool IsPosInBoard(Vector2 pos)
    {
        return pos.x >= 0 && pos.x < _boardSize && pos.y >= 0 && pos.y < _boardSize;
    }

    internal void NextPlayer()
    {

        _bc.DeselectPiece();

        ActivePlayer = ActivePlayer == PlayerColor.Black ? PlayerColor.White : PlayerColor.Black;


        _gameManager.SetActivePlayer(ActivePlayer);


        if (ActivePlayer == PlayerColor.Black && _useAutoOpponent)
        {

            _ = AutoOpponentAsync();
        }
        else
        {
            BoardState = GameState.WaitForAction;
        }

    }

    Operators GetOperatorSet(bool swap = false)
    {
        var opSET = new Operators();



        Func<int, int, int> add = (x, y) => { return x + y; };
        Func<int, int, int> sub = (x, y) => { return x - y; };
        Func<int, int, bool> greaterThan = (x, y) => { return x > y; };

        if (swap)
        {
            opSET.add = sub;
            opSET.sub = add;
            opSET.GreaterThan = (x, y) => { return x <= y; };
        }
        else
        {
            opSET.add = add;
            opSET.sub = sub;
            opSET.GreaterThan = (x, y) => { return x > y; };
        }

        return opSET;
    }
}
