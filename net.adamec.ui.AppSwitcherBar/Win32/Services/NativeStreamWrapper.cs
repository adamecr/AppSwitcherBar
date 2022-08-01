using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
// ReSharper disable CommentTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.Services
{

    /// <summary>
    /// Wrapper over the .NET <see cref="Stream"/> providing the simple readonly implementation of native <see cref="IStream"/>
    /// </summary>
    public class NativeStreamWrapper : IStream
    {
        private readonly Stream stream;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="stream">.NET <see cref="Stream"/> to be read using the native functions</param>
        /// <exception cref="ArgumentNullException"></exception>
        public NativeStreamWrapper(Stream stream)
        {
            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        /// <summary>
        /// The Clone method creates a new stream object with its own seek pointer that references the same bytes as the original stream.
        /// </summary>
        /// <param name="ppstm">When successful, pointer to the location of an IStream pointer to the new stream object. If an error occurs, this parameter is NULL.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Clone(out IStream ppstm)
        {
            throw new NotImplementedException(nameof(Clone));
        }

        /// <summary>
        /// The Commit method ensures that any changes made to a stream object open in transacted mode are reflected in the parent storage.
        /// </summary>
        /// <param name="grfCommitFlags">Controls how the changes for the stream object are committed. See the STGC enumeration for a definition of these values.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Commit(int grfCommitFlags)
        {
            throw new NotImplementedException(nameof(Commit));
        }

        /// <summary>
        /// The CopyTo method copies a specified number of bytes from the current seek pointer in the stream to the current seek pointer in another stream.
        /// </summary>
        /// <param name="pstm">A pointer to the destination stream. The stream pointed to by pstm can be a new stream or a clone of the source stream.</param>
        /// <param name="cb">The number of bytes to copy from the source stream.</param>
        /// <param name="pcbRead">A pointer to the location where this method writes the actual number of bytes read from the source. You can set this pointer to NULL. In this case, this method does not provide the actual number of bytes read.</param>
        /// <param name="pcbWritten">A pointer to the location where this method writes the actual number of bytes written to the destination. You can set this pointer to NULL. In this case, this method does not provide the actual number of bytes written.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten)
        {
            throw new NotImplementedException(nameof(CopyTo));
        }

        /// <summary>
        /// The LockRegion method restricts access to a specified range of bytes in the stream. Supporting this functionality is optional since some file systems do not provide it.
        /// </summary>
        /// <param name="libOffset">Integer that specifies the byte offset for the beginning of the range.</param>
        /// <param name="cb">Integer that specifies the length of the range, in bytes, to be restricted.</param>
        /// <param name="dwLockType">Specifies the restrictions being requested on accessing the range.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void LockRegion(long libOffset, long cb, int dwLockType)
        {
            throw new NotImplementedException(nameof(LockRegion));
        }

        /// <summary>
        /// The Read method reads a specified number of bytes from the stream object into memory, starting at the current seek pointer.
        /// </summary>
        /// <param name="pv">A pointer to the buffer which the stream data is read into.</param>
        /// <param name="cb">The number of bytes of data to read from the stream object.</param>
        /// <param name="pcbRead">A pointer to a ULONG variable that receives the actual number of bytes read from the stream object.</param>
        public void Read(byte[] pv, int cb, IntPtr pcbRead)
        {
            var r = stream.Read(pv, 0, cb);
            if (pcbRead != IntPtr.Zero) Marshal.WriteInt64(pcbRead, r);
        }

        /// <summary>
        /// The Revert method discards all changes that have been made to a transacted stream since the last IStream::Commit call.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Revert()
        {
            throw new NotImplementedException(nameof(Revert));
        }

        /// <summary>
        /// The Seek method changes the seek pointer to a new location. The new location is relative to either the beginning of the stream, the end of the stream, or the current seek pointer.
        /// </summary>
        /// <param name="dlibMove">The displacement to be added to the location indicated by the dwOrigin parameter. If dwOrigin is STREAM_SEEK_SET, this is interpreted as an unsigned value rather than a signed value.</param>
        /// <param name="dwOrigin">The origin for the displacement specified in dlibMove. The origin can be the beginning of the file (STREAM_SEEK_SET), the current seek pointer (STREAM_SEEK_CUR), or the end of the file (STREAM_SEEK_END). For more information about values, see the STREAM_SEEK enumeration.</param>
        /// <param name="plibNewPosition">A pointer to the location where this method writes the value of the new seek pointer from the beginning of the stream.
        /// You can set this pointer to NULL.In this case, this method does not provide the new seek pointer.</param>
        public void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition)
        {
            var s = stream.Seek(dlibMove, (SeekOrigin)dwOrigin);
            if (plibNewPosition != IntPtr.Zero) Marshal.WriteInt64(plibNewPosition, s);
        }

        /// <summary>
        /// The SetSize method changes the size of the stream object.
        /// </summary>
        /// <param name="libNewSize">Specifies the new size, in bytes, of the stream.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void SetSize(long libNewSize)
        {
            throw new NotImplementedException(nameof(SetSize));
        }

        /// <summary>
        /// The Stat method retrieves the STATSTG structure for this stream.
        /// </summary>
        /// <param name="pstatstg">Pointer to a STATSTG structure where this method places information about this stream object.</param>
        /// <param name="grfStatFlag">Specifies that this method does not return some of the members in the STATSTG structure, thus saving a memory allocation operation. Values are taken from the STATFLAG enumeration.</param>
        public void Stat(out STATSTG pstatstg, int grfStatFlag)
        {
            pstatstg = new STATSTG() { cbSize = stream.Length };
        }

        /// <summary>
        /// The UnlockRegion method removes the access restriction on a range of bytes previously restricted with IStream::LockRegion.
        /// </summary>
        /// <param name="libOffset">Specifies the byte offset for the beginning of the range.</param>
        /// <param name="cb">Specifies, in bytes, the length of the range to be restricted.</param>
        /// <param name="dwLockType">Specifies the access restrictions previously placed on the range.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void UnlockRegion(long libOffset, long cb, int dwLockType)
        {
            throw new NotImplementedException(nameof(UnlockRegion));
        }

        /// <summary>
        /// The Write method writes a specified number of bytes into the stream object starting at the current seek pointer.
        /// </summary>
        /// <param name="pv">A pointer to the buffer that contains the data that is to be written to the stream. A valid pointer must be provided for this parameter even when cb is zero.</param>
        /// <param name="cb">The number of bytes of data to attempt to write into the stream. This value can be zero.</param>
        /// <param name="pcbWritten">A pointer to a ULONG variable where this method writes the actual number of bytes written to the stream object. The caller can set this pointer to NULL, in which case this method does not provide the actual number of bytes written.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Write(byte[] pv, int cb, IntPtr pcbWritten)
        {
            throw new NotImplementedException(nameof(Write));
        }
    }
}
