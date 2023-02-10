using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    private void OnGUI()
    {
        GUILayout.Label("Player Control");
        GUILayout.Label("Run Direction : W/A/S/D");
        GUILayout.Label("Run Sprint    : Shift + W/A/S/D");
        GUILayout.Label("Jump          : Space");
        GUILayout.Label("Attack        : J");

    }
}
