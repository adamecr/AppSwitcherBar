using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;
using net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces;
using net.adamec.ui.AppSwitcherBar.Win32.NativeMethods;

namespace net.adamec.ui.AppSwitcherBar.Win32.Services
{
    /// <summary>
    /// Encapsulates the package related native API
    /// </summary>
    internal static class Package
    {
        /// <summary>
        /// Retrieve a path to package directory for the package given in <paramref name="packageFullName"/>
        /// </summary>
        /// <param name="packageFullName">Full name of package</param>
        /// <returns>Path to package directory or null when the path can't be retrieved</returns>
        public static string? GetPackagePath(string packageFullName)
        {
            var sb=new StringBuilder(1024);
            var len = (uint)sb.Capacity;
            var res=Kernel32.GetPackagePathByFullName(packageFullName, ref len, sb);
            return res == 0 ? sb.ToString() : null;
        }

        /// <summary>
        /// Retrieve a full path to the image asset file for given <paramref name="packageFullName"/> and (base) <paramref name="assetName"/> (ms-appx:///...).
        /// As the package can contain asset in different sizes, scales and contrasts, it will try to get the best match:
        ///  The target size closest to <paramref name="size"/> (the first bigger or the last smaller)
        ///  The highest scale
        ///  White or standard contrast
        /// </summary>
        /// <param name="packageFullName">Full name of package</param>
        /// <param name="assetName">Name of the asset without qualifiers</param>
        /// <param name="size">Required size of the image</param>
        /// <returns>Full path to the image asset file with the best match, or null when no asset can be found</returns>
        [SuppressMessage("ReSharper", "CommentTypo")]
        [SuppressMessage("ReSharper", "IdentifierTypo")]
        public static string? GetPackageImageAsset(string packageFullName, string assetName, int size)
        {
            if(string.IsNullOrEmpty(packageFullName) || string.IsNullOrEmpty(assetName)) return null;

            var basePath = GetPackagePath(packageFullName);
            if(basePath == null || !Directory.Exists(basePath)) return null;

            //ms-appx:///Assets/Standard.png
            const string resProtocol = "ms-appx:///";
            var idx = assetName.IndexOf(resProtocol, StringComparison.InvariantCultureIgnoreCase);
            assetName = idx != -1 ? assetName[(idx + resProtocol.Length)..] : assetName;
            
            //get the required asset directory
            var assetDir = Path.Combine(basePath, Path.GetDirectoryName(assetName)??string.Empty);
            if(!Directory.Exists(assetDir)) return null;

            //get all images for the asset (filename.extension -> filename.qualifiers.extension)
            var assetFileBaseName =Path.GetFileNameWithoutExtension(assetName);
            var assetFileExt=Path.GetExtension(assetName);
            var availableImages = new List<ImgAssetInfo>();
            foreach (var fileName in Directory.GetFiles(assetDir, $"{assetFileBaseName}*{assetFileExt}"))
            {
                var fileBaseName = Path.GetFileNameWithoutExtension(fileName);
                
                var scale = -1;
                var contrast = string.Empty;
                var targetSize = -1;

                var tmpArry = fileBaseName.Split(".");
                if (tmpArry.Length > 1)
                {
                    //parse qualifiers
                    var qualifiers = tmpArry.Last().Split("_");
                    foreach (var qualifier in qualifiers)
                    {
                        tmpArry = qualifier.Split("-");
                        var qualifierName = tmpArry.Length > 0 ? tmpArry[0] : string.Empty;
                        var qualifierValueStr = tmpArry.Length > 1 ? tmpArry[1] : string.Empty;

                        switch (qualifierName)
                        {
                            case "scale" when int.TryParse(qualifierValueStr, out var intScale):
                                scale=intScale;
                                break;
                            case "targetsize" when int.TryParse(qualifierValueStr, out var intTargetSize):
                                targetSize = intTargetSize;
                                break;
                            case "contrast" when !string.IsNullOrEmpty(qualifierValueStr):
                                contrast=qualifierValueStr;
                                break;
                        }
                    }
                }

                var info = new ImgAssetInfo(scale, targetSize, contrast, fileName);
                availableImages.Add(info);
            }

            //try to find the best match

            //best size
            var bestSize = -1;
             //the closest bigger
            var bestSizeArray = availableImages.Where(i => i.targetSize >= size).Select(i => i.targetSize).Distinct().ToArray();
            if (bestSizeArray.Length > 0)
            {
                bestSize = bestSizeArray.Min();
            }
            else
            {
                //the closest smaller
                bestSizeArray = availableImages.Where(i => i.targetSize < size).Select(i => i.targetSize).Distinct().ToArray();
                if (bestSizeArray.Length > 0)
                {
                    bestSize = bestSizeArray.Max();
                }
            }

            //highest scale
            var highScale = availableImages.Where(i => i.targetSize == bestSize).Select(i => i.scale).Max();

            var candidateImages = availableImages.Where(i => i.targetSize == bestSize && i.scale == highScale).ToArray();

            //contrast
            var candidateInfo =
                    candidateImages.FirstOrDefault(i => i.contrast == "white") ??
                    candidateImages.FirstOrDefault(i => i.contrast == "standard") ??
                    candidateImages.FirstOrDefault(i => i.contrast == string.Empty) ??
                    candidateImages.FirstOrDefault();

            var candidateFile = candidateInfo?.fileName;
            return candidateFile;
        }

        /// <summary>
        /// Information about image asset
        /// </summary>
        private class ImgAssetInfo
        {
            /// <summary>
            /// scale qualifier or -1
            /// </summary>
            public readonly int scale;
            /// <summary>
            /// targetsize qualifier or -1
            /// </summary>
            public readonly int targetSize;
            /// <summary>
            /// contrast qualifier of string.Empty
            /// </summary>
            public readonly string contrast;
            /// <summary>
            /// Full path to the image file
            /// </summary>
            public readonly string fileName;

            /// <summary>
            /// CTOR
            /// </summary>
            /// <param name="scale">scale qualifier or -1</param>
            /// <param name="targetSize">targetsize qualifier or -1</param>
            /// <param name="contrast">contrast qualifier of string.Empty</param>
            /// <param name="fileName">Full path to the image file</param>
            public ImgAssetInfo(int scale, int targetSize, string contrast, string fileName)
            {
                this.scale = scale;
                this.targetSize = targetSize;
                this.contrast = contrast;
                this.fileName = fileName;
            }
        }

        /// <summary>
        /// Activates the Store/UWP application with given <paramref name="appId"/> and optional <paramref name="arguments"/>
        /// </summary>
        /// <param name="appId">Application ID (packageFamilyName!application)</param>
        /// <param name="arguments">Optional arguments</param>
        /// <param name="pid">Process ID of the new application process</param>
        /// <returns>True when the application was launched otherwise false</returns>
        public static bool ActivateApplication(string? appId, string? arguments, out uint pid)
        {
            pid = 0;
            if (string.IsNullOrEmpty(appId)) return false;
            var appActiveManager = new ApplicationActivationManager();//Class not registered
            var hr=appActiveManager.ActivateApplication(appId, arguments, ActivateOptions.None, out pid);
            return hr.IsSuccess;
        }
    }
}
