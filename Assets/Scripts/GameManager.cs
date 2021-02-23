using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using System;

public class GameManager : MonoBehaviour
{

    public GameObject MainMenu;
    public GameObject SettingsMenu;
    public GameObject CreditMenu;

    private BoardCreator _boardCreator;
    private GameLogic _gameLogic;
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

    }

    public void MoveToSettingsMenu()
    {
        MainMenu.SetActive(false);
        CreditMenu.SetActive(false);
        SettingsMenu.SetActive(true);
    }

    public void MoveToCreditMenu()
    {
        MainMenu.SetActive(false);
        SettingsMenu.SetActive(false);
        CreditMenu.SetActive(true);
    }

    public async void StartNewGameAsync()
    {
        MainMenu.SetActive(false);

        _gameLogic = _boardCreator.BuildNewGameBoard();
        
        await _cameraController.PlayIntroEffect(_boardCreator.BoardSize);

        _GameUI.StartNewGame(_gameLogic.ActivePlayer);
    }

    public void SetActivePlayer(PlayerColor activePlayer)
    {
        _GameUI?.SetActivePlayer(activePlayer);
    }

    public async void GameExitAsync()
    {
        _GameUI.GameEnd();
        await _cameraController.PlayOutroEffect();

        MainMenu.SetActive(true);

        _gameLogic.DestroyGame();
    }

    public void ReturnToMainMenu()
    {
        CreditMenu.SetActive(false);
        SettingsMenu.SetActive(false);
        MainMenu.SetActive(true);
    }

    public void AppExit()
    {
        Application.Quit();
    }

    public void OnCreditClick()
    {
        Debug.Log("Open link");
        Application.OpenURL("https://www.linkedin.com/in/bendan7/");
    }

    internal void SetWinner(PlayerColor color)
    {
        _GameUI.SetWinner(color);
    }
}
