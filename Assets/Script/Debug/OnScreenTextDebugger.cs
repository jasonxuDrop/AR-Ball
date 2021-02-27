using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OnScreenTextDebugger : MonoBehaviour
{

    public TMP_Text debugText;
    string log = "";

    public void Queue(string s) {
        log += s + "\n";
    }

    void LateUpdate()
    {
        debugText.text = log;
        Debug.Log(log);

        log = "";
    }
}
