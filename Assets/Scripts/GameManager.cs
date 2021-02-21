using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{

    public GameUIController gameUIController;
    public PlayerColor ActivePlayer = PlayerColor.White;

    void Start()
    {
        gameUIController?.StartNewGame(ActivePlayer);
    }

    public PlayerColor NextPlayer()
    {
        ActivePlayer = ActivePlayer == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;
        gameUIController?.UpdateActivePlayer(ActivePlayer);
        return ActivePlayer;
    }

    public void ExitButtonClicked()
    {
        Debug.Log("GameExit");
    }

    void Update()
    {

    }
}
