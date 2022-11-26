using net.adamec.ui.AppSwitcherBar.AppBar;

namespace net.adamec.ui.AppSwitcherBar.Dto
{
    /// <summary>
    /// Information about a screen edge
    /// </summary>
    public class EdgeInfo
    {
        /// <summary>
        /// Dock mode for the edge
        /// </summary>
        public AppBarDockMode DockMode { get; }
        /// <summary>
        /// Name of the edge
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="dockMode">Dock mode for the edge</param>
        /// <param name="name">Name of the edge</param>
        public EdgeInfo(AppBarDockMode dockMode,string? name)
        {
            DockMode=dockMode;
            Name = name??DockMode.ToString();
        }
    }
}
