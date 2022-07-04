using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace net.adamec.ui.AppSwitcherBar.Win32.ShellExt
{
    /// <summary>
    /// Represents the OLE struct PROPVARIANT.
    /// This class is intended for internal use only.
    /// </summary>
    /// <remarks>
    /// Based on PropVariant.cs of Windows API Code Pack 1.1
    /// Originally sourced from http://blogs.msdn.com/adamroot/pages/interop-with-propvariants-in-net.aspx
    /// and modified to support additional types including vectors and ability to set values
    /// </remarks>
    [SuppressMessage("Microsoft.Portability", "CA1900:ValueTypeFieldsShouldBePortable", MessageId = "_ptr2"),
     SuppressMessage("ReSharper", "BitwiseOperatorOnEnumWithoutFlags"),
     StructLayout(LayoutKind.Explicit),]
    internal sealed class PropVariant : IDisposable
    {
        #region Vector Action Cache

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
                        PropVariantNativeMethods.PropVariantGetInt16Elem(pv, i, out var val);
                        array.SetValue(val, i);
                    }
                },
                {
                    typeof(ushort), (pv, array, i) =>
                    {
                        PropVariantNativeMethods.PropVariantGetUInt16Elem(pv, i, out var val);
                        array.SetValue(val, i);
                    }
                },
                { typeof(int), (pv, array, i) =>
                {
                    PropVariantNativeMethods.PropVariantGetInt32Elem(pv, i, out var val);
                    array.SetValue(val, i);
                } },
                { typeof(uint), (pv, array, i) =>
                {
                    PropVariantNativeMethods.PropVariantGetUInt32Elem(pv, i, out var val);
                    array.SetValue(val, i);
                } },
                { typeof(long), (pv, array, i) =>
                {
                    PropVariantNativeMethods.PropVariantGetInt64Elem(pv, i, out var val);
                    array.SetValue(val, i);
                } },
                { typeof(ulong), (pv, array, i) =>
                {
                    PropVariantNativeMethods.PropVariantGetUInt64Elem(pv, i, out var val);
                    array.SetValue(val, i);
                } },
                { typeof(DateTime), (pv, array, i) =>
                {
                    PropVariantNativeMethods.PropVariantGetFileTimeElem(pv, i, out var val);
                    var fileTime = GetFileTimeAsLong(ref val);
                    array.SetValue(DateTime.FromFileTime(fileTime), i);
                } },
                { typeof(bool), (pv, array, i) =>
                {
                    PropVariantNativeMethods.PropVariantGetBooleanElem(pv, i, out var val);
                    array.SetValue(val, i);
                } },
                { typeof(double), (pv, array, i) =>
                {
                    PropVariantNativeMethods.PropVariantGetDoubleElem(pv, i, out var val);
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
                    var val = string.Empty;
                    PropVariantNativeMethods.PropVariantGetStringElem(pv, i, ref val);
                    array.SetValue(val, i);
                } }
            };

            return newCache;
        }


        #endregion

        #region Dynamic Construction / Factory (Expressions)

        /// <summary>
        /// Attempts to create a PropVariant by finding an appropriate constructor.
        /// </summary>
        /// <param name="value">Object from which PropVariant should be created.</param>
        public static PropVariant FromObject(object? value)
        {
            if (value == null) return new PropVariant();

            var func = GetDynamicConstructor(value.GetType());
            return func(value);
        }

        // A dictionary and lock to contain compiled expression trees for constructors
        private static readonly Dictionary<Type, Func<object, PropVariant>> cache = new();
        private static readonly object padlock = new();

        /// <summary>
        /// Retrieves a cached constructor expression.
        /// If no constructor has been cached, it attempts to find/add it.  If it cannot be found an exception is thrown.
        /// This method looks for a public constructor with the same parameter type as the object.
        /// </summary>
        /// <param name="type">NET <see cref="Type"/> to base the PropVariant on</param>
        /// <returns>Cached constructor expression</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="type"/> is not supported</exception>
        private static Func<object, PropVariant> GetDynamicConstructor(Type type)
        {
            lock (padlock)
            {
                // initial check, if action is found, return it
                if (cache.TryGetValue(type, out var action)) return action;

                // iterates through all constructors
                var constructor = typeof(PropVariant).GetConstructor(new[] { type });

                // CTOR was not found, throw.
                if (constructor == null) throw new ArgumentException("PropVariant Type not supported", nameof(type));

                // CTOR was found, create an expression to call it.

                // create parameters to action                    
                var arg = Expression.Parameter(typeof(object), "arg");

                // create an expression to invoke the constructor with an argument cast to the correct type
                var create = Expression.New(constructor, Expression.Convert(arg, type));

                // compiles expression into an action delegate
                action = Expression.Lambda<Func<object, PropVariant>>(create, arg).Compile();
                cache.Add(type, action);
                return action;
            }
        }

        #endregion

        #region Fields

        [FieldOffset(0)] readonly decimal decimalValue;

        // This is actually a VarEnum value, but the VarEnum type
        // requires 4 bytes instead of the expected 2.
        [FieldOffset(0)] ushort valueTypeValue;

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
        [SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
        [FieldOffset(12)] readonly IntPtr intPtrValue2;

        [SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
        [FieldOffset(8)] readonly IntPtr intPtrValue;
        [FieldOffset(8)] readonly int intValue;
        [FieldOffset(8)] readonly uint uintValue;
        [FieldOffset(8)] readonly byte byteValue;
        [FieldOffset(8)] readonly sbyte sbyteValue;
        [FieldOffset(8)] readonly short shortValue;
        [FieldOffset(8)] readonly ushort ushortValue;
        [FieldOffset(8)] readonly long longValue;
        [FieldOffset(8)] readonly ulong ulongValue;
        [FieldOffset(8)] readonly double doubleValue;
        [FieldOffset(8)] readonly float floatValue;

        #endregion // struct fields

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public PropVariant()
        {
            // left empty
        }

        /// <summary>
        /// Set a string value
        /// </summary>
        public PropVariant(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            valueTypeValue = (ushort)VarEnum.VT_LPWSTR;
            intPtrValue = Marshal.StringToCoTaskMemUni(value);
        }

        /// <summary>
        /// Set a string vector
        /// </summary>
        public PropVariant(string[] value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            PropVariantNativeMethods.InitPropVariantFromStringVector(value, (uint)value.Length, this);
        }

        /// <summary>
        /// Set a bool vector
        /// </summary>
        public PropVariant(bool[] value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            PropVariantNativeMethods.InitPropVariantFromBooleanVector(value, (uint)value.Length, this);
        }

        /// <summary>
        /// Set a short vector
        /// </summary>
        public PropVariant(short[] value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            PropVariantNativeMethods.InitPropVariantFromInt16Vector(value, (uint)value.Length, this);
        }

        /// <summary>
        /// Set a short vector
        /// </summary>
        public PropVariant(ushort[] value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            PropVariantNativeMethods.InitPropVariantFromUInt16Vector(value, (uint)value.Length, this);

        }

        /// <summary>
        /// Set an int vector
        /// </summary>
        public PropVariant(int[] value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            PropVariantNativeMethods.InitPropVariantFromInt32Vector(value, (uint)value.Length, this);
        }

        /// <summary>
        /// Set an uint vector
        /// </summary>
        public PropVariant(uint[] value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            PropVariantNativeMethods.InitPropVariantFromUInt32Vector(value, (uint)value.Length, this);
        }

        /// <summary>
        /// Set a long vector
        /// </summary>
        public PropVariant(long[] value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            PropVariantNativeMethods.InitPropVariantFromInt64Vector(value, (uint)value.Length, this);
        }

        /// <summary>
        /// Set a ulong vector
        /// </summary>
        public PropVariant(ulong[] value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            PropVariantNativeMethods.InitPropVariantFromUInt64Vector(value, (uint)value.Length, this);
        }

        /// <summary>>
        /// Set a double vector
        /// </summary>
        public PropVariant(double[] value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            PropVariantNativeMethods.InitPropVariantFromDoubleVector(value, (uint)value.Length, this);
        }


        /// <summary>
        /// Set a DateTime vector
        /// </summary>
        public PropVariant(DateTime[] value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            var fileTimeArr = new FILETIME[value.Length];
            for (var i = 0; i < value.Length; i++)
            {
                fileTimeArr[i] = DateTimeToFileTime(value[i]);
            }

            PropVariantNativeMethods.InitPropVariantFromFileTimeVector(fileTimeArr, (uint)fileTimeArr.Length, this);
        }

        /// <summary>
        /// Set a bool value
        /// </summary>
        public PropVariant(bool value)
        {
            valueTypeValue = (ushort)VarEnum.VT_BOOL;
            intValue = value ? -1 : 0;
        }

        /// <summary>
        /// Set a DateTime value
        /// </summary>
        public PropVariant(DateTime value)
        {
            valueTypeValue = (ushort)VarEnum.VT_FILETIME;

            var ft = DateTimeToFileTime(value);
            PropVariantNativeMethods.InitPropVariantFromFileTime(ref ft, this);
        }


        /// <summary>
        /// Set a byte value
        /// </summary>
        public PropVariant(byte value)
        {
            valueTypeValue = (ushort)VarEnum.VT_UI1;
            byteValue = value;
        }

        /// <summary>
        /// Set a sbyte value
        /// </summary>
        public PropVariant(sbyte value)
        {
            valueTypeValue = (ushort)VarEnum.VT_I1;
            sbyteValue = value;
        }

        /// <summary>
        /// Set a short value
        /// </summary>
        public PropVariant(short value)
        {
            valueTypeValue = (ushort)VarEnum.VT_I2;
            shortValue = value;
        }

        /// <summary>
        /// Set an unsigned short value
        /// </summary>
        public PropVariant(ushort value)
        {
            valueTypeValue = (ushort)VarEnum.VT_UI2;
            ushortValue = value;
        }

        /// <summary>
        /// Set an int value
        /// </summary>
        public PropVariant(int value)
        {
            valueTypeValue = (ushort)VarEnum.VT_I4;
            intValue = value;
        }

        /// <summary>
        /// Set an unsigned int value
        /// </summary>
        public PropVariant(uint value)
        {
            valueTypeValue = (ushort)VarEnum.VT_UI4;
            uintValue = value;
        }

        /// <summary>
        /// Set a decimal  value
        /// </summary>
        public PropVariant(decimal value)
        {
            decimalValue = value;

            // It is critical that the value type be set after the decimal value, because they overlap.
            // If valueType is written first, its value will be lost when _decimal is written.
            valueTypeValue = (ushort)VarEnum.VT_DECIMAL;
        }

        /// <summary>
        /// Create a PropVariant with a contained decimal array.
        /// </summary>
        /// <param name="value">Decimal array to wrap.</param>
        public PropVariant(decimal[] value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            valueTypeValue = (ushort)(VarEnum.VT_DECIMAL | VarEnum.VT_VECTOR);
            intValue = value.Length;

            // allocate required memory for array with 128bit elements
            intPtrValue2 = Marshal.AllocCoTaskMem(value.Length * sizeof(decimal));
            foreach (var d in value)
            {
                var bits = decimal.GetBits(d);
                Marshal.Copy(bits, 0, intPtrValue2, bits.Length);
            }
        }

        /// <summary>
        /// Create a PropVariant containing a float type.
        /// </summary>        
        public PropVariant(float value)
        {
            valueTypeValue = (ushort)VarEnum.VT_R4;
            floatValue = value;
        }

        /// <summary>
        /// Creates a PropVariant containing a float[] array.
        /// </summary>        
        public PropVariant(float[] value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            valueTypeValue = (ushort)(VarEnum.VT_R4 | VarEnum.VT_VECTOR);
            intValue = value.Length;

            intPtrValue2 = Marshal.AllocCoTaskMem(value.Length * sizeof(float));

            Marshal.Copy(value, 0, intPtrValue2, value.Length);
        }

        /// <summary>
        /// Set a long
        /// </summary>
        public PropVariant(long value)
        {
            longValue = value;
            valueTypeValue = (ushort)VarEnum.VT_I8;
        }

        /// <summary>
        /// Set a ulong
        /// </summary>
        public PropVariant(ulong value)
        {
            valueTypeValue = (ushort)VarEnum.VT_UI8;
            ulongValue = value;
        }

        /// <summary>
        /// Set a double
        /// </summary>
        public PropVariant(double value)
        {
            valueTypeValue = (ushort)VarEnum.VT_R8;
            doubleValue = value;
        }

        #endregion

        #region public Properties

        /// <summary>
        /// Gets or sets the variant type.
        /// </summary>
        public VarEnum VarType
        {
            get => (VarEnum)valueTypeValue;
            set => valueTypeValue = (ushort)value;
        }

        /// <summary>
        /// Checks if this has an empty or null value
        /// </summary>
        /// <returns></returns>
        public bool IsNullOrEmpty => valueTypeValue is (ushort)VarEnum.VT_EMPTY or (ushort)VarEnum.VT_NULL;

        /// <summary>
        /// Gets the variant value.
        /// </summary>
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
                    case (VarEnum.VT_VECTOR | VarEnum.VT_LPWSTR):
                        return GetVector<string>();
                    case (VarEnum.VT_VECTOR | VarEnum.VT_I2):
                        return GetVector<short>();
                    case (VarEnum.VT_VECTOR | VarEnum.VT_UI2):
                        return GetVector<ushort>();
                    case (VarEnum.VT_VECTOR | VarEnum.VT_I4):
                        return GetVector<int>();
                    case (VarEnum.VT_VECTOR | VarEnum.VT_UI4):
                        return GetVector<uint>();
                    case (VarEnum.VT_VECTOR | VarEnum.VT_I8):
                        return GetVector<long>();
                    case (VarEnum.VT_VECTOR | VarEnum.VT_UI8):
                        return GetVector<ulong>();
                    case (VarEnum.VT_VECTOR | VarEnum.VT_R4):
                        return GetVector<float>();
                    case (VarEnum.VT_VECTOR | VarEnum.VT_R8):
                        return GetVector<double>();
                    case VarEnum.VT_VECTOR | VarEnum.VT_BOOL:
                        return GetVector<bool>();
                    case (VarEnum.VT_VECTOR | VarEnum.VT_FILETIME):
                        return GetVector<DateTime>();
                    case (VarEnum.VT_VECTOR | VarEnum.VT_DECIMAL):
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
            return (((long)val.dwHighDateTime) << 32) + val.dwLowDateTime;
        }

        private static FILETIME DateTimeToFileTime(DateTime value)
        {
            var fileTime = value.ToFileTime();
            var ft = new FILETIME
            {
                dwLowDateTime = (int)(fileTime & 0xFFFFFFFF),
                dwHighDateTime = (int)(fileTime >> 32)
            };
            return ft;
        }

        private object GetBlobData()
        {
            var blobData = new byte[intValue];
            Marshal.Copy(intPtrValue2, blobData, 0, intValue);
            return blobData;
        }

        private Array? GetVector<T>()
        {
            var count = PropVariantNativeMethods.PropVariantGetElementCount(this);
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
            var cDims = PropVariantNativeMethods.SafeArrayGetDim(psa);
            if (cDims != 1) throw new ArgumentException("Multidimensional array is not supported", nameof(psa));

            var lBound = PropVariantNativeMethods.SafeArrayGetLBound(psa, 1U);
            var uBound = PropVariantNativeMethods.SafeArrayGetUBound(psa, 1U);

            var n = uBound - lBound + 1; // uBound is inclusive

            var array = new object[n];
            for (var i = lBound; i <= uBound; ++i)
            {
                array[i] = PropVariantNativeMethods.SafeArrayGetElement(psa, ref i);
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
            PropVariantNativeMethods.PropVariantClear(this);
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
