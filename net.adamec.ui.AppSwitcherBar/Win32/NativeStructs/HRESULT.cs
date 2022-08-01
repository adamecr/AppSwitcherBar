using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeStructs
{
    /// <summary>
    /// Wrapper for HRESULT status codes.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal readonly struct HRESULT:IEquatable<HRESULT>
    {
        /// <summary>
        /// Status code value
        /// </summary>
        [FieldOffset(0)]
        private readonly uint value;

        ///<summary>S_OK</summary>
        public static readonly HRESULT S_OK = new(0x00000000);
        ///<summary>S_FALSE</summary>
        public static readonly HRESULT S_FALSE = new(0x00000001);
        ///<summary>E_NOTIMPL</summary>
        public static readonly HRESULT E_NOTIMPL = new(0x80004001);
        ///<summary>E_NOINTERFACE</summary>
        public static readonly HRESULT E_NOINTERFACE = new(0x80004002);
        ///<summary>E_POINTER</summary>
        public static readonly HRESULT E_POINTER = new(0x80004003);
        ///<summary>E_ABORT</summary>
        public static readonly HRESULT E_ABORT = new(0x80004004);
        ///<summary>E_FAIL</summary>
        public static readonly HRESULT E_FAIL = new(0x80004005);
        ///<summary>E_UNEXPECTED</summary>
        public static readonly HRESULT E_UNEXPECTED = new(0x8000FFFF);
        ///<summary>DISP_E_MEMBERNOTFOUND</summary>
        public static readonly HRESULT DISP_E_MEMBERNOTFOUND = new(0x80020003);
        ///<summary>DISP_E_TYPEMISMATCH</summary>
        public static readonly HRESULT DISP_E_TYPEMISMATCH = new(0x80020005);
        ///<summary>DISP_E_UNKNOWNNAME</summary>
        public static readonly HRESULT DISP_E_UNKNOWNNAME = new(0x80020006);
        ///<summary>DISP_E_EXCEPTION</summary>
        public static readonly HRESULT DISP_E_EXCEPTION = new(0x80020009);
        ///<summary>DISP_E_OVERFLOW</summary>
        public static readonly HRESULT DISP_E_OVERFLOW = new(0x8002000A);
        ///<summary>DISP_E_BADINDEX</summary>
        public static readonly HRESULT DISP_E_BADINDEX = new(0x8002000B);
        ///<summary>DISP_E_BADPARAMCOUNT</summary>
        public static readonly HRESULT DISP_E_BADPARAMCOUNT = new(0x8002000E);
        ///<summary>DISP_E_PARAMNOTOPTIONAL</summary>
        public static readonly HRESULT DISP_E_PARAMNOTOPTIONAL = new(0x8002000F);
        ///<summary>SCRIPT_E_REPORTED</summary>
        public static readonly HRESULT SCRIPT_E_REPORTED = new(0x80020101);
        ///<summary>TYPE_E_ELEMENTNOTFOUND</summary>
        public static readonly HRESULT TYPE_E_ELEMENTNOTFOUND = new(0x8002802B);
        ///<summary>STG_E_INVALIDFUNCTION</summary>
        public static readonly HRESULT STG_E_INVALIDFUNCTION = new(0x80030001);
        ///<summary>DESTS_E_NO_MATCHING_ASSOC_HANDLER.</summary>
        public static readonly HRESULT DESTS_E_NO_MATCHING_ASSOC_HANDLER = new(0x80040F03);
        ///<summary>NO_OBJECT</summary>
        public static readonly HRESULT NO_OBJECT = new(0x800401E5);
        ///<summary>E_ACCESSDENIED</summary>
        public static readonly HRESULT E_ACCESSDENIED = new(0x80070005);
        ///<summary>E_OUTOFMEMORY</summary>
        public static readonly HRESULT E_OUTOFMEMORY = new(0x8007000E);
        ///<summary>E_INVALIDARG</summary>
        public static readonly HRESULT E_INVALIDARG = new(0x80070057);
        ///<summary>E_ELEMENTNOTFOUND</summary>
        public static readonly HRESULT E_ELEMENTNOTFOUND = new(0x80070490);
        ///<summary>ERROR_CANCELLED</summary>
        public static readonly HRESULT ERROR_CANCELLED = new(0x800704C7);
        ///<summary>COR_E_OBJECTDISPOSED</summary>
        public static readonly HRESULT COR_E_OBJECTDISPOSED = new(0x80131622);
        ///<summary>WC_E_GREATERTHAN</summary>
        public static readonly HRESULT WC_E_GREATERTHAN = new(0xC00CEE23);
        ///<summary>WC_E_SYNTAX</summary>
        public static readonly HRESULT WC_E_SYNTAX = new(0xC00CEE2D);

        /// <summary>
        /// Map of know codes (values) to names
        /// </summary>
        private static Dictionary<uint, string>? KnownCodes;

        /// <summary>
        /// Is HRESULT success (>=0)?
        /// </summary>
        public bool IsSuccess => (int)value >= 0;
        
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="val">Status code</param>
        public HRESULT(uint val)
        {
            value = val;
        }

       

        /// <summary>
        /// Get a string representation of this HRESULT.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // ReSharper disable once InvertIf
            if (KnownCodes == null)
            {
                KnownCodes = new Dictionary<uint, string>();
                var fields = typeof(HRESULT)
                    .GetFields(BindingFlags.Static | BindingFlags.Public)
                    .Where(f => f.FieldType == typeof(HRESULT)).ToArray();

                foreach (var fieldInfo in fields)
                {
                    var nam = fieldInfo.Name;
                    var val= (HRESULT)fieldInfo.GetValue(null)!;
                    KnownCodes.Add(val.value,nam);
                }
            }

            return KnownCodes.TryGetValue(this.value,out var name)? name: string.Format(CultureInfo.InvariantCulture, "0x{0:X8}", value);
        }

        public bool Equals(HRESULT other)
        {
            return value == other.value;
        }

        public override bool Equals(object? obj)
        {
            return obj is HRESULT other && Equals(other);
        }
        
       
        public static bool operator ==(HRESULT hrLeft, HRESULT hrRight)
        {
            return hrLeft.value == hrRight.value;
        }

        public static bool operator !=(HRESULT hrLeft, HRESULT hrRight)
        {
            return !(hrLeft == hrRight);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
    }
}
