using UnityEditor.XR;
using UnityEngine;

public class Slots : MonoBehaviour
{

    public bool xActive = false;
    public bool treasureActive = false;
    public bool toolsActive = false;


    private void Start()
    {


    }

    public void X()
    {

        xActive = true;
        treasureActive = false;
        toolsActive = false;

    }

    public void Treasure()
    {

        xActive = false;
        treasureActive = true;
        toolsActive = false;

    }

    public void Tools()
    {

        xActive = false;
        treasureActive = false;
        toolsActive = true;

    }

}
