using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{

    private GameUI _GameUI;
    private BoardController _boardController;

    void Start()
    {
        _boardController = FindObjectOfType<BoardController>();
        _GameUI = FindObjectOfType<GameUI>();

        _GameUI?.StartNewGame(_boardController.ActivePlayer);
    }

    public void NextPlayer(PlayerColor activePlayer)
    {
        _GameUI.UpdateActivePlayer(activePlayer);
    }

    public void ExitButtonClicked()
    {
        Debug.Log("GameExit");
    }

    void Update()
    {

    }
}
