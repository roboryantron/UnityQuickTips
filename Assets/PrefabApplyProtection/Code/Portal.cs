// ----------------------------------------------------------------------------
// Quick Tips
// 
// Author: Ryan Hipple
// Date:   10/30/17
// ----------------------------------------------------------------------------

using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RoboRyanTron.PrefabApplyProtection
{
    public class Portal : MonoBehaviour, ISerializationCallbackReceiver
    {
        #region -- Inspector Fields -------------------------------------------
#if UNITY_EDITOR
        [Tooltip("The scene to load into when the portal is entered.")]
        public UnityEditor.SceneAsset DestinationScene;
#endif
        [Tooltip("The string name of the scene used by Unity to load it " +
                 "since SceneAsset can not be saved in builds. This can't " +
                 "be edited directly, it is derived from DestinationScene")]
        public string DestinationSceneName;
        #endregion -- Inspector Fields ----------------------------------------

        /// <summary>
        /// Caches the destination scene name that will be used for loading 
        /// since Unity can not reference a SceneAsset in a build.
        /// </summary>
        private void OnValidate()
        {
            if (DestinationScene != null)
                DestinationSceneName = DestinationScene.name;
            else
                DestinationSceneName = string.Empty;
        }

        /// <summary> Triggers usage of the portal. </summary>
        /// <param name="col">Collider that entered the portal.</param>
        private void OnTriggerEnter(Collider col)
        {
            Use();
        }

        /// <summary>
        /// Load the destination scene if it is valid.
        /// </summary>
        public void Use()
        {
            if (!string.IsNullOrEmpty(DestinationSceneName))
            {
                SceneManager.LoadScene(DestinationSceneName);
            }
            else
            {
                throw new Exception("No valid scene specified as " +
                                    "portal destination.");
            }
        }

        #region -- ISerializationCallbackReceiver -----------------------------
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
#if UNITY_EDITOR
            // Prevent saving if this is a prefab
            if (UnityEditor.PrefabUtility.GetPrefabType(this) == 
                UnityEditor.PrefabType.Prefab)
            {
                DestinationScene = null;
                DestinationSceneName = string.Empty;
            }
#endif
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            // Nothing needed here
        }
        #endregion -- ISerializationCallbackReceiver --------------------------
    }
}