using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using System;

public class GameManager : MonoBehaviour
{

    public GameObject MainMenu;
    
    private BoardCreator _boardCreator;
    private GameBoardController _gameBoardController;
    private GameUI _GameUI;
    private CameraController _cameraController;


    private void Awake()
    {
        _boardCreator = FindObjectOfType<BoardCreator>();
        if (_boardCreator == null)
        {
            Debug.LogWarning("BoardCreator doesn't found");
        }

        _cameraController = FindObjectOfType<CameraController>();
        if (_cameraController == null)
        {
            Debug.LogWarning("CameraController doesn't found");
        }

        _GameUI = FindObjectOfType<GameUI>();
        if (_GameUI == null)
        {
            Debug.LogWarning("GameUI doesn't found");
        }
        _GameUI.gameObject.SetActive(false);
    }

    public async void StartNewGameAsync()
    {
        MainMenu.SetActive(false);

        _gameBoardController = _boardCreator.BuildNewGameBoard();
        _GameUI?.SetActivePlayer(_gameBoardController.ActivePlayer);

        await _cameraController.PlayIntroEffect();

        _GameUI.gameObject.SetActive(true);
        _GameUI?.ClockStart();
    }

    public void SetActivePlayer(PlayerColor activePlayer)
    {
        _GameUI?.SetActivePlayer(activePlayer);
    }

    public async void GameExitAsync()
    {
        _GameUI.gameObject.SetActive(false);
        await _cameraController.PlayOutroEffect();

        MainMenu.SetActive(true);

        Destroy(_gameBoardController);
    }

}
