using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace net.adamec.ui.AppSwitcherBar.Win32.Services.Shell.Properties
{
    /// <summary>
    /// Defines a unique key for a Shell Property
    /// </summary>
    /// <remarks>Based on PropertyKey.cs of Windows API Code Pack 1.1</remarks>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal readonly partial struct PropertyKey : IEquatable<PropertyKey>
    {
        /// <summary>
        /// Dictionary of known PropertyKeys and their names
        /// </summary>
        private static Dictionary<PropertyKey, string>? knownPropertyKeys;

        /// <summary>
        /// A unique GUID for the property
        /// </summary>
        public Guid FormatId { get; }

        /// <summary>
        /// Property identifier (PID)
        /// </summary>
        public int PropertyId { get; }

        /// <summary>
        /// PropertyKey Constructor
        /// </summary>
        /// <param name="formatId">A unique GUID for the property</param>
        /// <param name="propertyId">Property identifier (PID)</param>
        public PropertyKey(Guid formatId, int propertyId)
        {
            FormatId = formatId;
            PropertyId = propertyId;
        }

        /// <summary>
        /// PropertyKey Constructor
        /// </summary>
        /// <param name="formatId">A string representation of a GUID for the property</param>
        /// <param name="propertyId">Property identifier (PID)</param>
        public PropertyKey(string formatId, int propertyId) : this(new Guid(formatId), propertyId)
        {
        }

        /// <summary>
        /// CTOR for known PropertyKeys
        /// </summary>
        /// <param name="a">PropertyKey GUID component</param>
        /// <param name="b">PropertyKey GUID component</param>
        /// <param name="c">PropertyKey GUID component</param>
        /// <param name="d">PropertyKey GUID component</param>
        /// <param name="e">PropertyKey GUID component</param>
        /// <param name="f">PropertyKey GUID component</param>
        /// <param name="g">PropertyKey GUID component</param>
        /// <param name="h">PropertyKey GUID component</param>
        /// <param name="i">PropertyKey GUID component</param>
        /// <param name="j">PropertyKey GUID component</param>
        /// <param name="k">PropertyKey GUID component</param>
        /// <param name="propertyId">Property identifier (PID)</param>

        private PropertyKey(uint a, uint b, uint c, uint d, uint e, uint f, uint g, uint h, uint i, uint j, uint k, int propertyId)
            : this(new Guid(a, (ushort)b, (ushort)c, (byte)d, (byte)e, (byte)f, (byte)g, (byte)h, (byte)i, (byte)j, (byte)k), propertyId)
        {
        }

        /// <summary>
        /// Returns whether this object is equal to another. This is vital for performance of value types.
        /// </summary>
        /// <param name="other">The object to compare against.</param>
        /// <returns>Equality result.</returns>
        public bool Equals(PropertyKey other)
        {
            return other.Equals((object)this);
        }

        /// <summary>
        /// Returns the hash code of the object. This is vital for performance of value types.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return FormatId.GetHashCode() ^ PropertyId;
        }

        /// <summary>
        /// Returns whether this object is equal to another. This is vital for performance of value types.
        /// </summary>
        /// <param name="obj">The object to compare against.</param>
        /// <returns>Equality result.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is not PropertyKey other) return false;

            return other.FormatId.Equals(FormatId) && (other.PropertyId == PropertyId);
        }

        /// <summary>
        /// Implements the == (equality) operator.
        /// </summary>
        /// <param name="propKey1">First property key to compare.</param>
        /// <param name="propKey2">Second property key to compare.</param>
        /// <returns>true if object a equals object b. false otherwise.</returns>        
        public static bool operator ==(PropertyKey propKey1, PropertyKey propKey2)
        {
            return propKey1.Equals(propKey2);
        }

        /// <summary>
        /// Implements the != (inequality) operator.
        /// </summary>
        /// <param name="propKey1">First property key to compare</param>
        /// <param name="propKey2">Second property key to compare.</param>
        /// <returns>true if object a does not equal object b. false otherwise.</returns>
        public static bool operator !=(PropertyKey propKey1, PropertyKey propKey2)
        {
            return !propKey1.Equals(propKey2);
        }

        /// <summary>
        /// String representation of object
        /// </summary>
        /// <returns>String representation of object</returns>        
        public override string ToString()
        {
            return $"{FormatId:B}, {PropertyId}";
        }

        /// <summary>
        /// Gets the name of <paramref name="propertyKey"/> 
        /// </summary>
        /// <param name="propertyKey">Property key to get name of</param>
        /// <returns>Name of <paramref name="propertyKey"/> if known, otherwise null</returns>
        public static string? GetPropertyKeyName(PropertyKey propertyKey)
        {
            return GetKnownPropertyKeys().TryGetValue(propertyKey, out var name) ? name : null;
        }

        /// <summary>
        /// Gets the dictionary of known property keys
        /// </summary>
        /// <returns>Dictionary of known property keys</returns>
        private static Dictionary<PropertyKey, string> GetKnownPropertyKeys()
        {
            knownPropertyKeys ??= typeof(PropertyKey)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(PropertyKey))
                .Select(f => ((PropertyKey)f.GetValue(null)!, f.Name))
                .ToDictionary(pn => pn.Item1, pn => pn.Name);

            return knownPropertyKeys;
        }

     }


}
