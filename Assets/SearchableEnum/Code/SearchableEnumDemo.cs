using UnityEngine;
using UnityEngine.XR;

namespace RoboRyanTron.SearchableEnum
{
    [CreateAssetMenu]
    public class SearchableEnumDemo : ScriptableObject
    {
        [SearchableEnum]
        public KeyCode KeyCode;
    
        [SearchableEnum]
        public XRNode Node;

        public KeyCode OtherKeyCode;
    }
}
