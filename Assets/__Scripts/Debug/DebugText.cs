using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DebugText : MonoBehaviour
{
    
    static Text debugText;

    static int step = 0;

    void Awake()
    {
        debugText = GetComponent<Text>();
    }

    public static void ShowDebug(string text){

        debugText.text = debugText.text + "\nstep " + step + ": " + text;
        step++;
    }

}
