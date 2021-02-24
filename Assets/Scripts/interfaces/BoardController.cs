using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface BoardController
{
    Piece GetPieceAtPos(Vector2Int pos);
    Square GetSquareAtPos(Vector2Int pos);
    Task MovePieceInPath(Piece gamePiece, Path path);
    void DeselectPiece();
    List<Piece> GetAllPieceInColor(PlayerColor black);
    void DestroyBoard();
}