using System;

namespace net.adamec.ui.AppSwitcherBar.Dto
{
    /// <summary>
    /// Container holding the pair of <see cref="string"/> and <see cref="Guid"/> values
    /// </summary>
    internal class StringGuidPair
    {
        
        /// <summary>
        /// String value
        /// </summary>
        public string String { get; }
        /// <summary>
        /// Guid value
        /// </summary>
        public Guid Guid { get; }

        /// <summary>
        /// Guid value represented as string
        /// </summary>
        public string GuidStr { get; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="s">String value</param>
        /// <param name="guid">Guid value</param>
        public StringGuidPair(string s, Guid guid)
        {
            String = s;
            Guid = guid;
            GuidStr = $"{Guid:B}";
        }

        /// <summary>
        /// Gets the string representation of the object
        /// </summary>
        /// <returns>String representation of the object</returns>
        public override string ToString()
        {
            return $"{String} - {Guid:B}";
        }
    }
}
