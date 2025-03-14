using UnityEngine;

/// <summary>
/// DEPRECIATED
/// </summary>
public class UnderwaterEffects : MonoBehaviour
{
    [SerializeField] private GameObject waterFX;

    private void OnTriggerEnter(Collider other)
    {
        if (waterFX == null)
        {
            Debug.LogError("WATER IS NOT APPLIED IN INSPECTOR");
        }
        Debug.Log("on");
        waterFX.gameObject.SetActive(true);
        RenderSettings.fog = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (waterFX == null)
        {
            Debug.LogError("WATER IS NOT APPLIED IN INSPECTOR");
        }

        Debug.Log("off");
        waterFX.gameObject.SetActive(false);
        RenderSettings.fog = false;
    }
}
