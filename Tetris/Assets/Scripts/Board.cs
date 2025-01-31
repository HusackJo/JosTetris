using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public enum GameState
    {
        Start,
        Playing,
        FixedSequencePlaying,
        Game_Over,
    }

    public Tilemap tileMap { get; private set; }
    public Piece activePiece { get; private set; }
    public GameState gameState { get; private set; }
    public TetrominoData[] tetrominos;
    public TetrominoData[] specialTetrominoSequence;
    public int tetrominosPlaced;

    public Vector3Int spawnPos;

    public Vector2Int boardSize;

    public TetrisManager tetrisManager;

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
        gameState = GameState.Start;
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
        TetrominoData data = tetrominos[0];
        if (this.gameState == GameState.FixedSequencePlaying)
        {
            print($"Fixed Sequence - piece No: {tetrominosPlaced}");
            if (tetrominosPlaced < specialTetrominoSequence.Length)
            {
                data = specialTetrominoSequence[tetrominosPlaced];
                print($"assigning piece: {data.tetromino}");
            }
        }
        else
        {
            data = GetRandomTetromino();
        }
        data.Initialize();

        //Debug.Log($"Spawning piece: {data.tetromino} It's cells are: {data.cells.ToCommaSeparatedString()}");

        tetrominosPlaced++;
        this.activePiece.Initialize(this, spawnPos, data);
        if (IsValidPosition(this.activePiece, spawnPos))
        {
            SetPiece(this.activePiece);
        }
        else
        {
            tetrisManager.GameOver();
        }
    }

    private TetrominoData GetRandomTetromino()
    {
        int random = UnityEngine.Random.Range(0, tetrominos.Length);
        return tetrominos[random];
    }

    public void SetPiece(Piece piece)
    {
        if (gameState == GameState.Playing || gameState == GameState.FixedSequencePlaying)
        {
            for (int i = 0; i < piece.cells.Length; i++)
            {
                Vector3Int tilePosition = piece.cells[i] + piece.position;
                this.tileMap.SetTile(tilePosition, piece.data.tile);
            }
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

    public void ClearAllLines()
    {
        this.tileMap.ClearAllTiles();
    }

    public void StartSpawning()
    {
        gameState = GameState.Playing;
    }
    public void StartSpawningSpecialSequence()
    {
        ClearPiece(activePiece);
        gameState = GameState.FixedSequencePlaying;
        tetrominosPlaced = 0;
        SpawnPiece();
    }
    public void StopSpawning()
    {
        gameState = GameState.Game_Over;
    }
}
