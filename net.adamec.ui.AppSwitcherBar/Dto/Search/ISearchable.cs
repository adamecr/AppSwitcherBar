namespace net.adamec.ui.AppSwitcherBar.Dto.Search
{
    /// <summary>
    /// Searchable item (window or application)
    /// </summary>
    public interface ISearchable
    {
        /// <summary>
        /// Search sort key
        /// </summary>
        string SearchSortKey { get; }

        /// <summary>
        /// Application run statistics
        /// </summary>
        RunStats RunStats { get; }
    }
}
