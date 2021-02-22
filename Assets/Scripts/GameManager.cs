using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{

    private BoardCreator _boardCreator;
    private BoardController _boardController;
    private GameUI _GameUI;
    

    private void Awake()
    {
        StartNewGame();
    }

    private void StartNewGame()
    {
        _boardCreator = FindObjectOfType<BoardCreator>();
        _boardController = _boardCreator.BuildNewGameBoard();

        _GameUI = FindObjectOfType<GameUI>();
        if(_GameUI == null)
        {
            Debug.LogWarning("GameUI doesn't found");
        }

        _GameUI?.SetActivePlayer(_boardController.ActivePlayer);
        _GameUI?.StopwatchStart();
    }

    public void SetActivePlayer(PlayerColor activePlayer)
    {
        _GameUI?.SetActivePlayer(activePlayer);
    }

    public void ExitButtonClicked()
    {
        Debug.Log("GameExit");
    }

}
