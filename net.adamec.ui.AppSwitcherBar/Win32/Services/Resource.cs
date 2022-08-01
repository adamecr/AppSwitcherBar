using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using net.adamec.ui.AppSwitcherBar.Win32.NativeMethods;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.Services
{
    /// <summary>
    /// Encapsulates the resource related native API
    /// </summary>
    internal static class Resource
    {
        /// <summary>
        /// Retrieves the resource string from library using the <paramref name="resourceRoute"/> in format "[@]library,-id"
        /// </summary>
        /// <param name="resourceRoute">Resource route such as "@%systemdir%\system32\shell32.dll,-19263". The "@" character is optional</param>
        /// <returns>The resource string or null when the <paramref name="resourceRoute"/> is invalid or resource is not found</returns>
        public static string? GetResourceString(string resourceRoute)
        {
            if (!ParseResourceRoute(resourceRoute, true, out var libPath, out int resourceId)) return null;
            var hLibrary = IntPtr.Zero;
            try
            {
                hLibrary = Kernel32.LoadLibrary(libPath!);
                if (hLibrary != IntPtr.Zero)
                {
                    var sb = new StringBuilder(1024);
                    var len = User32.LoadString(hLibrary, resourceId, sb, sb.Capacity);
                    if (len > 0)
                    {
                        return sb.ToString();
                    }
                }
            }
            finally
            {
                if (hLibrary != IntPtr.Zero) Kernel32.FreeLibrary(hLibrary);
            }

            return null;
        }

        /// <summary>
        /// Retrieves the icon from library or .ico file identified by the <paramref name="resourceRoute"/> in format "[@]library,-id"
        /// </summary>
        /// <param name="resourceRoute">Resource route such as "%systemdir%\system32\shell32.dll,-128" or "%programfiles%\Microsoft\Notepad\Notepad.ico,0".</param>
        /// <param name="iconSize">Required icon size (small or large)</param>
        /// <returns>The icon as <see cref="BitmapSource"/> or null when the <paramref name="resourceRoute"/> is invalid or icon is not found</returns>
        public static BitmapSource? GetResourceIcon(string resourceRoute, IconSizeEnum iconSize)
        {
            if (!ParseResourceRoute(resourceRoute, false, out var libPath, out var resourceId)) return null;
            var hIcon = IntPtr.Zero;
            try
            {
                var iconArrySmall = iconSize == IconSizeEnum.Small ? new IntPtr[1] : null;
                var iconArryLarge = iconSize == IconSizeEnum.Large ? new IntPtr[1] : null;

                var num = Shell32.ExtractIconEx(libPath!, resourceId, iconArryLarge, iconArrySmall, 1);
                if (num == 1)
                {
                    hIcon = iconSize == IconSizeEnum.Small ? iconArrySmall![0] : iconArryLarge![0];
                    if (hIcon != IntPtr.Zero)
                    {
                        var bitmapSourceFromHIcon = Imaging.CreateBitmapSourceFromHIcon(hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        bitmapSourceFromHIcon.Freeze();

                        return bitmapSourceFromHIcon;
                    }
                }
            }
            finally
            {
                if (hIcon != IntPtr.Zero) User32.DestroyIcon(hIcon);
            }

            return null;
        }


        /// <summary>
        /// Inverts the bitmap in case it consist of white pixels (with different aplha) only
        /// </summary>
        /// <param name="source">Bitmap to check and invert if white only</param>
        /// <returns>Inverted bitmap if consist of white pixels only otherwise the <paramref name="source"/> bitmap is returned</returns>
        public static BitmapSource? InvertBitmapIfWhiteOnly(BitmapSource? source)
        {
            const int whiteThreshold = 245;

            if (source == null) return null;
            // Calculate stride of source
            var stride = (source.PixelWidth * source.Format.BitsPerPixel + 7) / 8;

            // Create data array to hold source pixel data
            var length = stride * source.PixelHeight;
            var data = new byte[length];

            // Copy source image pixels to the data array
            source.CopyPixels(data, stride, 0);

            //Check if is white only
            var isWhiteOnly = true;

            for (var i = 0; i < length; i += 4)
            {
                var r = data[i];
                var g = data[i + 1];
                var b = data[i + 2];
                var a = data[i + 3];
                if (a != 0 && (r < whiteThreshold | g < whiteThreshold | b < whiteThreshold)) isWhiteOnly = false;
            }

            if (!isWhiteOnly) return source;
            //invert
            for (var i = 0; i < length; i += 4)
            {
                data[i] = (byte)(255 - data[i]); //R
                data[i + 1] = (byte)(255 - data[i + 1]); //G
                data[i + 2] = (byte)(255 - data[i + 2]); //B
                //data[i + 3] =  data[i + 3]); //keep A
            }

            // Create a new BitmapSource from the inverted pixel buffer
            return BitmapSource.Create(
                    source.PixelWidth, source.PixelHeight,
                    source.DpiX, source.DpiY, source.Format,
                    null, data, stride);
        }

        /// <summary>
        /// Parses resource route in format "[@]library,[-]id" from <paramref name="resourceRoute"/>
        /// "@" character is optional, "-" is optional unless <paramref name="requireDashBeforeId"/> is true
        /// </summary>
        /// <param name="resourceRoute">Resource route in format "[@]library,[-]id"</param>
        /// <param name="requireDashBeforeId">Flag whether the "-" character before resource id is required</param>
        /// <param name="filePath">File path parsed from <paramref name="resourceRoute"/></param>
        /// <param name="resourceId">Resource ID parsed from <paramref name="resourceRoute"/> including the "-" if present</param>
        /// <returns>True when the parsing was successful and the file at <paramref name="filePath"/> exists, otherwise false</returns>
        private static bool ParseResourceRoute(string resourceRoute, bool requireDashBeforeId, out string? filePath, out int resourceId)
        {
            filePath = null;
            resourceId = 0;

            if (string.IsNullOrEmpty(resourceRoute) || resourceRoute == "@") return false;
            if (resourceRoute.StartsWith("@")) resourceRoute = resourceRoute[1..];
            var resourceRouteParts = resourceRoute.Split(',');
            if (resourceRouteParts.Length != 2) return false;

            filePath = resourceRouteParts[0];
            if (string.IsNullOrEmpty(filePath)) return false;
            if (!File.Exists(filePath)) return false;

            var resourceIdStr = resourceRouteParts[1];
            if (string.IsNullOrEmpty(resourceIdStr) || resourceIdStr == "-" || (requireDashBeforeId && !resourceIdStr.StartsWith("-"))) return false;

            return int.TryParse(resourceIdStr, out resourceId);
        }

        /// <summary>
        /// Extracts a specified text resource from package when given that resource in the form of an indirect string (a string that begins with the '@' symbol or ms-resource:// protocol).
        /// </summary>
        /// <param name="resource">Indirect string of resource</param>
        /// <param name="packageFullName">Full name of the package containing the resource</param>
        /// <returns>Extracted resource string or
        /// "original" <paramref name="resource"/> when the <paramref name="resource"/> is not indirect string or
        /// null when <paramref name="resource"/> is the indirect string but the extraction fails</returns>
        public static string? GetIndirectString(string resource, string? packageFullName)
        {
            //@filename,resource
            //@{PRIFilepath?resource}
            //@{PackageFullName?resource} 
            //ms-resource://resource

            const string resProtocol = "ms-resource://";
            if (string.IsNullOrEmpty(resource)) return null;
            if (!resource.StartsWith("@") && !resource.StartsWith(resProtocol)) return resource; //not indirect string, just copy

            var sb = new StringBuilder(1024);
            HRESULT hr;

            if (resource.StartsWith("@"))
            {
                //try to use the resource as it is
                hr = ShlwApi.SHLoadIndirectString(resource, sb, sb.Capacity, IntPtr.Zero);
                if (hr.IsSuccess) return sb.ToString();
            }

            if (string.IsNullOrEmpty(packageFullName)) return resource; //can't do anything more

            var idx = resource.IndexOf(resProtocol, StringComparison.InvariantCultureIgnoreCase);
            var resourceName = idx != -1 ? resource[(idx + resProtocol.Length)..] : resource; //if there is no ms-resource, take the whole resource as identifier
            if (string.IsNullOrEmpty(resourceName)) return null; //can't do anything with it

            var resourceKey = $"@{{{packageFullName}?{resProtocol}{resourceName}}}";
            sb = new StringBuilder(1024);
            hr = ShlwApi.SHLoadIndirectString(resourceKey, sb, sb.Capacity, IntPtr.Zero);
            return hr.IsSuccess ? sb.ToString() : null;
        }
    }

    /// <summary>
    /// Size of the icon
    /// </summary>
    internal enum IconSizeEnum
    {
        /// <summary>
        /// Small icon
        /// </summary>
        Small,
        /// <summary>
        /// Large icon
        /// </summary>
        Large,
    }
}
