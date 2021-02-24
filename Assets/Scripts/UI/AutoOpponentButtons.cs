using UnityEngine;
using System.Collections;

public class AutoOpponentButtons: MonoBehaviour
{

    public GameObject YesButt;
    public GameObject NoButt;

    private Color32 SelectedColor;

    BoardCreator _boardCreator;

    TMPro.TextMeshProUGUI _yesButt;
    TMPro.TextMeshProUGUI _noButt;


    private void Awake()
    {
        _yesButt = YesButt.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        _noButt = NoButt.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        SelectedColor = _yesButt.color;
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
        MarkSelectedOption(_boardCreator.UseAutoOpponent);
    }

    private void MarkSelectedOption(bool useAutoOpponent)
    {
        if (useAutoOpponent)
        {
            _noButt.color = Color.white;
            _yesButt.color = SelectedColor;
        }
        else
        {
            _noButt.color = SelectedColor;
            _yesButt.color = Color.white;
        }


    }

    public void SetAutoOpponent(bool useAutoOpponent)
    {
        MarkSelectedOption(useAutoOpponent);
        _boardCreator.UseAutoOpponent = useAutoOpponent;

    }


}
