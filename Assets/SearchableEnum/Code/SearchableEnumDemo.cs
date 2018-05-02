using UnityEngine;
using UnityEngine.XR;

namespace RoboRyanTron.SearchableEnum
{
    [CreateAssetMenu]
    public class SearchableEnumDemo : ScriptableObject
    {
        [SearchableEnum]
        public KeyCode AwesomeKeyCode;
    
        public KeyCode LameKeyCode;
        
        [SearchableEnum]
        public XRNode Node;
    }
}
