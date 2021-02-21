using System.Collections.Generic;

public partial class BoardController
{
    private class Path
    {
        public List<Square> moveableSquares = new List<Square>();
        public List<Piece> eatablePieces = new List<Piece>();

        public Path(Path previousPath =null)
        {
            if(previousPath != null)
            {
                this.moveableSquares.AddRange(previousPath.moveableSquares);
                this.eatablePieces.AddRange(previousPath.eatablePieces);

            }

        }
    }
}

