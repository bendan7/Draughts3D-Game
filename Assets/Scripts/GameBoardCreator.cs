﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[ExecuteInEditMode]
public class GameBoardCreator : MonoBehaviour
{
    public int BoardSize = 8;
    public GameObject Square;
    public GameObject GamePiecePrefab;

    public Material BoardBlack;
    public Material BoardWhite;

    public Material PieceBlack;
    public Material PieceWhite;

    public Material SelectedBlack;
    public Material SelectedWhite;
    public Material Eatable;

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


    public (Square[,], GamePiece[,]) BuildNewGameBoard() {

        Debug.Log("New Game Board");

        _board = new GameObject("Board");
        _gamePieces = new GameObject("GamePieces");

        Square[,] boardArr = new Square[BoardSize, BoardSize];
        GamePiece[,] gamePiecesArr = new GamePiece[BoardSize, BoardSize];


        bool isBlackCell = false;

        for (int i = 0; i < BoardSize; i++)
        {
            isBlackCell = !isBlackCell;

            for (int j = 0; j < BoardSize; j++)
            {
                var square = Instantiate(Square, _board.transform);

                square.name = $"{i}:{j}";
                square.transform.position = new Vector3(j, 0, i);

                var squareScript = square.GetComponent<Square>();
                squareScript.Row = i;
                squareScript.Col = j;
                boardArr[i,j] = squareScript;


                if (isBlackCell)
                {

                    square.GetComponentInChildren<MeshRenderer>().material = BoardBlack;
                }
                else
                {
                    square.GetComponentInChildren<MeshRenderer>().material = BoardWhite;
                }


                if (i < 3 && isBlackCell)
                {
                    var gamePiece = AddPiece(PlayerColor.White, j,i);
                    gamePiecesArr[i, j] = gamePiece;
                }
                else if (i > 4 && isBlackCell)
                {
                    var gamePiece = AddPiece(PlayerColor.Black,  j,i);
                    gamePiecesArr[i, j] = gamePiece;
                }



                isBlackCell = !isBlackCell;
            }
        }


        _board.transform.position =new  Vector3(0, -0.05f, 0);

        
        return (boardArr, gamePiecesArr);
    }

    private GamePiece AddPiece(PlayerColor color, int i, int j)
    {

        var GamePiece = Instantiate(GamePiecePrefab, _gamePieces.transform);

        GamePiece.name = $"{j}:{i} - {color}";
        GamePiece.transform.position = new Vector3(i, 0, j);
        GamePiece.GetComponentInChildren<MeshRenderer>().material = color == PlayerColor.White ? PieceWhite : PieceBlack;


        var selectedMat = color == PlayerColor.White ? SelectedWhite : SelectedBlack;
        var GamePieceScript = GamePiece.GetComponent<GamePiece>();

        GamePieceScript.SelectedMaterial = selectedMat;
        GamePieceScript.Eatable = Eatable;
        GamePieceScript.Color = color;

        GamePieceScript.Row = j;
        GamePieceScript.Col = i;

        GamePiece.SetActive(true);

        return GamePieceScript;
    }
}
