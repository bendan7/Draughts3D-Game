using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    GamePiece _selectedGP = null;

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
                    _selectedGP?.OnDeselect();
                    var gamePiece = hit.collider.gameObject.transform.parent.gameObject;

                    _selectedGP = gamePiece.GetComponentInChildren<GamePiece>();
                    _selectedGP.OnSelected();
                }
            }
        }

    }
}

