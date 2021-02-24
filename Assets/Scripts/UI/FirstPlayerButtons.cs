using UnityEngine;
using System.Collections;
using System;

public class FirstPlayerButtons : MonoBehaviour
{

    public GameObject ButtonWhite;
    public GameObject ButtonBlack;

    private Color32 SelectedColor;

    BoardCreator _boardCreator;

    TMPro.TextMeshProUGUI _buttonWhite;
    TMPro.TextMeshProUGUI _buttonBlack;


    private void Awake()
    {
        _buttonWhite = ButtonWhite.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        _buttonBlack = ButtonBlack.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        SelectedColor = _buttonWhite.color;
    }

    private void OnEnable()
    {
        _boardCreator = FindObjectOfType<BoardCreator>();

        if (_boardCreator == null)
        {
            Debug.LogWarning("BoardCreator doesn't found");
        }
    }

    void Start()
    {
        MarkSelectedStartColor(_boardCreator.StartPlayer);
    }

    private void MarkSelectedStartColor(PlayerColor startPlayer)
    {
        if(startPlayer == PlayerColor.White)
        {
            _buttonBlack.color = Color.white;
            _buttonWhite.color = SelectedColor;
        }
        else
        {
            _buttonBlack.color = SelectedColor;
            _buttonWhite.color = Color.white; 
        }


    }

    public void ChangeStartColor(string newColor)
    {
        

        if(Enum.TryParse<PlayerColor>(newColor, out var color))
        {

            MarkSelectedStartColor(color);
            _boardCreator.StartPlayer = color;


        }
    }

}
