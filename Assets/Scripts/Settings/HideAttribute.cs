using System;

namespace Elektronik.Settings
{
    /// <summary> Marks that target settings field should not be rendered in UI. </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class HideAttribute : Attribute
    {
        
    }
}