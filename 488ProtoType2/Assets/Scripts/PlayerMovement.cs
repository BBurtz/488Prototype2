/*****************************************************************************
// File Name :          PlayerMovement.cs
// Author :             Brenden Burtz
// Creation Date :      January 29, 2025
// Modified Date :      February 3, 2025
// Last Modified By :   Cade Naylor
//
// Brief Description :  Handles player input controls
                            - Player Movement, both normal and treadmill
                            - Calls interaction functions
*****************************************************************************/
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public float jumpStrength;
    public float PlayerHeight;

    public bool CurrentlyJumping;
    private bool CurrentlyMoving;
    private bool InCollection;

    public GameObject Camera;
    public GameObject EndScrene;
    public GameObject PauseMenu;

    public PlayerInput playerControls;

    private InputAction MoveAction;
    private InputAction InteractAction;
    private InputAction JumpAction;
    private InputAction AttackAction;
    private InputAction RightAction;
    private InputAction PauseAction;

    private bool grounded;

    [SerializeField] private LayerMask whatIsGround;

    private Rigidbody rb;

    Vector2 MoveVal;

    Coroutine movementcoroutineInstance;

    


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "CollectionZone")
        {
            InCollection = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "CollectionZone")
        {
            InCollection = false;
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        //boxCreationDestruction = FindObjectOfType<BoxCreationDestruction>();
        Cursor.lockState = CursorLockMode.Locked;

        //audio
    }
    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, PlayerHeight * 0.5f + 0.2f, whatIsGround);
    }

    private void OnEnable()
    {
        playerControls.ActivateInput();
        MoveAction = playerControls.currentActionMap.FindAction("Move");
        InteractAction = playerControls.currentActionMap.FindAction("Interact");
        JumpAction = playerControls.currentActionMap.FindAction("Jump");
        RightAction = playerControls.currentActionMap.FindAction("RightClick");
        AttackAction = playerControls.currentActionMap.FindAction("Attack");
        PauseAction = playerControls.currentActionMap.FindAction("Pause");
        JumpAction.started += Jump;
        PauseAction.started += pause;
        AttackAction.performed += LeftClick;
        RightAction.performed += RightClick;
        MoveAction.performed += move;
        MoveAction.canceled += stop;
        InteractAction.started += interact;
    }

    private void RightClick(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    private void LeftClick(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    private void pause(InputAction.CallbackContext context)
    {
        if (PauseMenu == null)
        {
            return;
        }

        if (!PauseMenu.activeSelf)
        {
            PauseMenu.SetActive(true);
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
        }
        else if (PauseMenu.activeSelf)
        {
            PauseMenu.SetActive(false);
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
        }

    }

    private void OnDisable()
    {
        playerControls.DeactivateInput();
        JumpAction.started -= Jump;
        MoveAction.performed -= move;
        MoveAction.canceled -= stop;
        PauseAction.started -= pause;
        InteractAction.started -= interact;
        AttackAction.performed -= LeftClick;
        RightAction.performed -= RightClick;
    }


    private void FixedUpdate()
    {
            if (!CurrentlyMoving)
            {
                MoveVal = Vector3.zero;
            }
            var c = MoveVal;
            Vector3 moveDirection = Camera.transform.forward * c.y + Camera.transform.right * c.x;
            moveDirection.y = 0;
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (grounded)
        {
            rb.AddForce(0, jumpStrength, 0, ForceMode.Force);
            grounded = false;
        }
    }


    /// <summary>
    /// Called when the interact key is pressed. 
    /// Calls MoveBox if interact is used to move box
    /// </summary>
    /// <param name="context"></param>
    private void interact(InputAction.CallbackContext context)
    {
       
    }


    private void stop(InputAction.CallbackContext context)
    {
        /*StopCoroutine(movementcoroutineInstance);
        movementcoroutineInstance = null;*/
        CurrentlyMoving = false;
        MoveVal = new Vector3(0, rb.linearVelocity.y, 0);
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
    }

    private void move(InputAction.CallbackContext context)
    {
        CurrentlyMoving = true;
        MoveVal  = context.ReadValue<Vector2>();
        /*if(movementcoroutineInstance == null )
        {
            movementcoroutineInstance = StartCoroutine(Movement());
        }*/

    }


    /// <summary>
    /// Coroutine for movement under normal conditions
    /// </summary>
    /// <returns>Time waited between calls</returns>
    /*public IEnumerator Movement()
    {
        while (true)
        {
            if(!movementOverrideForTreadmill)
            {
                var c = MoveVal;
                Vector3 moveDirection = Camera.transform.forward * c.y + Camera.transform.right * c.x;
                moveDirection.y = 0;
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f * Time.deltaTime, ForceMode.Force);

                Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                if (flatVel.magnitude > moveSpeed)
                {
                    Vector3 limitedVel = flatVel.normalized * moveSpeed;
                    rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
                }
                UpdateWalkSFX();
            }
            yield return null;
        }
    }*/

}
