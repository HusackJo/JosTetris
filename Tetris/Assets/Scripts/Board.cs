using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tileMap { get; private set; }
    public Piece activePiece { get; private set; }
    public TetrominoData[] tetrominos;

    public Vector3Int spawnPos;

    public Vector2Int boardSize;

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-this.boardSize.x/2, -this.boardSize.y/2);
            return new RectInt(position, this.boardSize);
        }
    }

    private void Awake()
    {
        tileMap = GetComponentInChildren<Tilemap>();
        this.activePiece = GetComponentInChildren<Piece>();
        for (int i = 0; i < this.tetrominos.Length; i++)
        {
            this.tetrominos.Initialize();
        }
    }

    private void Start()
    {
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        int random = UnityEngine.Random.Range(0,tetrominos.Length);
        TetrominoData data = tetrominos[random];
        data.Initialize();

        //print($"Spawning piece: {data.tetromino} It's cells are: {data.cells.ToCommaSeparatedString()}");

        this.activePiece.Initialize(this, spawnPos, data);
        if (IsValidPosition(this.activePiece, spawnPos))
        {
            SetPiece(this.activePiece);
        }
        else
        {
            //GM.GameOver
        }
    }

    public void SetPiece(Piece piece)
    {
        //print(piece.cells.Length);
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;

            //print("tile spawned at: " + piece.cells[i].ToString());

            this.tileMap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void ClearPiece(Piece piece)
    {
        //print(piece.cells.Length);
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;

            this.tileMap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = this.Bounds;

        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }

            if (this.tileMap.HasTile(tilePosition))
            {
                return false;
            }
        }
        return true;
    }

    public void ClearLines()
    {
        int row = this.Bounds.yMin;
        while (row < this.Bounds.yMax)
        {
            if (IsLineFull(row))
            {
                LineClear(row);
                //increment counter
            } else  row++; 
        }
        //run scoring function based on lines cleared
        //GM.AddScore(linesClearCounter)
    }

    private void LineClear(int row)
    {
        for (int col = this.Bounds.xMin; col < Bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            this.tileMap.SetTile(position, null);
        }
        while (row < this.Bounds.yMax)
        {
            for(int col = this.Bounds.xMin; col < Bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row, 0);
                TileBase above = this.tileMap.GetTile(new Vector3Int(col, row + 1, 0));

                this.tileMap.SetTile(position, above);
            }
            row++;
        }
    }

    private bool IsLineFull(int row)
    {
        for(int col = this.Bounds.xMin; col < Bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            if (!this.tileMap.HasTile(position))
            {
                return false;
            }
        }
        return true;
    }
}
