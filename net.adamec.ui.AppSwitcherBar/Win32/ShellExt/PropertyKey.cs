using System;
using System.Runtime.InteropServices;

namespace net.adamec.ui.AppSwitcherBar.Win32.ShellExt
{
    /// <summary>
    /// Defines a unique key for a Shell Property
    /// </summary>
    /// <remarks>Based on PropertyKey.cs of Windows API Code Pack 1.1</remarks>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal readonly struct PropertyKey : IEquatable<PropertyKey>
    {
        #region Fields
        /// <summary>
        /// A unique GUID for the property
        /// </summary>
        private readonly Guid formatId;
        /// <summary>
        /// Property identifier (PID)
        /// </summary>
        private readonly int propertyId;

        #endregion

        #region Public Properties

        /// <summary>
        /// A unique GUID for the property
        /// </summary>
        public Guid FormatId => formatId;

        /// <summary>
        ///  Property identifier (PID)
        /// </summary>
        public int PropertyId => propertyId;

        #endregion

        #region Constructor

        /// <summary>
        /// PropertyKey Constructor
        /// </summary>
        /// <param name="formatId">A unique GUID for the property</param>
        /// <param name="propertyId">Property identifier (PID)</param>
        public PropertyKey(Guid formatId, int propertyId)
        {
            this.formatId = formatId;
            this.propertyId = propertyId;
        }

        /// <summary>
        /// PropertyKey Constructor
        /// </summary>
        /// <param name="formatId">A string representation of a GUID for the property</param>
        /// <param name="propertyId">Property identifier (PID)</param>
        public PropertyKey(string formatId, int propertyId)
        {
            this.formatId = new Guid(formatId);
            this.propertyId = propertyId;
        }

        #endregion

        #region IEquatable<PropertyKey> Members

        /// <summary>
        /// Returns whether this object is equal to another. This is vital for performance of value types.
        /// </summary>
        /// <param name="other">The object to compare against.</param>
        /// <returns>Equality result.</returns>
        public bool Equals(PropertyKey other)
        {
            return other.Equals((object)this);
        }

        #endregion

        #region Equality and hashing

        /// <summary>
        /// Returns the hash code of the object. This is vital for performance of value types.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return formatId.GetHashCode() ^ propertyId;
        }

        /// <summary>
        /// Returns whether this object is equal to another. This is vital for performance of value types.
        /// </summary>
        /// <param name="obj">The object to compare against.</param>
        /// <returns>Equality result.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is not PropertyKey other) return false;

            return other.formatId.Equals(formatId) && (other.propertyId == propertyId);
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
        /// Override ToString() to provide a user friendly string representation
        /// </summary>
        /// <returns>String representing the property key</returns>        
        public override string ToString()
        {
            return $"{formatId:B}, {propertyId}";
        }

        #endregion
    }
}
