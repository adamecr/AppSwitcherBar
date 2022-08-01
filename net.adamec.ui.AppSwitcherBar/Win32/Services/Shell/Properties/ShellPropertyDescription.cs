using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;
using net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces;
using net.adamec.ui.AppSwitcherBar.Win32.NativeMethods;
// ReSharper disable InvertIf
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global

namespace net.adamec.ui.AppSwitcherBar.Win32.Services.Shell.Properties
{

    /// <summary>
    /// Defines the shell property description information for a property.
    /// </summary>
    /// <remarks>Based on Windows API Code Pack 1.1</remarks>
    internal class ShellPropertyDescription : IDisposable
    {
        /// <summary>
        /// Native property description COM interface
        /// </summary>
        private IPropertyDescription? nativePropertyDescription;
        /// <summary>
        /// Get the native property description COM interface
        /// </summary>
        private IPropertyDescription? NativePropertyDescription
        {
            get
            {
                if (nativePropertyDescription == null)
                {
                    var guid = new Guid(Win32Consts.IID_IPropertyDescription);
                    PropSys.PSGetPropertyDescription(ref propertyKey, ref guid, out nativePropertyDescription);
                }

                return nativePropertyDescription;
            }
        }

        /// <summary>
        /// Property key identifying the underlying property.
        /// </summary>
        private PropertyKey propertyKey;
        /// <summary>
        /// Gets the property key identifying the underlying property.
        /// </summary>
        public PropertyKey PropertyKey => propertyKey;

        /// <summary>
        /// Case-sensitive name of a property as it is known to the system, regardless of its localized name
        /// </summary>
        private string? canonicalName;
        /// <summary>
        /// Gets the case-sensitive name of a property as it is known to the system,  regardless of its localized name.
        /// </summary>
        public string CanonicalName
        {
            get
            {
                if (canonicalName == null)
                {
                    PropSys.PSGetNameFromPropertyKey(ref propertyKey, out canonicalName);
                }

                return canonicalName;
            }
        }


        /// <summary>
        /// Display name of the property as it is shown in any user interface (UI).
        /// </summary>
        private string? displayName;
        /// <summary>
        /// Gets the display name of the property as it is shown in any user interface (UI).
        /// </summary>
        public string? DisplayName
        {
            get
            {
                if (NativePropertyDescription != null && displayName == null)
                {
                    var hr = NativePropertyDescription.GetDisplayName(out var displayNamePtr);

                    if (hr.IsSuccess && displayNamePtr != IntPtr.Zero)
                    {
                        displayName = Marshal.PtrToStringUni(displayNamePtr);

                        // Free the string
                        Marshal.FreeCoTaskMem(displayNamePtr);
                    }
                }

                return displayName;
            }
        }

        /// <summary>
        /// Text used in edit controls hosted in various dialog boxes.
        /// </summary>
        private string? editInvitation;
        /// <summary>
        /// Gets the text used in edit controls hosted in various dialog boxes.
        /// </summary>
        public string? EditInvitation
        {
            get
            {
                if (NativePropertyDescription != null && editInvitation == null)
                {
                    // EditInvitation can be empty, so ignore the HR value, but don't throw an exception
                    var hr = NativePropertyDescription.GetEditInvitation(out var ptr);

                    if (hr.IsSuccess && ptr != IntPtr.Zero)
                    {
                        editInvitation = Marshal.PtrToStringUni(ptr);
                        // Free the string
                        Marshal.FreeCoTaskMem(ptr);
                    }
                }

                return editInvitation;
            }
        }

        /// <summary>
        /// VarEnum OLE type for this property.
        /// </summary>
        private VarEnum? varEnumType;
        /// <summary>
        /// Gets the VarEnum OLE type for this property.
        /// </summary>
        public VarEnum VarEnumType
        {
            get
            {
                if (NativePropertyDescription != null && varEnumType == null)
                {
                    var hr = NativePropertyDescription.GetPropertyType(out var tempType);
                    if (hr.IsSuccess)
                    {
                        varEnumType = tempType;
                    }
                }

                return varEnumType ?? default;
            }
        }

        /// <summary>
        /// .NET system type for a value of this property, or null if the value is empty.
        /// </summary>
        private Type? valueType;
        /// <summary>
        /// Gets the .NET system type for a value of this property, or null if the value is empty.
        /// </summary>
        public Type ValueType => valueType ??= ShellProperty.VarEnumToSystemType(VarEnumType);

        /// <summary>
        /// Current data type used to display the property.
        /// </summary>
        private PropertyDisplayType? displayType;
        /// <summary>
        /// Gets the current data type used to display the property.
        /// </summary>
        public PropertyDisplayType DisplayType
        {
            get
            {
                if (NativePropertyDescription != null && displayType == null)
                {
                    var hr = NativePropertyDescription.GetDisplayType(out var tempDisplayType);

                    if (hr.IsSuccess)
                    {
                        displayType = tempDisplayType;
                    }
                }

                return displayType ?? default;
            }
        }

        /// <summary>
        /// Default user interface (UI) column width for this property.
        /// </summary>
        private uint? defaultColumnWidth;
        /// <summary>
        /// Gets the default user interface (UI) column width for this property.
        /// </summary>
        public uint DefaultColumnWidth
        {
            get
            {
                if (NativePropertyDescription != null && !defaultColumnWidth.HasValue)
                {
                    var hr = NativePropertyDescription.GetDefaultColumnWidth(out var tempDefaultColumnWidth);

                    if (hr.IsSuccess)
                    {
                        defaultColumnWidth = tempDefaultColumnWidth;
                    }
                }

                return defaultColumnWidth ?? default;
            }
        }

        /// <summary>
        /// Value that describes how the property values are displayed when multiple items are selected in the user interface (UI).
        /// </summary>
        private PropertyAggregationType? aggregationTypes;
        /// <summary>
        /// Gets a value that describes how the property values are displayed when multiple items are selected in the user interface (UI).
        /// </summary>
        public PropertyAggregationType AggregationTypes
        {
            get
            {
                if (NativePropertyDescription != null && aggregationTypes == null)
                {
                    var hr = NativePropertyDescription.GetAggregationType(out var tempAggregationTypes);

                    if (hr.IsSuccess)
                    {
                        aggregationTypes = tempAggregationTypes;
                    }
                }

                return aggregationTypes ?? default;
            }
        }

        /// <summary>
        /// List of the possible values for this property
        /// </summary>
        private ReadOnlyCollection<ShellPropertyEnumType>? propertyEnumTypes;
        /// <summary>
        /// Gets a list of the possible values for this property.
        /// </summary>
        public ReadOnlyCollection<ShellPropertyEnumType>? PropertyEnumTypes
        {
            get
            {
                if (NativePropertyDescription != null && propertyEnumTypes == null)
                {
                    var propEnumTypeList = new List<ShellPropertyEnumType>();

                    var guid = new Guid(Win32Consts.IID_IPropertyEnumTypeList);
                    var hr = NativePropertyDescription.GetEnumTypeList(ref guid, out var nativeList);

                    if (nativeList != null && hr.IsSuccess)
                    {
                        nativeList.GetCount(out var count);
                        guid = new Guid(Win32Consts.IID_IPropertyEnumType);

                        for (uint i = 0; i < count; i++)
                        {
                            nativeList.GetAt(i, ref guid, out var nativeEnumType);
                            propEnumTypeList.Add(new ShellPropertyEnumType(nativeEnumType));
                        }
                    }

                    propertyEnumTypes = new ReadOnlyCollection<ShellPropertyEnumType>(propEnumTypeList);
                }

                return propertyEnumTypes;

            }
        }

        /// <summary>
        /// Column state flag, which describes how the property should be treated by interfaces or APIs that use this flag.
        /// </summary>
        private PropertyColumnStateOptions? columnState;
        /// <summary>
        /// Gets the column state flag, which describes how the property should be treated by interfaces or APIs that use this flag.
        /// </summary>
        public PropertyColumnStateOptions ColumnState
        {
            get
            {
                // If default/first value, try to get it again, otherwise used the cached one.
                if (NativePropertyDescription != null && columnState == null)
                {
                    var hr = NativePropertyDescription.GetColumnState(out var state);

                    if (hr.IsSuccess)
                    {
                        columnState = state;
                    }
                }

                return columnState ?? default;
            }
        }


        /// <summary>
        /// The condition type to use when displaying the property in the query builder user interface (UI).
        /// </summary>
        private PropertyConditionType? conditionType;
        
        /// <summary>
        /// Gets the condition type to use when displaying the property in the query builder user interface (UI).
        /// This influences the list of predicate conditions (for example, equals, less than, and contains) that are shown for this property.
        /// </summary>
        /// <remarks>For more information, see the <c>conditionType</c> attribute of the <c>typeInfo</c> element in the property's .propdesc file.</remarks>
        public PropertyConditionType ConditionType
        {
            get
            {
                // If default/first value, try to get it again, otherwise used the cached one.
                if (NativePropertyDescription != null && conditionType == null)
                {
                    var hr = NativePropertyDescription.GetConditionType(out var tempConditionType, out var tempConditionOperation);
                    if (hr.IsSuccess)
                    {
                        conditionOperation = tempConditionOperation;
                        conditionType = tempConditionType;
                    }
                }

                return conditionType ?? default;
            }
        }

        /// <summary>
        /// Default condition operation to use when displaying the property in the query builder user  interface (UI)
        /// </summary>
        private PropertyConditionOperation? conditionOperation;
        /// <summary>
        /// Gets the default condition operation to use when displaying the property in the query builder user interface (UI).
        /// This influences the list of predicate conditions (for example, equals, less than, and contains) that are shown for this property.
        /// </summary>
        /// <remarks>For more information, see the <c>conditionType</c> attribute of the <c>typeInfo</c> element in the property's .propdesc file.</remarks>
        public PropertyConditionOperation ConditionOperation
        {
            get
            {
                // If default/first value, try to get it again, otherwise used the cached one.
                if (NativePropertyDescription != null && conditionOperation == null)
                {
                    var hr = NativePropertyDescription.GetConditionType(out var tempConditionType, out var tempConditionOperation);

                    if (hr.IsSuccess)
                    {
                        conditionOperation = tempConditionOperation;
                        conditionType = tempConditionType;
                    }
                }

                return conditionOperation ?? default;
            }
        }

        /// <summary>
        /// Method used when a view is grouped by this property
        /// </summary>
        private PropertyGroupingRange? groupingRange;
        /// <summary>
        /// Gets the method used when a view is grouped by this property.
        /// </summary>
        /// <remarks>The information retrieved by this method comes from the <c>groupingRange</c> attribute of the <c>typeInfo</c> element in the property's .propdesc file.</remarks>
        public PropertyGroupingRange GroupingRange
        {
            get
            {
                // If default/first value, try to get it again, otherwise used the cached one.
                if (NativePropertyDescription != null && groupingRange == null)
                {
                    var hr = NativePropertyDescription.GetGroupingRange(out var tempGroupingRange);

                    if (hr.IsSuccess)
                    {
                        groupingRange = tempGroupingRange;
                    }
                }

                return groupingRange ?? default;
            }
        }

        /// <summary>
        /// Current sort description flags for the property, which indicate the particular wordings of sort offerings.
        /// </summary>
        private PropertySortDescription? sortDescription;
        /// <summary>
        /// Gets the current sort description flags for the property, which indicate the particular wordings of sort offerings.
        /// </summary>
        /// <remarks>The settings retrieved by this method are set 
        /// through the <c>sortDescription</c> attribute of the <c>labelInfo</c> 
        /// element in the property's .propdesc file.</remarks>
        public PropertySortDescription SortDescription
        {
            get
            {
                // If default/first value, try to get it again, otherwise used the cached one.
                if (NativePropertyDescription != null && sortDescription == null)
                {
                    var hr = NativePropertyDescription.GetSortDescription(out var tempSortDescription);

                    if (hr.IsSuccess)
                    {
                        sortDescription = tempSortDescription;
                    }
                }

                return sortDescription ?? default;
            }
        }

        /// <summary>
        /// Gets the localized display string that describes the current sort order.
        /// </summary>
        /// <param name="descending">Indicates the sort order should 
        /// reference the string "Z on top"; otherwise, the sort order should reference the string "A on top".</param>
        /// <returns>The sort description for this property.</returns>
        /// <remarks>The string retrieved by this method is determined by flags set in the 
        /// <c>sortDescription</c> attribute of the <c>labelInfo</c> element in the property's .propdesc file.</remarks>
        public string GetSortDescriptionLabel(bool descending)
        {
            var label = string.Empty;
            if (NativePropertyDescription != null)
            {
                var hr = NativePropertyDescription.GetSortDescriptionLabel(descending, out var ptr);

                if (hr.IsSuccess && ptr != IntPtr.Zero)
                {
                    label = Marshal.PtrToStringUni(ptr)??string.Empty;
                    // Free the string
                    Marshal.FreeCoTaskMem(ptr);
                }
            }

            return label;
        }

        /// <summary>
        /// Set of flags that describe the uses and capabilities of the property.
        /// </summary>
        private PropertyTypeOptions? propertyTypeFlags;
        /// <summary>
        /// Gets a set of flags that describe the uses and capabilities of the property.
        /// </summary>
        public PropertyTypeOptions TypeFlags
        {
            get
            {
                if (NativePropertyDescription != null && propertyTypeFlags == null)
                {
                    var hr = NativePropertyDescription.GetTypeFlags(PropertyTypeOptions.MaskAll, out var tempFlags);

                    propertyTypeFlags = hr.IsSuccess ? tempFlags : default;
                }

                return propertyTypeFlags ?? default;
            }
        }

        /// <summary>
        /// Current set of flags governing the property's view
        /// </summary>
        private PropertyViewOptions? propertyViewFlags;
        /// <summary>
        /// Gets the current set of flags governing the property's view.
        /// </summary>
        public PropertyViewOptions ViewFlags
        {
            get
            {
                if (NativePropertyDescription != null && propertyViewFlags == null)
                {
                    var hr = NativePropertyDescription.GetViewFlags(out var tempFlags);

                    propertyViewFlags = hr.IsSuccess ? tempFlags : default;
                }

                return propertyViewFlags ?? default;
            }
        }

        /// <summary>
        /// Gets a value that determines if the native property description is present on the system.
        /// </summary>
        public bool HasSystemDescription => NativePropertyDescription != null;


        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="key">Property key identifying the underlying property</param>
        internal ShellPropertyDescription(PropertyKey key)
        {
            propertyKey = key;
        }

        /// <summary>
        /// Gets the string representation of the object
        /// </summary>
        /// <returns>String representation of the object</returns>
        public override string ToString()
        {
            return $"{CanonicalName} {ShellProperty.VarEnumToString(VarEnumType)} ({DisplayName}:{DisplayType}) {propertyKey}";
        }

        #region IDisposable Members

        /// <summary>
        /// Release the native objects
        /// </summary>
        /// <param name="disposing">Indicates that this is being called from Dispose(), rather than the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (nativePropertyDescription != null)
            {
                Marshal.ReleaseComObject(nativePropertyDescription);
                nativePropertyDescription = null;
            }

            if (disposing)
            {
                // and the managed ones
                canonicalName = null;
                displayName = null;
                editInvitation = null;
                defaultColumnWidth = null;
                valueType = null;
                propertyEnumTypes = null;
            }
        }

        /// <summary>
        /// Release the native objects
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Release the native objects
        /// </summary>
        ~ShellPropertyDescription()
        {
            Dispose(false);
        }

        #endregion
    }
}
