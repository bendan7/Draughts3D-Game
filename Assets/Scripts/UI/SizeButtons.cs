using UnityEngine;
using System.Collections;

public class SizeButtons : MonoBehaviour
{

    public GameObject Button8;
    public GameObject Button10;
    public GameObject Button12;

    Color32 SelectedColor;

    BoardCreator _boardCreator;

    TMPro.TextMeshProUGUI _button8;
    TMPro.TextMeshProUGUI _button10;
    TMPro.TextMeshProUGUI _button12;



    private void Awake()
    {
        _button8 = Button8.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        _button10 = Button10.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        _button12 = Button12.GetComponentInChildren<TMPro.TextMeshProUGUI>();

        SelectedColor = _button8.color;


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
        MarkSelectedSize(_boardCreator.BoardSize);
    }


    public void ChangeBoardSize(int newSize)
    {
        Debug.Log($"Change Board Size to {newSize}");
        _boardCreator.BoardSize = newSize;
        MarkSelectedSize(newSize);
    }


    void MarkSelectedSize(int size)
    {
        _button8.color = Color.white;
        _button10.color = Color.white;
        _button12.color = Color.white;

        switch (size)
        {
            case 8:
                _button8.color = SelectedColor;
                break;
            case 10:
                _button10.color = SelectedColor;
                break;
            case 12:
                _button12.color = SelectedColor;
                break;
            default:
                break;
        }
    }


}
