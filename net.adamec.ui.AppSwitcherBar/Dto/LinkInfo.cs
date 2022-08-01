using System.Windows.Media.Imaging;

namespace net.adamec.ui.AppSwitcherBar.Dto
{
    /// <summary>
    /// Information about the shell link
    /// </summary>
    public class LinkInfo
    {
        /// <summary>
        /// Special <see cref="LinkInfo"/> representing the separator in JumpList
        /// </summary>
        public static LinkInfo Separator { get; } = new() { IsSeparator = true };

        /// <summary>
        /// Flag whether the <see cref="LinkInfo"/> represents the separator or link
        /// </summary>
        public bool IsSeparator { get; init; }

        /// <summary>
        /// Name of the category the link belongs to
        /// </summary>
        public string? Category { get; }

        /// <summary>
        /// Name of the link
        /// </summary>
        public string? Name { get; }
        /// <summary>
        /// Description of the link
        /// </summary>
        public string? Description { get; }
        /// <summary>
        /// Target path to file to be executed
        /// </summary>
        public string? TargetPath { get; }
        /// <summary>
        /// Target start arguments
        /// </summary>
        public string? Arguments { get; }
        /// <summary>
        /// Target working directory
        /// </summary>
        public string? WorkingDirectory { get; }
        /// <summary>
        /// Link icon location
        /// </summary>
        public string? IconLocation { get; }
        /// <summary>
        /// Link icon index within <see cref="IconLocation"/>
        /// </summary>
        public int IconIndex { get; }
        /// <summary>
        /// Link icon
        /// </summary>
        public BitmapSource? Icon { get; }

        /// <summary>
        /// Flag whether the <see cref="LinkInfo"/> represents the Store/UWP application
        /// </summary>
        public bool IsStoreApp { get; init; }

        /// <summary>
        /// Source of the link information
        /// </summary>
        public string? Source { get; }

        /// <summary>
        /// Flag whether the link has a target
        /// </summary>
        public bool HasTarget => !string.IsNullOrEmpty(TargetPath);

        /// <summary>
        /// CTOR
        /// </summary>
        private LinkInfo()
        {
            IsSeparator = false;
        }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="category">Category the link belongs to</param>
        /// <param name="name">Name of the link</param>
        /// <param name="description">Description of the link</param>
        /// <param name="targetPath">Target path to file to be executed</param>
        /// <param name="arguments">Target start arguments</param>
        /// <param name="workingDirectory">Target working directory</param>
        /// <param name="iconLocation">Link icon location</param>
        /// <param name="iconIndex">Link icon index within <see cref="IconLocation"/></param>
        /// <param name="icon">Link icon</param>
        /// <param name="isStoreApp">Flag whether the <see cref="LinkInfo"/> represents the Store/UWP application</param>
        /// <param name="source">Source of the link information</param>
        public LinkInfo(
            string category,
            string name, string? description,
            string? targetPath, string? arguments, string? workingDirectory,
            string? iconLocation, int iconIndex, BitmapSource? icon,
            bool isStoreApp,
            string? source = null) : this()
        {
            Category = category;
            Name = name;
            Description = description;
            TargetPath = targetPath;
            Arguments = arguments;
            WorkingDirectory = workingDirectory;
            IconLocation = iconLocation;
            IconIndex = iconIndex;
            Icon = icon;
            IsStoreApp = isStoreApp;
            Source = source;
        }

        /// <summary>
        /// Gets the string representation of object
        /// </summary>
        /// <returns>String representation of object</returns>
        public override string ToString()
        {
            return $"{Source} {Name}:{TargetPath} {Arguments} ({Description}) Icon:{IconLocation},{IconIndex}";
        }
    }
}
