using UnityEngine;

public class UnderwaterEffects : MonoBehaviour
{
    [SerializeField] private GameObject waterFX;


    private void OnTriggerEnter(Collider other)
    {
        waterFX.gameObject.SetActive(true);
        RenderSettings.fog = true;
    }

    private void OnTriggerExit(Collider other)
    {
        waterFX.gameObject.SetActive(false);
        RenderSettings.fog = false;
    }
}
