using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using net.adamec.ui.AppSwitcherBar.Win32.NativeMethods;

namespace net.adamec.ui.AppSwitcherBar.Win32.Services.Shell.Properties
{
    /// <summary>
    /// Represents the OLE struct PROPVARIANT.
    /// This class is intended for internal use only.
    /// </summary>
    /// <remarks>
    /// Based on PropVariant.cs of Windows API Code Pack 1.1
    /// </remarks>
    [StructLayout(LayoutKind.Explicit)]
    internal sealed class PropVariant : IDisposable
    {
        #region Vector Action Cache
        private static readonly object padlock = new();
        // A static dictionary of delegates to get data from array's contained within PropVariants
        private static Dictionary<Type, Action<PropVariant, Array, uint>>? vectorActions;

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private static Dictionary<Type, Action<PropVariant, Array, uint>> GenerateVectorActions()
        {
            var newCache = new Dictionary<Type, Action<PropVariant, Array, uint>>
            {
                {
                    typeof(short), (pv, array, i) =>
                    {
                        PropSys.PropVariantGetInt16Elem(pv, i, out var val);
                        array.SetValue(val, i);
                    }
                },
                {
                    typeof(ushort), (pv, array, i) =>
                    {
                        PropSys.PropVariantGetUInt16Elem(pv, i, out var val);
                        array.SetValue(val, i);
                    }
                },
                { typeof(int), (pv, array, i) =>
                {
                    PropSys.PropVariantGetInt32Elem(pv, i, out var val);
                    array.SetValue(val, i);
                } },
                { typeof(uint), (pv, array, i) =>
                {
                    PropSys.PropVariantGetUInt32Elem(pv, i, out var val);
                    array.SetValue(val, i);
                } },
                { typeof(long), (pv, array, i) =>
                {
                    PropSys.PropVariantGetInt64Elem(pv, i, out var val);
                    array.SetValue(val, i);
                } },
                { typeof(ulong), (pv, array, i) =>
                {
                    PropSys.PropVariantGetUInt64Elem(pv, i, out var val);
                    array.SetValue(val, i);
                } },
                { typeof(DateTime), (pv, array, i) =>
                {
                    PropSys.PropVariantGetFileTimeElem(pv, i, out var val);
                    var fileTime = GetFileTimeAsLong(ref val);
                    array.SetValue(DateTime.FromFileTime(fileTime), i);
                } },
                { typeof(bool), (pv, array, i) =>
                {
                    PropSys.PropVariantGetBooleanElem(pv, i, out var val);
                    array.SetValue(val, i);
                } },
                { typeof(double), (pv, array, i) =>
                {
                    PropSys.PropVariantGetDoubleElem(pv, i, out var val);
                    array.SetValue(val, i);
                } },
                { typeof(float), (pv, array, i) => // float
                {
                    var val = new float[1];
                    Marshal.Copy(pv.intPtrValue2, val, (int)i, 1);
                    array.SetValue(val[0], (int)i);
                } },
                { typeof(decimal), (pv, array, i) =>
                {
                    var val = new int[4];
                    for (var a = 0; a < val.Length; a++)
                    {
                        val[a] = Marshal.ReadInt32(pv.intPtrValue2,
                            (int)i * sizeof(decimal) + a * sizeof(int)); //index * size + offset quarter
                    }
                    array.SetValue(new decimal(val), i);
                } },
                { typeof(string), (pv, array, i) =>
                {
                    var val = string.Empty;//TODO string vector is not working
                    PropSys.PropVariantGetStringElem(pv, i, ref val);
                    array.SetValue(val, i);
                } }
            };

            return newCache;
        }
        #endregion

        #region Fields
#pragma warning disable CS0649
        [FieldOffset(0)] private readonly decimal decimalValue;


        // This is actually a VarEnum value, but the VarEnum type
        // requires 4 bytes instead of the expected 2.
        [FieldOffset(0)] private ushort valueTypeValue;

        // Reserved Fields
        //[FieldOffset(2)]
        //ushort _wReserved1;
        //[FieldOffset(4)]
        //ushort _wReserved2;
        //[FieldOffset(6)]
        //ushort _wReserved3;

        // In order to allow x64 compat, we need to allow for
        // expansion of the IntPtr. However, the BLOB struct
        // uses a 4-byte int, followed by an IntPtr, so
        // although the valueData field catches most pointer values,
        // we need an additional 4-bytes to get the BLOB
        // pointer. The valueDataExt field provides this, as well as
        // the last 4-bytes of an 8-byte value on 32-bit
        // architectures.
        [FieldOffset(16)]
        private readonly IntPtr intPtrValue2;

        [FieldOffset(8)]
        private readonly IntPtr intPtrValue;
        [FieldOffset(8)] private readonly int intValue;
        [FieldOffset(8)] private readonly uint uintValue;
        [FieldOffset(8)] private readonly byte byteValue;
        [FieldOffset(8)] private readonly sbyte sbyteValue;
        [FieldOffset(8)] private readonly short shortValue;
        [FieldOffset(8)] private readonly ushort ushortValue;
        [FieldOffset(8)] private readonly long longValue;
        [FieldOffset(8)] private readonly ulong ulongValue;
        [FieldOffset(8)] private readonly double doubleValue;
        [FieldOffset(8)] private readonly float floatValue;
#pragma warning restore CS0649
        #endregion // struct fields

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public PropVariant()
        {
            // left empty
        }

        #endregion

        #region public Properties

        /// <summary>
        /// Gets the variant type.
        /// </summary>
        public VarEnum VarType => (VarEnum)valueTypeValue;

        /// <summary>
        /// Checks if this has an empty or null value
        /// </summary>
        /// <returns></returns>
        public bool IsNullOrEmpty => valueTypeValue is (ushort)VarEnum.VT_EMPTY or (ushort)VarEnum.VT_NULL;

        /// <summary>
        /// Gets the variant value.
        /// </summary>
        [SuppressMessage("ReSharper", "BitwiseOperatorOnEnumWithoutFlags")]
        public object? Value
        {
            get
            {
                switch ((VarEnum)valueTypeValue)
                {
                    case VarEnum.VT_I1:
                        return sbyteValue;
                    case VarEnum.VT_UI1:
                        return byteValue;
                    case VarEnum.VT_I2:
                        return shortValue;
                    case VarEnum.VT_UI2:
                        return ushortValue;
                    case VarEnum.VT_I4:
                    case VarEnum.VT_INT:
                        return intValue;
                    case VarEnum.VT_UI4:
                    case VarEnum.VT_UINT:
                        return uintValue;
                    case VarEnum.VT_I8:
                        return longValue;
                    case VarEnum.VT_UI8:
                        return ulongValue;
                    case VarEnum.VT_R4:
                        return floatValue;
                    case VarEnum.VT_R8:
                        return doubleValue;
                    case VarEnum.VT_BOOL:
                        return intValue == -1;
                    case VarEnum.VT_ERROR:
                        return longValue;
                    case VarEnum.VT_CY:
                        return decimalValue;
                    case VarEnum.VT_DATE:
                        return DateTime.FromOADate(doubleValue);
                    case VarEnum.VT_FILETIME:
                        return DateTime.FromFileTime(longValue);
                    case VarEnum.VT_BSTR:
                        return Marshal.PtrToStringBSTR(intPtrValue);
                    case VarEnum.VT_BLOB:
                        return GetBlobData();
                    case VarEnum.VT_LPSTR:
                        return Marshal.PtrToStringAnsi(intPtrValue);
                    case VarEnum.VT_LPWSTR:
                        return Marshal.PtrToStringUni(intPtrValue);
                    case VarEnum.VT_UNKNOWN:
                        return Marshal.GetObjectForIUnknown(intPtrValue);
                    case VarEnum.VT_DISPATCH:
                        return Marshal.GetObjectForIUnknown(intPtrValue);
                    case VarEnum.VT_DECIMAL:
                        return decimalValue;
                    case VarEnum.VT_ARRAY | VarEnum.VT_UNKNOWN:
                        return CrackSingleDimSafeArray(intPtrValue);
                    case VarEnum.VT_VECTOR | VarEnum.VT_LPWSTR:
                        return GetVector<string>();
                    case VarEnum.VT_VECTOR | VarEnum.VT_I2:
                        return GetVector<short>();
                    case VarEnum.VT_VECTOR | VarEnum.VT_UI2:
                        return GetVector<ushort>();
                    case VarEnum.VT_VECTOR | VarEnum.VT_I4:
                        return GetVector<int>();
                    case VarEnum.VT_VECTOR | VarEnum.VT_UI4:
                        return GetVector<uint>();
                    case VarEnum.VT_VECTOR | VarEnum.VT_I8:
                        return GetVector<long>();
                    case VarEnum.VT_VECTOR | VarEnum.VT_UI8:
                        return GetVector<ulong>();
                    case VarEnum.VT_VECTOR | VarEnum.VT_R4:
                        return GetVector<float>();
                    case VarEnum.VT_VECTOR | VarEnum.VT_R8:
                        return GetVector<double>();
                    case VarEnum.VT_VECTOR | VarEnum.VT_BOOL:
                        return GetVector<bool>();
                    case VarEnum.VT_VECTOR | VarEnum.VT_FILETIME:
                        return GetVector<DateTime>();
                    case VarEnum.VT_VECTOR | VarEnum.VT_DECIMAL:
                        return GetVector<decimal>();
                    default:
                        // if the value cannot be marshaled
                        return null;
                }
            }
        }

        #endregion

        #region Private Methods

        private static long GetFileTimeAsLong(ref FILETIME val)
        {
            return ((long)val.dwHighDateTime << 32) + val.dwLowDateTime;
        }

        private object GetBlobData()
        {
            var blobData = new byte[intValue];
            Marshal.Copy(intPtrValue2, blobData, 0, intValue);
            return blobData;
        }

        private Array? GetVector<T>()
        {
            var count = PropSys.PropVariantGetElementCount(this);
            if (count <= 0) { return null; }

            lock (padlock)
            {
                vectorActions ??= GenerateVectorActions();
            }

            if (!vectorActions.TryGetValue(typeof(T), out var action))
            {
                throw new InvalidCastException($"type {typeof(T)} is not supported");
            }

            Array array = new T[count];
            for (uint i = 0; i < count; i++)
            {
                action(this, array, i);
            }

            return array;
        }

        private static Array CrackSingleDimSafeArray(IntPtr psa)
        {
            var cDims = OleAut32.SafeArrayGetDim(psa);
            if (cDims != 1) throw new ArgumentException("Multidimensional array is not supported", nameof(psa));

            OleAut32.SafeArrayGetLBound(psa, 1U, out var lBound);
            OleAut32.SafeArrayGetUBound(psa, 1U, out var uBound);

            var n = uBound - lBound + 1; // uBound is inclusive

            var array = new object?[n];
            for (var i = lBound; i <= uBound; ++i)
            {
                OleAut32.SafeArrayGetElement(psa, ref i, out array[i]);
            }

            return array;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes the object, calls the clear function.
        /// </summary>
        public void Dispose()
        {
            if (!IsNullOrEmpty)
            {
                Ole32.PropVariantClear(this);
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~PropVariant()
        {
            Dispose();
        }

        #endregion

        /// <summary>
        /// Provides an simple string representation of the contained data and type.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}: {1}", Value, VarType.ToString());
        }

    }
}
