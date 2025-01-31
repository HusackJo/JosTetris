using UnityEngine;
using UnityEngine.Tilemaps;
using static Board;

public class Ghost : MonoBehaviour
{
    public Tile tile;
    public Board board;
    public Piece trackingPiece;
    
    public Tilemap tilemap { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        this.cells = new Vector3Int[6];
    }

    private void LateUpdate()
    {
        Clear();
        Copy();
        Drop();
        Set();
    }

    private void Clear()
    {
        for (int i = 0; i < trackingPiece.cells.Length; i++)
        {
            Vector3Int tilePosition = trackingPiece.cells[i] + trackingPiece.position;
            this.tilemap.ClearAllTiles();
        }
    }
    private void Copy()
    {
        for (int i = 0; i < trackingPiece.cells.Length; i++)
        {
            this.cells[i] = trackingPiece.cells[i];
        }
    }
    private void Drop()
    {
        Vector3Int dropPosition = this.trackingPiece.position;

        int currentRow = dropPosition.y;
        int bottom = (-this.board.boardSize.y / 2) - 1;

        this.board.ClearPiece(this.trackingPiece);

        for (int row = currentRow; row >= bottom; row--)
        {
            dropPosition.y = row;

            if (this.board.IsValidPosition(this.trackingPiece, dropPosition))
            {
                this.position = dropPosition;
            }
            else break;
        }

        this.board.SetPiece(this.trackingPiece);
    }
    private void Set()
    {
        if (board.gameState == GameState.Playing)
        {
            for (int i = 0; i < this.cells.Length; i++)
            {
                Vector3Int tilePosition = this.cells[i] + this.position;


                this.tilemap.SetTile(tilePosition, this.tile);
            }
        }
    }
}
