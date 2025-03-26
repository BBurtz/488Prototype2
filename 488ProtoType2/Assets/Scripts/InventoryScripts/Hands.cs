using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using FMOD.Studio;
using FMODUnity;

public class Hands : MonoBehaviour
{
    public static UnityEvent<bool> leftHandCalled = new UnityEvent<bool>();
    [SerializeField] private Transform LeftHandTransform;
    [SerializeField] private Transform RightHandTransform;
    [Tooltip("For scaling objects in hand")][SerializeField] private float Handsize;
    [SerializeField] private float throwStrength;

    //public InventoryItemData testData;

    private InventorySystem leftHand;
    private InventorySystem rightHand;
    private Camera _camera;
    private GameObject _targetGameObj;
    private IInteractable _interactable;
    private bool _canInteract;
    private bool leftHandTargeted;
    //raycast variables
    private RaycastHit _colliderHit;
    [SerializeField] private float _maxInteractDistance;
    [SerializeField] LayerMask _layerToIgnore;

    public float handCycleSpeed;
    public float moveThreshold;
    public float maxDeviation;
    private Vector3 leftHandStartPos, rightHandStartPos;
    private Rigidbody _playerRb;
    private PlayerMovement _player;

    private EventInstance grabSFX;
    private EventInstance dropSFX;

    private void Start()
    {
        leftHand = new InventorySystem(1);
        rightHand = new InventorySystem(1);
        _camera = Camera.main;
        StartDetectingInteractions();
        leftHandCalled.AddListener(InteractPressed);
        _player = GetComponent<PlayerMovement>();
        _playerRb = GetComponent<Rigidbody>();

        grabSFX = AudioManager.instance.CreateEventInstance(FMODEvents.instance.Pickup);
        dropSFX = AudioManager.instance.CreateEventInstance(FMODEvents.instance.Drop);

        leftHandStartPos = LeftHandTransform.localPosition;
        rightHandStartPos = RightHandTransform.localPosition;
        StartCoroutine(MoveHands());
    }


    /// <summary>
    /// There was not sufficient time in the prototyping schedule for me to use
    /// the animator to make hands, as much as I would have liked to in order 
    /// to make a better looking run cycle. In the interest of time and 
    /// prioritizing other parts of development, I took to the internet to make 
    /// a math-based run cycle.
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveHands()
    {
        float elapsed = 0f;
        float startA = LeftHandTransform.localPosition.y;
        float startB = RightHandTransform.localPosition.y;
        bool forward = true;
        bool wasMoving = false;
        float transitionDuration = 0.3f;

        while (true)
        {
            elapsed = 0f;

            if (_playerRb.linearVelocity.magnitude > moveThreshold)
            {
                if (!wasMoving)
                {
                    float transitionElapsed = 0f;

                    float targetA = startA - maxDeviation;
                    float targetB = startB + maxDeviation;

                    while (transitionElapsed < transitionDuration)
                    {
                        transitionElapsed += Time.deltaTime;
                        float t = transitionElapsed / transitionDuration;
                        t = Mathf.Clamp01(t * t * (3f - 2f * t)); //more direct easing than smoothlerp

                        LeftHandTransform.localPosition = new Vector3(
                            LeftHandTransform.localPosition.x, Mathf.Lerp(startA, targetA, t), LeftHandTransform.localPosition.z);

                        RightHandTransform.localPosition = new Vector3(
                            RightHandTransform.localPosition.x, Mathf.Lerp(startB, targetB, t), RightHandTransform.localPosition.z);

                        yield return null;
                    }

                    wasMoving = true;
                }

                //crap ton of ternary operators to determine normal movement direction for left/right
                float fromA = forward ? startA - maxDeviation : startA + maxDeviation;
                float toA = forward ? startA + maxDeviation : startA - maxDeviation;
                float fromB = forward ? startB + maxDeviation : startB - maxDeviation;
                float toB = forward ? startB - maxDeviation : startB + maxDeviation;

                float adjustedDuration = _playerRb.linearVelocity.magnitude / handCycleSpeed;

                while (elapsed < adjustedDuration)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / adjustedDuration;
                    t = Mathf.Clamp01(t * t * (3f - 2f * t)); //more aggressive easing, similar to above

                    LeftHandTransform.localPosition = new Vector3(
                        LeftHandTransform.localPosition.x, Mathf.Lerp(fromA, toA, t), LeftHandTransform.localPosition.z);

                    RightHandTransform.localPosition = new Vector3(
                        RightHandTransform.localPosition.x, Mathf.Lerp(fromB, toB, t), RightHandTransform.localPosition.z);

                    yield return null;
                }

                forward = !forward;
            }
            else
            {
                if (wasMoving)
                {
                    float returnElapsed = 0f;
                    Vector3 startPosA = LeftHandTransform.localPosition;
                    Vector3 startPosB = RightHandTransform.localPosition;

                    while (returnElapsed < transitionDuration)
                    {
                        returnElapsed += Time.deltaTime;
                        float t = returnElapsed / transitionDuration;
                        t = Mathf.Clamp01(t * t * (3f - 2f * t));

                        LeftHandTransform.localPosition = new Vector3(
                            LeftHandTransform.localPosition.x, Mathf.Lerp(startPosA.y, startA, t), LeftHandTransform.localPosition.z);

                        RightHandTransform.localPosition = new Vector3(
                            RightHandTransform.localPosition.x, Mathf.Lerp(startPosB.y, startB, t), RightHandTransform.localPosition.z);

                        yield return null;
                    }
                    wasMoving = false;
                }
                yield return null;
            }
        }
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
            DropObject(leftHandTargeted);
        }
    }
    public InventorySystem GetTargetedInventory()
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
    public bool GetTargetedHand()
    {
        return leftHandTargeted;
    }

    /// <summary>
    /// Picks up the object passed in the parameter.
    /// </summary>
    /// <param name="pickup"></param>
    public void ShowObjectInHand(GameObject pickup, Transform hand)
    {
        GameObject clone = Instantiate(pickup, hand.position, Quaternion.identity); // Create clone
        clone.transform.parent = hand.transform;
        clone.transform.localScale = pickup.transform.localScale * Handsize;
        clone.transform.rotation = hand.rotation; //Quaternion.identity;
        clone.GetComponent<PickupInteractable>().DisableRB(); // Ensure RB is disabled
        clone.GetComponent <PickupInteractable>().SetHeldInHand(true);
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
                float sound = checkItemSFX(droppedItem.DisplayName);
                dropSFX.setParameterByName("ItemSheet", sound);
                dropSFX.start();

                var go = Instantiate(droppedItem.ItemPrefab, LeftHandTransform.position, Quaternion.identity);
                go.transform.parent = null;
                go.transform.localScale = droppedItem.ItemPrefab.transform.lossyScale;
                if (go.TryGetComponent(out PickupInteractable pi))
                {
                    pi.EnableRB();
                    go.GetComponent<Rigidbody>().AddForce(LeftHandTransform.up + LeftHandTransform.forward * throwStrength, ForceMode.Impulse);
                    pi.SetHeldInHand(false);
                }
                for(int i = 0; i < LeftHandTransform.childCount; i++)
                {
                    if(i ==0)
                    {
                        continue;
                    }
                    else
                    {
                        Destroy(LeftHandTransform.transform.GetChild(i).gameObject);
                    }
                }
                //foreach (Transform child in LeftHandTransform)
                //{
                //    Destroy(child.gameObject);
                //}
            }
        }
        else
        {
            InventoryItemData droppedItem = null;
            rightHand.RemoveFromInventory(rightHand.GetInventoryItemList()[0], 1, true, out droppedItem, out _);
            if(droppedItem != null)
            {
                float sound = checkItemSFX(droppedItem.DisplayName);
                dropSFX.setParameterByName("ItemSheet", sound);
                dropSFX.start();

                var go = Instantiate(droppedItem.ItemPrefab, RightHandTransform.position, Quaternion.identity);
                go.transform.parent = null;
                go.transform.localScale = droppedItem.ItemPrefab.transform.lossyScale;
                if (go.TryGetComponent(out PickupInteractable pi))
                {
                    pi.EnableRB();
                    go.GetComponent<Rigidbody>().AddForce(RightHandTransform.up + RightHandTransform.forward * throwStrength, ForceMode.Impulse);
                    pi.SetHeldInHand(false);
                }
                for (int i = 0; i < RightHandTransform.childCount; i++)
                {
                    if (i == 0)
                    {
                        continue;
                    }
                    else
                    {
                        Destroy(RightHandTransform.transform.GetChild(i).gameObject);
                    }
                }
                //foreach (Transform child in LeftHandTransform)
                //{
                //    Destroy(child.gameObject);
                //}
            }
        }
    }

    public void AddItem(InventoryItemData data, InventorySystem handToAddTo, Vector3 itemScale)
    {

        if (handToAddTo.AddToInventory(data, 1, out _))
        {
            ShowObjectInHand(data.ItemPrefab, leftHandTargeted? LeftHandTransform : RightHandTransform);

            float sound = checkItemSFX(data.DisplayName);
            grabSFX.setParameterByName("ItemSheet", sound);
            grabSFX.start();

            return;
        }
        else //was adding unsuccessful? (hand full?)
        {
            DropObject(leftHandTargeted);

            /*float sound = checkItemSFX(data.DisplayName);
            dropSFX.setParameterByName("ItemSheet", sound);
            dropSFX.start();*/

            handToAddTo.AddToInventory(data, 1, out _);
            ShowObjectInHand(data.ItemPrefab, leftHandTargeted ? LeftHandTransform : RightHandTransform);
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
                _targetGameObj = _colliderHit.transform.gameObject;
                //var go = _colliderHit.transform.gameObject;
                //sets the _interactable variable for the InteractPressed function
                //if (_targetGameObj.TryGetComponent(out IInteractable interactable))
                if(_targetGameObj.TryGetComponent(out IInteractable interactable))
                {
                    //_interactable = _targetGameObj.GetComponent<IInteractable>();
                    _interactable = interactable;
                    //CanvasInteractionBehavior.ShowInteractUI?.Invoke();
                    //if each object needs their own prompt use this
                    _interactable.DisplayInteractUI();
                }
                else if (_interactable != null)
                {
                    //CanvasInteractionBehavior.HideInteractUI?.Invoke();

                    //if each object needs their own prompt use this
                    _interactable.HideInteractUI();

                    _interactable = null;
                }
            }
            //resets the variables if the player backs away from interactable
            else if (_interactable != null)
            {
                //_targetGameObj = null;

                //CanvasInteractionBehavior.HideInteractUI?.Invoke();

                //if each object needs their own prompt use this
                _interactable.HideInteractUI();

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

    private float checkItemSFX (string name)
    {
        float sound = 0;
        switch (name)
        {
            case "WoodenPlank":
                sound = 0;
                break;
            case "Map":
                sound = 1;
                break;
            case "RumBottle":
                sound = 2;
                break;
            case "Beachball":
                sound = 3;
                break;
            case "Diamond":
                sound = 4;
                break;
            case "MoneyBag":
                sound = 5;
                break;
            case "Fork":
                sound = 6;
                break;
            case "Cannonball":
                sound = 7;
                break;
        }
        return sound;
    }

    //just to update sfx to player location
    private void Update()
    {
        grabSFX.set3DAttributes(RuntimeUtils.To3DAttributes(_player.transform, _playerRb));
        dropSFX.set3DAttributes(RuntimeUtils.To3DAttributes(_player.transform, _playerRb));
    }
}
