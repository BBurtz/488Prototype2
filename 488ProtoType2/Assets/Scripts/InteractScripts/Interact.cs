/******************************************************************
*    Author: Marissa Moser
*    Contributors: 
*    Date Created: May 24, 2024
*    Description: 
*       Contains a function called when the player presses E. This function will:
            Use a raycast to check if there is something in from of them
            to interact with and if it is interactable. If so, call the
            Interact(player) function on that game object

*******************************************************************/

using System.Collections;
using UnityEngine;

public class Interact : MonoBehaviour
{
    private Camera _camera;

    [SerializeField] private GameObject _targetGameObj;
    private IInteractable _interactable;
    private bool _canInteract;

    [SerializeField] private GameObject _pickupAnchor;
    private GameObject _inHandObject;
    private bool _isHolding;

    //raycast variables
    private RaycastHit _colliderHit;
    [SerializeField] private float _maxInteractDistance;
    [SerializeField] LayerMask _layerToIgnore;

    [SerializeField] GameObject marble;

    private void Awake()
    {
        _camera = Camera.main;

        StartDetectingInteractions();
    }

    /// <summary>
    /// Called when Interact input is started. Calls Interact() on the detected
    /// interactable game object
    /// </summary>
    private void InteractPressed()
    {
        //if holding something
        if (_isHolding)
        {
            DropObj();
        }
        if (_interactable != null)
        {
            _interactable.Interact(gameObject);
        }
    }

    /// <summary>
    /// Starts the Detect Interactable coroutine
    /// </summary>
    public void StartDetectingInteractions()
    {
        _canInteract = true;
        StartCoroutine(DetectInteractable());
    }

    /// <summary>
    /// Ends the Detect Interactable coroutine
    /// </summary>
    public void StopDetectingInteractions()
    {
        _canInteract = false;
    }

    /// <summary>
    /// A coroutine that detects if there is an interactable object in front of
    /// the player using a raycast. This coroutine can be stopped with the public 
    /// Start/StopDetectingInteraction function
    /// </summary>
    /// <returns></returns>
    private IEnumerator DetectInteractable()
    {
        while (_canInteract)
        {
            //Casts Raycast in the center of the screen
            Ray r = _camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            if (Physics.Raycast(r, out _colliderHit, _maxInteractDistance, ~_layerToIgnore))
            {
                _targetGameObj = _colliderHit.transform.gameObject;

                //sets the _interactable variable for the InteractPressed function
                if (_targetGameObj.TryGetComponent(out IInteractable interactable))
                {
                    _interactable = _targetGameObj.GetComponent<IInteractable>();
                    CanvasInteractionBehavior.ShowInteractUI?.Invoke();

                    //if each object needs their own prompt use this
                    //_interactable.DisplayInteractUI();
                }
                else if (_interactable != null)
                {
                    CanvasInteractionBehavior.HideInteractUI?.Invoke();

                    //if each object needs their own prompt use this
                    //_interactable.HideInteractUI();

                    _interactable = null;
                }
            }
            //resets the variables if the player backs away from interactable
            else if (_interactable != null)
            {
                _targetGameObj = null;

                CanvasInteractionBehavior.HideInteractUI?.Invoke();

                //if each object needs their own prompt use this
                //_interactable.HideInteractUI();

                _interactable = null;
            }
            yield return null;
        }
    }

    /// <summary>
    /// Called when Interact input is canceled. Calls CancelInteract() on the
    /// detected interactable game object.
    /// </summary>
    /// <param name="obj"></param>
    private void InteractReleased()
    {
        if (_interactable != null)
        {
            _interactable.CancelInteract();
        }
    }

    /// <summary>
    /// Picks up the object passed in the parameter. if the player is already holding
    /// something, drop it first
    /// </summary>
    /// <param name="pickup"></param>
    public void PickUpObj(GameObject pickup)
    {
        _isHolding = true;
        _inHandObject = pickup;

        //sets the picked up iten to the anchor
        pickup.transform.position = _pickupAnchor.transform.position;
        pickup.transform.parent = _pickupAnchor.transform;

        PickupInteractable pi = pickup.GetComponent<PickupInteractable>();
        if (pi != null)
        {
            pickup.transform.localScale = _pickupAnchor.transform.localScale * pi.heldScaleMultiplier;
            pi.transform.rotation = pi.defaultRotation;
        }
        else
        {
            pickup.transform.rotation = _pickupAnchor.transform.rotation;
            pickup.transform.localScale = _pickupAnchor.transform.localScale;
        }


    }

    /// <summary>
    /// Drops the object in the player's hand from the player anchor
    /// </summary>
    public void DropObj()
    {
        if (_isHolding)
        {
            _isHolding = false;
            _inHandObject.transform.parent = null;
            _inHandObject.GetComponent<PickupInteractable>().EnableRB();
            _inHandObject = null;
        }
    }

    /// <summary>
    /// throws the object the player is holding
    /// </summary>
    public void ThrowObj()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        if (_isHolding)
        {
            _isHolding = false;

            if (_inHandObject == null)
                return;

            //"drops" object
            _inHandObject.transform.parent = null;
            _inHandObject.GetComponent<PickupInteractable>().EnableRB();

            //ADD THROW FORCE RAHHHHH 
            Ray r = _camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            _inHandObject.GetComponent<PickupInteractable>().ThrowObj(r.direction);

            _inHandObject = null;


            return;

        }
    }
}
