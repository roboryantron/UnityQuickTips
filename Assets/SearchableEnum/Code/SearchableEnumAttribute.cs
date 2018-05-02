// ---------------------------------------------------------------------------- 
// Author: Ryan Hipple
// Date:   05/01/2018
// ----------------------------------------------------------------------------

using System;
using UnityEngine;

namespace RoboRyanTron.SearchableEnum
{
    /// <summary>
    /// An attribute meant to be applied to serializeable enum fields that will
    /// display an enum selector that is searchable with a string filter. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SearchableEnumAttribute : PropertyAttribute {}
}
