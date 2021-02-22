using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[ExecuteInEditMode]
public class BoardCreator : MonoBehaviour
{
    public int BoardSize = 8;

    [SerializeField] private GameObject SquarePrefab;
    [SerializeField] private GameObject PiecePrefab;
    [SerializeField] private Material SquareBlack;
    [SerializeField] private Material SquareWhite;
    [SerializeField] private Material SquareMoveableMat;
    [SerializeField] private Material PieceBlack;
    [SerializeField] private Material PieceWhite;
    [SerializeField] private Material SelectedPieceBlack;
    [SerializeField] private Material SelectedPieceWhite;
    [SerializeField] private Material EatablePieceMat;

    private GameObject _board;
    private GameObject _squares;
    private GameObject _pieces;

    private void OnEnable()
    {
        /*
        var Board = GameObject.Find("Board");
        var GamePieces = GameObject.Find("GamePieces");

        if (Board && GamePieces)
        {
            DestroyImmediate(Board);
            DestroyImmediate(GamePieces);
        }


        if (!Application.isPlaying )
        {
            Debug.Log("editor mode");
            BuildNewGameBoard();
        }
        else
        {

            Debug.Log("play mode");
            Board = GameObject.Find("Board");
            GamePieces = GameObject.Find("GamePieces");

            
            if (Board && GamePieces)
            {
                DestroyImmediate(Board);
                DestroyImmediate(GamePieces);
            }
        }
        */
        
    }

    public BoardController BuildNewGameBoard() {

        Debug.Log("New Board");

        _board = new GameObject("GameBoard");
        _squares = new GameObject("Squares");
        _pieces = new GameObject("Pieces");

        _squares.transform.parent = _board.transform;
        _pieces.transform.parent = _board.transform;

        Square[,] squaresArr = new Square[BoardSize, BoardSize];
        Piece[,] piecesArr = new Piece[BoardSize, BoardSize];


        bool isBlackCell = false;

        for (int row = 0; row < BoardSize; row++)
        {
            isBlackCell = !isBlackCell;

            for (int col = 0; col < BoardSize; col++)
            {
                Color color = isBlackCell ? Color.black : Color.white;
                squaresArr[row,col] = InstantiateSquare(row,col, color);


                if (row < (BoardSize-2)/2 && isBlackCell)
                {
                    var gamePiece = InstantiateGamePiece(PlayerColor.White, row,col);
                    piecesArr[row, col] = gamePiece;
                }
                else if (row >= (BoardSize + 2) / 2 && isBlackCell)
                {
                    var gamePiece = InstantiateGamePiece(PlayerColor.Black,  row,col);
                    piecesArr[row, col] = gamePiece;
                }

                isBlackCell = !isBlackCell;
            }
        }


        _squares.transform.position =new  Vector3(0, -0.05f, 0);

        
        var controller = _board.AddComponent<BoardController>();

        controller.Init(BoardSize, squaresArr, piecesArr);

        return controller;
    }

    private Square InstantiateSquare( int row, int col , Color color)
    {

        var square = Instantiate(SquarePrefab, _squares.transform);
        var squareScript = square.GetComponent<Square>();
        var squareMat = color == Color.black ? SquareBlack : SquareWhite;

        squareScript.Init(row, col, squareMat, SquareMoveableMat);

        return squareScript;
        
    }

    private Piece InstantiateGamePiece(PlayerColor color, int row, int col )
    {

        var GamePiece = Instantiate(PiecePrefab, _pieces.transform);

        var material = color == PlayerColor.White ? PieceWhite : PieceBlack;
        var selectedMat = color == PlayerColor.White ? SelectedPieceWhite : SelectedPieceBlack;
        var PieceScript = GamePiece.GetComponent<Piece>();

        PieceScript.Init(row,col, color, material, selectedMat, EatablePieceMat);

        return PieceScript;
    }
}
