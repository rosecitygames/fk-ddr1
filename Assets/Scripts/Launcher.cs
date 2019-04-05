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
    float angle = 0.0f;

    [SerializeField]
    float strength = 1;

    [SerializeField]
    float torque = 0.0f;

    [SerializeField]
    UnityEvent OnLaunch = null;

    [SerializeField]
    float delaySeconds = 0.0f;

    Vector2 Force
    {
        get
        {
            Vector2 force = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            force *= strength;
            return force;
        }
    }

    private void Start()
    {
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.simulated = false;

        if (launchOnstart)
        {
            Invoke("Launch", delaySeconds);
        }
    }

    [ContextMenu("Launch")]
    public void Launch()
    {
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.simulated = true;

        rigidbody.AddForce(Force, ForceMode2D.Impulse);
        rigidbody.AddTorque(torque, ForceMode2D.Impulse);

        if (OnLaunch != null)
        {
            OnLaunch.Invoke();
        }
    }


    private void OnDrawGizmos()
    {
        Vector3 position = transform.position;

        Vector2 force = Force;
        Vector3 forcePosition = transform.position + new Vector3(force.x, force.y, 0.0f);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(position, forcePosition);
    }

}
