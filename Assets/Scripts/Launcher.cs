using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class Launcher : MonoBehaviour
{
    [SerializeField]
    bool launchOnstart = true;

    [SerializeField]
    [Range(0.0f, 360.0f)]
    float angle;

    [SerializeField]
    float strength = 1;

    [SerializeField]
    UnityEvent OnLaunch;

    private void Start()
    {
        if(launchOnstart)
        {
            Launch();
        }
    }

    [ContextMenu("Launch")]
    public void Launch()
    {
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();

        Vector2 force = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        force *= strength;

        rigidbody.AddForce(force, ForceMode2D.Impulse);

        if (OnLaunch != null)
        {
            OnLaunch.Invoke();
        }
    }

}
