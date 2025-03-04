using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class Hands : MonoBehaviour
{
    public static UnityEvent<bool> leftHandCalled = new UnityEvent<bool>();

    public Transform LeftHandTransform;
    public Transform RightHandTransform;
    //public InventoryItemData testData;

    private InventorySystem leftHand;
    private InventorySystem rightHand;
    private Camera _camera;

    //[SerializeField] private GameObject _targetGameObj;
    private IInteractable _interactable;
    private bool _canInteract;
    private bool leftHandTargeted;

    //raycast variables
    private RaycastHit _colliderHit;
    [SerializeField] private float _maxInteractDistance;
    [SerializeField] LayerMask _layerToIgnore;

    private void Start()
    {
        leftHand = new InventorySystem(1);
        rightHand = new InventorySystem(1);
        _camera = Camera.main;
        StartDetectingInteractions();
        leftHandCalled.AddListener(InteractPressed);
    }

    /// <summary>
    /// Called when Interact input is started, which passes along if the player hit the left or right mouse button
    /// After determining which hand is being targeted, the item is told to interact, and it (if it is a pickupable)
    /// will attempt to add itself to hands, calling AddItem in this script
    /// interactable game object
    /// </summary>
    private void InteractPressed(bool leftButtonPressed)
    {
        leftHandTargeted = leftButtonPressed;
        if (_interactable != null)
        {
            _interactable.Interact(gameObject); //tell the object that the player is interacting with it, and pass along the player for good measure
        }
        else if (_interactable == null)//not looking at something
        {
            DropObject(leftButtonPressed);
            Transform temp = leftButtonPressed ? LeftHandTransform : RightHandTransform;
            foreach (Transform child in temp)
            {
                Destroy(child.gameObject);
            }
        }
    }
    public InventorySystem GetTargetedHand()
    {
        if (leftHandTargeted)
        {
            return leftHand;
        }
        else
        {
            return rightHand;
        }
    }

    /// <summary>
    /// Picks up the object passed in the parameter.
    /// </summary>
    /// <param name="pickup"></param>
    public void ShowObjectInHand(GameObject pickup, Transform hand)
    {
        GameObject clone = Instantiate(pickup, hand.position, Quaternion.identity); // Create clone
        clone.transform.parent = hand.transform;
        clone.transform.localScale = hand.transform.localScale;
        clone.transform.rotation = hand.rotation; //Quaternion.identity;
        clone.GetComponent<PickupInteractable>().DisableRB(); // Ensure RB is disabled
    }

    /// <summary>
    /// Make a gameobject to drop the data from the passed in hand
    /// </summary>
    /// <param name="leftHandToDrop"></param>
    public void DropObject(bool leftHandToDrop)
    {
        if (leftHandToDrop)
        {
            InventoryItemData droppedItem = null;
            leftHand.RemoveFromInventory(leftHand.GetInventoryItemList()[0], 1, true, out droppedItem, out _);
            if(droppedItem != null)
            {
                var go = Instantiate(droppedItem.ItemPrefab, LeftHandTransform.position, Quaternion.identity);
                go.transform.parent = null;
                if (go.GetComponent<PickupInteractable>() != null)
                {
                    go.GetComponent<PickupInteractable>().EnableRB();
                }
            }

        }
        else
        {
            InventoryItemData droppedItem = null;
            rightHand.RemoveFromInventory(rightHand.GetInventoryItemList()[0], 1, true, out droppedItem, out _);
            if(droppedItem != null)
            {
                var go = Instantiate(droppedItem.ItemPrefab, RightHandTransform.position, Quaternion.identity);
                go.transform.parent = null;
                if (go.GetComponent<PickupInteractable>() != null)
                {
                    go.GetComponent<PickupInteractable>().EnableRB();
                }
            }
        }
    }

    public void AddItem(InventoryItemData data, InventorySystem handToAddTo)
    {
        if (handToAddTo.AddToInventory(data, 1, out _))
        {
            ShowObjectInHand(data.ItemPrefab, leftHandTargeted? LeftHandTransform : RightHandTransform);
            return;
        }
        else //was adding unsuccessful? (hand full?)
        {
            DropObject(leftHandTargeted);

            handToAddTo.AddToInventory(data, 1, out _);

            //PickupInteractable.CreateItemObject(droppedItem, LeftHandTransform.position);
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
                //_targetGameObj = _colliderHit.transform.gameObject;
                var go = _colliderHit.transform.gameObject;
                //sets the _interactable variable for the InteractPressed function
                //if (_targetGameObj.TryGetComponent(out IInteractable interactable))
                if(go.TryGetComponent(out IInteractable interactable))
                {
                    //_interactable = _targetGameObj.GetComponent<IInteractable>();
                    _interactable = interactable;
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
                //_targetGameObj = null;

                CanvasInteractionBehavior.HideInteractUI?.Invoke();

                //if each object needs their own prompt use this
                //_interactable.HideInteractUI();

                _interactable = null;
            }
            yield return new WaitForSeconds(0.1f);
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
}
