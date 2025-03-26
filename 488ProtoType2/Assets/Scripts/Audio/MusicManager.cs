using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class MusicManager : MonoBehaviour
{
    private EventInstance BGM;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BGM = AudioManager.instance.CreateEventInstance(FMODEvents.instance.TempBGM);

        BGM.start();
    }

    public void StopBGM()
    {
        BGM.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

}
