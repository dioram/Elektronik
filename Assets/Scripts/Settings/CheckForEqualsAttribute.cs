using System;

namespace Elektronik.Settings
{
    /// <summary>
    /// Marks that target settings field should be used when checking equality of two settings bags.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class CheckForEqualsAttribute : Attribute
    {
    }
}