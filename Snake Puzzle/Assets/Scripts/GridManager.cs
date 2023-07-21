using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private NormalTile _normalTile;
    [SerializeField] private WallTile _wallTile;
    [SerializeField] private Transform _cam;

    private const string _playerAbstract = "p";
    private const string _normalTileAbstract = " ";
    private const string _normalTilePaintedAbstract = "x";
    private const string _wallTileAbstract = "w";


    private Player player;
    private GameState currentState;
    private List<GameState> _gameStateManager = new List<GameState>();

    void Start()
    {
        GameState state = GameState.GetInitialGameSTate(1);
        GenerateGrid(state);

        // initial log
        currentState = state;
        //_gameStateManager.Add(state);

    }

    void Update()
    {
        
    }

    void GenerateGrid(GameState state)
    {
        int _height = state.GetHeight();
        int _width = state.GetWidth();

        // generating tiles
        for (int x = _width - 1; x >= 0; --x)
        {
            for (int y = 0; y < _height; ++y)
            {
                string objectInPosition = state.GetObjectAt(x, y);
                if (currentState != null && currentState.GetObjectAt(x, y) != state.GetObjectAt(x, y))
                {
                    DestroyAffactedTile(x, y);
                    SetObjectInPosition(objectInPosition, x, y);
                }
                else if (currentState == null)
                    SetObjectInPosition(objectInPosition, x, y);
            }
        }

        // adjusting camera
        _cam.transform.position = new Vector3((float)_width / 2 - .5f, (float)_height / 2 - .5f, -10);
    }


    private void SetObjectInPosition(string objectInPosition, int x, int y)
    {
        NormalTile spawnedTile;
        if (objectInPosition == _playerAbstract)
        {
            if (currentState == null)
            {
                player = Instantiate(_player, new Vector3(x, y), Quaternion.identity);
                player.name = "Player";
                player.OnPlayerMoved += Handle_OnPlayerMoved;
                player.OnUndoPressed += Handle_OnUndoPressed;
            }
            else
            {
                player.movePoint.position = new Vector3(x, y);
            }

            spawnedTile = Instantiate(_normalTile, new Vector3(x, y), Quaternion.identity);
            spawnedTile.name = $"NormalTile ({x}, {y})";
            spawnedTile.Paint();
        }
        else if (objectInPosition == _normalTileAbstract)
        {
            spawnedTile = Instantiate(_normalTile, new Vector3(x, y), Quaternion.identity);
            spawnedTile.name = $"NormalTile ({x}, {y})";
        }
        else if (objectInPosition == _normalTilePaintedAbstract)
        {
            NormalTile paintedTile = Instantiate(_normalTile, new Vector3(x, y), Quaternion.identity);
            paintedTile.Paint();
            paintedTile.name = $"NormalTile ({x}, {y})";
        }
        else if (objectInPosition == _wallTileAbstract)
        {
            WallTile wall = Instantiate(_wallTile, new Vector3(x, y), Quaternion.identity);
            wall.name = $"WallTile ({x}, {y})";
        }
    }

    private void Handle_OnPlayerMoved(object sender, Player.OnPlayerMovedArgs arg)
    {
        _gameStateManager.Add(currentState.Copy());
        if (_gameStateManager.Count > 0)
        {
            currentState.GetCurrentState()[(int)arg.previousPosition.x, (int)arg.previousPosition.y] = "x";
            currentState.GetCurrentState()[(int)arg.currentPosition.x, (int)arg.currentPosition.y] = "p";
        }

        //Debug.Log($"{_gameStateManager[_gameStateManager.Count - 1]}");

    }

    public void Handle_OnUndoPressed(object sender, EventArgs e)
    {
        if (_gameStateManager.Count > 0)
        {
            int lastElementIndex = _gameStateManager.Count - 1;

            GameState state = _gameStateManager[lastElementIndex];
            _gameStateManager.RemoveAt(lastElementIndex);
            GenerateGrid(state);

            currentState = state;
        }
            
    }
    public void DestroyPlayer()
    {
        if (GameObject.FindAnyObjectByType<Player>() == null)
        {
            Debug.Log("null");
        }
        // Destroy( GameObject.FindObjectOfType<Player>() );
    }

    public void DestroyAffactedTile(int x, int y)
    {
        Collider2D[] colliders = Physics2D.OverlapPointAll(new Vector3(x, y));
        foreach (Collider2D obj in colliders)
            if (obj != this && obj.name != "Main Camera")
            {
                Debug.Log(obj.name + " destroyed");
                Destroy(obj);
            }
    }

}


// gameState Class
public class GameState
{
    public string[,] _currentState;
    private int _width, _height;

    static string[,] level1 = {
        {"w", "w", "w", "w", "w", "w", "w", "w", "w"},
        {"w", "p", "w", "w", "w", "w", " ", " ", "w"},
        {"w", " ", "w", " ", " ", " ", " ", "w", "w"},
        {"w", " ", "w", " ", " ", " ", " ", " ", "w"},
        {"w", " ", " ", " ", "w", " ", " ", " ", "w"},
        {"w", " ", " ", " ", " ", " ", " ", " ", "w"},
        {"w", "w", "w", "w", "w", "w", "w", "w", "w"},
        };

    public override string ToString()
    {
        string to_return = "{\n";
        for (int x = 0; x < _width; ++x)
        {
            to_return += "{";
            for (int y = 0; y < _height; ++y)
            {
                to_return += " " + _currentState[x, y] + " ";
            }
            to_return += "}\n";
        }
        to_return += "\n}\n";
        return to_return;
    }

    static public GameState GetInitialGameSTate(int level)
    {
        switch (level)
        {
            case 1:
                return new GameState(level1);
        }

        return null;
    }

    public int GetWidth()
    {
        return _width;
    }

    public int GetHeight()
    {
        return _height;
    }

    public string GetObjectAt(int x, int y)
    {
        return _currentState[x, y];
    }

    public string[,] GetCurrentState()
    {
        return _currentState;
    }

    public void UpdateCurrentState()
    {

    }

    public GameState Copy()
    {
        GameState cpy = new GameState(_currentState);
        return cpy;
    }

    public GameState(string[,] gameState)
    {
        _width = gameState.GetLength(0);
        _height = gameState.GetLength(1);
        _currentState = new string[_width, _height];
        Array.Copy(gameState, _currentState, gameState.Length);
    }
}