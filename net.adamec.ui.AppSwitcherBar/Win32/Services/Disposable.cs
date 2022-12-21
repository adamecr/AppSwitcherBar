using System;

namespace net.adamec.ui.AppSwitcherBar.Win32.Services;

/// <summary>
/// Base for classes that need to be <see cref="IDisposable"/>
/// </summary>
public abstract class Disposable : IDisposable
{
    /// <summary>
    /// Flag whether the object has already been disposed
    /// </summary>

    private bool isDisposed;
    /// <summary>
    /// Flag whether the object has already been disposed
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public bool IsDisposed => isDisposed;

    /// <summary>
    /// Disposes the object.
    /// </summary>
    public void Dispose()
    {
        // ReSharper disable once InvertIf
        if (!isDisposed)
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
    /// <summary>
    /// Disposes the object.
    /// </summary>
    /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (isDisposed) return;

        DoDispose();

        isDisposed = true;
    }

    /// <summary>
    /// Dispose the managed and unmanaged resources
    /// </summary>
    protected abstract void DoDispose();

    /// <summary>
    /// Destructs the object
    /// </summary>
    ~Disposable()
    {
        Dispose(false);
    }

}