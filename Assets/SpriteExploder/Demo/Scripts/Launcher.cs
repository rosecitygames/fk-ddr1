using UnityEngine;
using UnityEngine.Events;

namespace IndieDeveloperTools.SpriteExploder.Demo
{
    /// <summary>
    /// A component that launches 2D rigidbodies using physics.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Launcher : MonoBehaviour
    {
        [SerializeField, Tooltip("Launch to rigidbody on start")]
        bool launchOnstart = true;

        [SerializeField, Range(0.0f, 360.0f), Tooltip("The launch angle")]
        float angle = 0.0f;

        [SerializeField, Tooltip("The strength of the launch")]
        float strength = 1;

        [SerializeField, Tooltip("The rotational torque (spin) of the launch")]
        float torque = 0.0f;

        [SerializeField] // Expose unity event in the inspector to trigger other component methods.
        UnityEvent OnLaunch = null;

        [SerializeField, Tooltip("The amount of delay before the auto start launch occurs")]
        float delaySeconds = 0.0f;

        /// <summary>
        /// The 2D force vector that will be applied to the rigidbody.
        /// </summary>
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
            // Disable the rigidbody simulation until launch is called.
            Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
            rigidbody.simulated = false;

            // Invoke the launch method with delay
            if (launchOnstart)
            {
                Invoke("Launch", delaySeconds);
            }
        }

        /// <summary>
        /// Launches the rigidbody.
        /// </summary>
        [ContextMenu("Launch")] // Accesible from context menu for testing in the editor.
        public void Launch()
        {
            // Enable the rigidbody simulation
            Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
            rigidbody.simulated = true;

            // Apply physics to the rigidbody
            rigidbody.AddForce(Force, ForceMode2D.Impulse);
            rigidbody.AddTorque(torque, ForceMode2D.Impulse);

            // Invoke any Unity Events
            OnLaunch?.Invoke();
        }

        /// <summary>
        /// Draw a simple inspector gizmo that shows the launch angle.
        /// </summary>
        private void OnDrawGizmos()
        {
            Vector3 position = transform.position;

            Vector2 force = Force;
            Vector3 forcePosition = transform.position + new Vector3(force.x, force.y, 0.0f);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(position, forcePosition);
        }
    }
}
