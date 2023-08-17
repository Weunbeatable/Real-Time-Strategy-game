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
    [SerializeField] private LayerMask layerMask = new LayerMask();

    private Camera mainCamera; // for refrencing mouse pos

    public List<Unit> selectedUnits { get; } = new List<Unit>();
   private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame) 
        {
            // we wan tto start selection area. 
            foreach (Unit selectedUnit in selectedUnits) // each unit thats been selected invoke select method
            {
                selectedUnit.Deselect();
            }
            selectedUnits.Clear();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        }
    }

    private void ClearSelectionArea()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) { return; }

        // if the thing we hit doesn't have a unit component return
        if (!hit.collider.TryGetComponent<Unit>(out Unit unit)) { return; }

        if (!unit.isOwned) { return; }

        selectedUnits.Add(unit); 

        foreach(Unit selectedUnit in selectedUnits) // each unit thats been selected invoke select method
        {
            selectedUnit.Select();
        }
    }
   
}
