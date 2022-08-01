namespace net.adamec.ui.AppSwitcherBar.Win32.Services.Startup;

/// <summary>
/// Dummy implementation of <see cref="IStartupService"/> with internal dummy state
/// </summary>
public class DummyStartupService : IStartupService
{
    /// <summary>
    /// Internal dummy state
    /// </summary>
    private bool dummyStartupSet;

    /// <summary>
    /// Set the internal dummy state to true
    /// </summary>
    /// <param name="description">Ignored</param>
    /// <returns>Always true</returns>
    public bool CreateAppStartupLink(string? description)
    {
        dummyStartupSet = true;
        return true;
    }

    /// <summary>
    /// Returns the internal dummy state
    /// </summary>
    /// <returns>Internal dummy state</returns>
    public bool HasAppStartupLink()
    {
        return dummyStartupSet;
    }

    /// <summary>
    /// Set the internal dummy state to false
    /// </summary>
    /// <returns>Always true</returns>
    public bool RemoveAppStartupLink()
    {
        dummyStartupSet = false;
        return true;
    }
}