using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscillatorHandler : MonoBehaviour
{
    public static int CoreClockState = 0;
    public static int MemoryClockState = 0;
    // Start is called before the first frame update
    void Start()
    {
        StartClock();
    }

    public int getCoreClockState() {
        return CoreClockState;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void StartClock() {
        StartCoroutine(RunCoreClock());
        
    }

    IEnumerator RunCoreClock() {
        CoreClockState = 1;
        yield return new WaitForSeconds(01);
        CoreClockState = 0;
        yield return new WaitForSeconds(01);
        CoreClockState = 1;
        yield return new WaitForSeconds(01);
        CoreClockState = 0;
    }

}
