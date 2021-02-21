using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[ExecuteInEditMode]
public class GameBoardCreator : MonoBehaviour
{
    public int BoardSize = 8;
    public GameObject SquarePrefab;
    public GameObject PiecePrefab;

    public Material SquareBlack;
    public Material SquareWhite;
    public Material SquareMoveable;

    public Material PieceBlack;
    public Material PieceWhite;

    public Material SelectedPieceBlack;
    public Material SelectedPieceWhite;
    public Material EatablePiece;

    private GameObject _board;
    private GameObject _gamePieces;

    private void OnEnable()
    {
        
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
        
        
    }


    public (Square[,], Piece[,]) BuildNewGameBoard() {

        Debug.Log("New Game Board");

        _board = new GameObject("Board");
        _gamePieces = new GameObject("GamePieces");

        Square[,] boardArr = new Square[BoardSize, BoardSize];
        Piece[,] gamePiecesArr = new Piece[BoardSize, BoardSize];


        bool isBlackCell = false;

        for (int i = 0; i < BoardSize; i++)
        {
            isBlackCell = !isBlackCell;

            for (int j = 0; j < BoardSize; j++)
            {

                Color color = isBlackCell ? Color.black : Color.white;
                boardArr[i,j] = InstantiateSquare(i,j, color);


                if (i < (BoardSize-2)/2 && isBlackCell)
                {
                    var gamePiece = InstantiateGamePiece(PlayerColor.White, j,i);
                    gamePiecesArr[i, j] = gamePiece;
                }
                else if (i >= (BoardSize + 2) / 2 && isBlackCell)
                {
                    var gamePiece = InstantiateGamePiece(PlayerColor.Black,  j,i);
                    gamePiecesArr[i, j] = gamePiece;
                }

                isBlackCell = !isBlackCell;
            }
        }


        _board.transform.position =new  Vector3(0, -0.05f, 0);

        
        return (boardArr, gamePiecesArr);
    }

    private Square InstantiateSquare(int i, int j , Color color)
    {

        var square = Instantiate(SquarePrefab, _board.transform);

        square.name = $"{i}:{j}";
        square.transform.position = new Vector3(j, 0, i);

        var squareScript = square.GetComponent<Square>();
        squareScript.Row = i;
        squareScript.Col = j;
        squareScript.SelectedMaterial = SquareMoveable;
        

        if (color == Color.black)
        {
            square.GetComponentInChildren<MeshRenderer>().material = SquareBlack;
        }
        else
        {
            square.GetComponentInChildren<MeshRenderer>().material = SquareWhite;
        }

        return squareScript;
        
    }

    private Piece InstantiateGamePiece(PlayerColor color, int i, int j)
    {

        var GamePiece = Instantiate(PiecePrefab, _gamePieces.transform);

        GamePiece.name = $"{j}:{i} - {color}";
        GamePiece.transform.position = new Vector3(i, 0, j);
        GamePiece.GetComponentInChildren<MeshRenderer>().material = color == PlayerColor.White ? PieceWhite : PieceBlack;


        var selectedMat = color == PlayerColor.White ? SelectedPieceWhite : SelectedPieceBlack;
        var GamePieceScript = GamePiece.GetComponent<Piece>();

        GamePieceScript.SelectedMaterial = selectedMat;
        GamePieceScript.Eatable = EatablePiece;
        GamePieceScript.Color = color;

        GamePieceScript.Row = j;
        GamePieceScript.Col = i;

        GamePiece.SetActive(true);

        return GamePieceScript;
    }
}
