using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    GamePiece _selectedGP = null;
    PlayerColor activePlayer = PlayerColor.White;

    void Start()
    {
        
    }




    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.transform.parent.tag == "GamePiece")
                {



                    var gamePiece = hit.collider.gameObject.transform.parent.gameObject.GetComponentInChildren<GamePiece>();

                    if(gamePiece.Color == activePlayer)
                    {
                        _selectedGP?.OnDeselect();
                        _selectedGP = gamePiece;
                        _selectedGP.OnSelected();
                    }

   
                }
            }
        }

    }
}

