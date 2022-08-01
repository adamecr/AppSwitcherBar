using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeEnums
{
    /// <summary>
    /// Describes the relative description type for a property description, as determined by the relativeDescriptionType attribute of the displayInfo element.
    /// </summary>
    internal enum RelativeDescriptionType
    {
        /// <summary>
        /// General type.
        /// </summary>
        General,
        /// <summary>
        /// Date type.
        /// </summary>
        Date,
        /// <summary>
        /// Size type.
        /// </summary>
        Size,
        /// <summary>
        /// Count type.
        /// </summary>
        Count,
        /// <summary>
        /// Revision type.
        /// </summary>
        Revision,
        /// <summary>
        /// Length type.
        /// </summary>
        Length,
        /// <summary>
        /// Duration type.
        /// </summary>
        Duration,
        /// <summary>
        /// Speed type.
        /// </summary>
        Speed,
        /// <summary>
        /// Rate type.
        /// </summary>
        Rate,
        /// <summary>
        /// Rating type.
        /// </summary>
        Rating,
        /// <summary>
        /// Priority type.
        /// </summary>
        Priority
    }
}
