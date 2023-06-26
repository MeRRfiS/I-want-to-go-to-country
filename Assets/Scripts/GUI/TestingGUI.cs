using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingGUI : MonoBehaviour
{

    private void OnGUI()
    {
        DotOnTheCenter();
    }

    private void Update()
    {
        ApplyDebugRay();
    }

    private void ApplyDebugRay()
    {
        Vector3 forward = Camera.main.transform.forward * 5f;
        Debug.DrawRay(Camera.main.transform.position, forward, Color.red);
    }

    private void DotOnTheCenter()
    {
        GUI.color = Color.gray;
        GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 5, 5), "");
    }
}
