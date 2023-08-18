using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// Purpose is to handle unit selection
/// need a reference to camera
/// units that can be managed.
/// will need mouse select so mouse drag, stay dragged etc.
/// </summary>
public class UnitSelectionHandler : MonoBehaviour
{
    [SerializeField] private RectTransform unitSelectionArea = null;
    
    [SerializeField] private LayerMask layerMask = new LayerMask();

    private Vector2 startPosition; 


    private RTSPlayer player;
    private Camera mainCamera; // for refrencing mouse pos

    public List<Unit> selectedUnits { get; } = new List<Unit>();
   private void Start()
    {
        mainCamera = Camera.main;
        
    }

    private void Update()
    {
        if(player == null)
        {
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        }
        if (Mouse.current.leftButton.wasPressedThisFrame) 
        {
            // we wan tto start selection area. 
            StartSelectionArea();
            
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            UpdateSelectionArea();
        }
    }

    private void StartSelectionArea()
    {
        if (!Keyboard.current.leftShiftKey.isPressed)
        {
            foreach (Unit selectedUnit in selectedUnits) // each unit thats been selected invoke select method
            {
                selectedUnit.Deselect();
            }
            selectedUnits.Clear();
        }
       
        unitSelectionArea.gameObject.SetActive(true);

        startPosition = Mouse.current.position.ReadValue();

        UpdateSelectionArea();
    }
    private void UpdateSelectionArea()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        //create corners of box 
        float areaWidth = mousePosition.x - startPosition.x;
        float areaHeight = mousePosition.y - startPosition.y;

        unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
        // half way up width and height will give us center for our anchor points
        unitSelectionArea.anchoredPosition = startPosition +
            new Vector2(areaWidth / 2, areaHeight / 2); // in editor anchor should be set to bottom left
    }

    private void ClearSelectionArea()
    {
        unitSelectionArea.gameObject.SetActive(false);

        if(unitSelectionArea.sizeDelta.magnitude == 0)// WE clicked but didn't start dragging
        {

            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) { return; }

            // if the thing we hit doesn't have a unit component return
            if (!hit.collider.TryGetComponent<Unit>(out Unit unit)) { return; }

            if (!unit.isOwned) { return; }

            selectedUnits.Add(unit);

            foreach (Unit selectedUnit in selectedUnits) // each unit thats been selected invoke select method
            {
                selectedUnit.Select();
            }
            return;
        }
        //Multi select code
        // using the coordinates of the bottom left and top right corners
        // if x is less than right greater than left and we can find if its inside the box.

        Vector2 min = unitSelectionArea.anchoredPosition - (unitSelectionArea.sizeDelta / 2);
        Vector2 max = unitSelectionArea.anchoredPosition + (unitSelectionArea.sizeDelta / 2);

        // this is in screen space but tanks are in world space so we have to factor that in
        foreach (Unit unit1 in player.getMyUnits())
        {
            if (selectedUnits.Contains(unit1)) { continue; }// If the unit already exists we don't need to add it again we can ignore and move on
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(unit1.transform.position);

            if(screenPosition.x > min.x && 
                screenPosition.x <max.x &&
                screenPosition.y > min.y &&
                screenPosition.y < max.y)
            {
                selectedUnits.Add(unit1);
                unit1.Select();
            }
        }
    }
   
}
