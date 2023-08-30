using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private Transform playerCameraTransform = null;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float screenBoarderThickness = 10f;
    [SerializeField] private Vector2 screenXLimits = Vector2.zero;
    [SerializeField] private Vector2 screenZLimits = Vector2.zero;
    private Vector2 previouInput;

    private Controls controls;

    public override void OnStartAuthority()
    {
        playerCameraTransform.gameObject.SetActive(true);

        controls = new Controls();

        //Don't need to unsubscribe that's handled for us. 
        controls.Player.MoveCamera.performed += SetPreviousInput;
        controls.Player.MoveCamera.canceled += SetPreviousInput;

        controls.Enable();
    }

    [ClientCallback]
    private void Update()
    {
        // if you don't own the camera, not focused on the game to prevent camera just panning.
        if (!isOwned || !Application.isFocused) { return; }

        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        Vector3 pos = playerCameraTransform.position;

        // if no keyboard inputs check to see if mouse is at oen of the edges of the screen
        if(previouInput == Vector2.zero)
        {
            Vector3 cursorMovement = Vector3.zero;

            Vector2 cursorPosition = Mouse.current.position.ReadValue();

            // allow for some screen padding
            if(cursorPosition.y >= Screen.height - screenBoarderThickness)
            {
                cursorMovement.z += 1;
            }
            else if(cursorPosition.y <= screenBoarderThickness)
            {
                cursorMovement.z -= 1;
            }

            if (cursorPosition.x >= Screen.width - screenBoarderThickness)
            {
                cursorMovement.x += 1;
            }
            else if (cursorPosition.x <= screenBoarderThickness)
            {
                cursorMovement.x -= 1;
            }

            //normalize the vector to make it same for all directions and make it framerate independent with deltatime. 
            pos += cursorMovement.normalized * speed * Time.deltaTime;

        }
        else
        {
            pos += new Vector3(previouInput.x, 0f, previouInput.y) * speed * Time.deltaTime;
        }

        // make sure we don't go out of bounds 
        // x and y are min and max values 
        pos.x = Mathf.Clamp(pos.x, screenXLimits.x, screenXLimits.y);
        pos.z = Mathf.Clamp(pos.z, screenXLimits.x, screenXLimits.y);

        //assign camera position to these values. 
        playerCameraTransform.position = pos;
    }

    private void SetPreviousInput(InputAction.CallbackContext ctx)
    {
        previouInput = ctx.ReadValue<Vector2>();
    }
    #region Server

    #endregion

    #region Client

    #endregion
}
