using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandGiver : MonoBehaviour
{
    [SerializeField] private UnitSelectionHandler unitSelectionHandler = null;
    [SerializeField] private LayerMask layerMask = new LayerMask();
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // when we right click raycast, check if we hit something
        // if we do move to the point of raycast
        if (!Mouse.current.rightButton.wasPressedThisFrame) { return; }

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) { return; }

        // check if we should attack or move
        if(hit.collider.TryGetComponent<Targetable>(out Targetable target))
        {
            if (target.isOwned)
            {
                // If we hit something that is targetable but dow't own it we'lll try set target. 
                TryMove(hit.point);
                return;
            }
            // If we do click on a targetable thing and own it we want to try moving so we dont attack our own units
            TryTarget(target);
            return;
        }
        // if we don't click on  a targetable thing we want to try moving
        TryMove(hit.point);
       
    }

    private void TryTarget(Targetable target)
    {
     
        foreach (Unit unit in unitSelectionHandler.selectedUnits)
        {
            // we want to get the unit movement from the unit
            unit.GetTargeter().CmdSetTarget(target.gameObject);
        }
    }

    private void TryMove(Vector3 point)
    {
        //only selected units will move where told to
        foreach (Unit unit in unitSelectionHandler.selectedUnits)
        {
            // we want to get the unit movement from the unit
            unit.GetUnitMovements().CmdMove(point);
        }
    }
}
