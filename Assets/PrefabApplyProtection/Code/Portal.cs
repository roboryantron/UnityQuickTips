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
    /// <summary>
    /// Triggers a scene load when something collides with it.
    /// </summary>
    public class Portal : MonoBehaviour, ISerializationCallbackReceiver
    {
        #region -- Inspector Fields -------------------------------------------
#if UNITY_EDITOR
        /// <summary>
        /// The scene asset that should be loaded. This scene needs to be 
        /// added to the EditorBuildSettings. This field can not be included 
        /// in a build since SceneAsset is part of UnityEditor. For this reason
        /// we have to save out the scene name to DestinationSceneName.
        /// </summary>
        [Tooltip("The scene to load into when the portal is entered.")]
        public UnityEditor.SceneAsset DestinationScene;
#endif
        [Tooltip("The string name of the scene used by Unity to load it " +
                 "since SceneAsset can not be saved in builds. This can't " +
                 "be edited directly, it is derived from DestinationScene")]
        public string DestinationSceneName;
        #endregion -- Inspector Fields ----------------------------------------

        #region -- MonoBehaviour Messages -------------------------------------
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

        /// <summary>
        /// On awake, make sure there is a valid scene. It is always a good 
        /// idea to make code fail as soon as possible to make it easier to 
        /// find issues.
        /// </summary>
        private void Awake()
        {
            if (string.IsNullOrEmpty(DestinationSceneName))
            {
                throw new Exception("No valid scene specified as " +
                                    "portal destination.");
            }
        }

        /// <summary> Triggers usage of the portal. </summary>
        /// <param name="col">Collider that entered the portal.</param>
        private void OnTriggerEnter(Collider col)
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
        #endregion -- MonoBehaviour Messages ----------------------------------

        #region -- ISerializationCallbackReceiver -----------------------------
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            // This needs to be in UNITY_EDITOR since we can not check prefab 
            // status in a build.
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