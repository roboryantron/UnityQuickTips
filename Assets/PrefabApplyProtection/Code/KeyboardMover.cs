// ----------------------------------------------------------------------------
// Quick Tips
// 
// Author: Ryan Hipple
// Date:   10/30/17
// ----------------------------------------------------------------------------

using System;
using UnityEngine;

namespace RoboRyanTron.PrefabApplyProtection
{
    /// <summary>
    /// Simple class to move a game object based on keyboard input.
    /// </summary>
    public class KeyboardMover : MonoBehaviour
    {
        /// <summary>
        /// Defines a movement axis, consisting of the keyboard input needed 
        /// for positive and negative movement on the axis.
        /// </summary>
        [Serializable]
        public class MoveAxis
        {
            [Tooltip("Key to move forward on this axis.")]
            public KeyCode Positive;

            [Tooltip("Key to move backward on this axis.")]
            public KeyCode Negative;

            public MoveAxis(KeyCode positive, KeyCode negative)
            {
                Positive = positive;
                Negative = negative;
            }
            
            public static implicit operator float(MoveAxis axis)
            {
                return (Input.GetKey(axis.Positive)
                    ? 1.0f : 0.0f) -
                    (Input.GetKey(axis.Negative)
                    ? 1.0f : 0.0f);
            }
        }

        [Tooltip("Units per second to move.")]
        public float MoveRate = 1.0f;

        [Tooltip("Keys to move along the horizontal axis.")]
        public MoveAxis Horizontal = new MoveAxis(KeyCode.D, KeyCode.A);

        [Tooltip("Keys to move along the vertical axis.")]
        public MoveAxis Vertical = new MoveAxis(KeyCode.W, KeyCode.S);

        private void Update()
        {
            Vector3 moveNormal = new Vector3(Horizontal, Vertical, 0.0f).normalized;

            transform.position += moveNormal*Time.deltaTime*MoveRate;
        }
    }
}