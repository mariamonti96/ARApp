/** @addtogroup vlUnitySDK
 *  @{
 */

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

/// <summary>
///  The VLDataSetWrapper is a wrapper for the DataSet. The DataSet stores a
///  collection of DataBase objects.
/// </summary>
/// <remarks>
///  <para>
///   Since the DataSet inherits from the DataBase class it's possible that
///   a DataSet contains other DataSets (composite pattern).
///  </para>
///  <para>
///   The AbstractApplication stores a global DataSet and forwards it to all
///   Actions when applying the Actions. That way the Actions can exchange data
///   without needing to know each other.
///  </para>
/// </remarks>
/// <seealso cref="VLDataBaseWrapper"/>
/// <seealso cref="VLAbstractApplicationWrapper.GetDataSet"/>
public class VLDataSetWrapper: IDisposable
{
    private IntPtr handle;
    private bool disposed = false;
    private bool owner;

    /// <summary>
    ///  Constructor of VLDataBaseWrapper.
    /// </summary>
    /// <remarks>
    ///  Don't call this constructor directly. Use the
    ///  <see cref="VLAbstractApplicationWrapper.GetDataSet"/> and / or
    ///  <see cref="VLDataSetWrapper.GetDataSet"/> methods instead.
    /// </remarks>
    /// <param name="handle">
    ///  Handle to the native object.
    /// </param>
    /// <param name="owner">
    ///  <c>true</c>, if the VLDataSetWrapper is the owner of the native
    ///  object; <c>false</c>, otherwise.
    /// </param>
    public VLDataSetWrapper(IntPtr handle, bool owner)
    {
        this.handle = handle;
        this.owner = owner;
    }

    ~VLDataSetWrapper()
    {
        // The finalizer was called implicitly from the garbage collector
        this.Dispose(false);
    }

    /// <summary>
    ///  Returns the handle to the native object.
    /// </summary>
    /// <returns>
    ///  Handle to native object.
    /// </returns>
    public IntPtr GetHandle()
    {
        return this.handle;
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern void vlDelete_DataSetWrapper(
        IntPtr dataSetWrapper);
    private void Dispose(bool disposing)
    {
        // Prevent multiple calls to Dispose
        if (this.disposed)
        {
            return;
        }

        // Was dispose called explicitly by the user?
        if (disposing)
        {
            // Dispose managed resources (those that implement IDisposable)
        }

        // Clean up unmanaged resources
        if (this.owner)
        {
            vlDelete_DataSetWrapper(this.handle);
        }
        this.handle = IntPtr.Zero;

        this.disposed = true;
    }

    /// <summary>
    ///  Explicitly releases references to unmanaged resources.
    /// </summary>
    /// <remarks>
    ///  Call <see cref="Dispose"/> when you are finished using the
    ///  <see cref="VLDataSetWrapper"/>. The <see cref="Dispose"/> method
    ///  leaves the <see cref="VLDataSetWrapper"/> in an unusable state.
    ///  After calling <see cref="Dispose"/>, you must release all references
    ///  to the <see cref="VLDataSetWrapper"/> so the garbage
    ///  collector can reclaim the memory that the
    ///  <see cref="VLDataSetWrapper"/> was occupying.
    /// </remarks>
    public void Dispose()
    {
        Dispose(true); // Dispose was explicitly called by the user
        GC.SuppressFinalize(this);
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern IntPtr vlDataSetWrapper_GetDataBase(
        IntPtr dataSetWrapper,
        [MarshalAs(UnmanagedType.LPStr)] string key);
    /// <summary>
    ///  Find the DataBase object with the given key.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The DataBase object will be searched only within the own sub-database
    ///   objects. If you want to get a DataBase object within an other DataSet,
    ///   you also need to specify the key of the containing DataSet like this:
    ///   'subDataSetKey.dataBaseKey'.
    ///  </para>
    ///  <para>
    ///   You must call the Dispose method of the returned object after you are
    ///   done using it. Failure to do so will result in leaked memory.
    ///  </para>
    /// </remarks>
    /// <returns>
    ///  <c>VLDataBaseWrapper</c>, if DataBase was found;
    ///  <c>null</c>, otherwise.
    /// </returns>
    /// <param name="key">Key of the DataBase object.</param>
    public VLDataBaseWrapper GetDataBase(string key)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLDataSetWrapper");
        }

        IntPtr dataBaseHandle = vlDataSetWrapper_GetDataBase(this.handle, key);

        if (dataBaseHandle != IntPtr.Zero)
        {
            return new VLDataBaseWrapper(dataBaseHandle, true);
        }
        else
        {
            return null;
        }
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern IntPtr vlDataSetWrapper_GetDataSet(
        IntPtr dataSetWrapper,
        [MarshalAs(UnmanagedType.LPStr)] string key);
    /// <summary>
    ///  Find the DataBase object with the given key.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The DataSet object will be searched only within the own sub-database
    ///   objects. If you want to get a DataSet object within an other DataSet,
    ///   you also need to specify the key of the containing DataSet like this:
    ///   'subDataSetKey.subSubDataSetKey'.
    ///  </para>
    ///  <para>
    ///   You must call the Dispose method of the returned object after you are
    ///   done using it. Failure to do so will result in leaked memory.
    ///  </para>
    /// </remarks>
    /// <returns>
    ///  <c>VLDataSetWrapper</c>, if DataSet was found;
    ///  <c>null</c>, otherwise.
    /// </returns>
    /// <param name="key">Key of the DataSet object.</param>
    public VLDataSetWrapper GetDataSet(string key)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLDataSetWrapper");
        }

        IntPtr dataSetHandle = vlDataSetWrapper_GetDataSet(this.handle, key);

        if (dataSetHandle != IntPtr.Zero)
        {
            return new VLDataSetWrapper(dataSetHandle, true);
        }
        else
        {
            return null;
        }
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern IntPtr vlDataSetWrapper_GetImage(
        IntPtr dataSetWrapper,
        [MarshalAs(UnmanagedType.LPStr)] string key);
    /// <summary>
    ///  Find the Image with the given key.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The Image will be searched only within the own sub-database objects.
    ///   If you want to get an Image within an other DataSet, you also need to
    ///   specify the key of the containing DataSet like this:
    ///   'subDataSetKey.imageKey'.
    ///  </para>
    ///  <para>
    ///   You must call the Dispose method of the returned object after you are
    ///   done using it. Failure to do so will result in leaked memory.
    ///  </para>
    /// </remarks>
    /// <returns>
    ///  <c>VLImageWrapper</c>, if Image was found;
    ///  <c>null</c>, otherwise.
    /// </returns>
    /// <param name="key">Key of the DataSet object.</param>
    public VLImageWrapper GetImage(string key)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLDataSetWrapper");
        }

        IntPtr imageHandle = vlDataSetWrapper_GetImage(this.handle, key);

        if (imageHandle != IntPtr.Zero)
        {
            return new VLImageWrapper(imageHandle, true);
        }
        else
        {
            return null;
        }
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern IntPtr vlDataSetWrapper_GetExtrinsicData(
        IntPtr dataSetWrapper,
        [MarshalAs(UnmanagedType.LPStr)] string key);
    /// <summary>
    ///  Find the ExtrinsicData object with the given key.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The ExtrinsicData object will be searched only within the own
    ///   sub-database objects. If you want to get an ExtrinsicData object
    ///   within an other DataSet, you also need to specify the key of the
    ///   containing DataSet like this: 'subDataSetKey.extrinsicDataKey'.
    ///  </para>
    ///  <para>
    ///   You must call the Dispose method of the returned object after you are
    ///   done using it. Failure to do so will result in leaked memory.
    ///  </para>
    /// </remarks>
    /// <returns>
    ///  <c>VLExtrinsicDataWrapper</c>, if ExtrinsicData object was found;
    ///  <c>null</c>, otherwise.
    /// </returns>
    /// <param name="key">Key of the DataSet object.</param>
    public VLExtrinsicDataWrapper GetExtrinsicData(string key)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLDataSetWrapper");
        }

        IntPtr extrinsicDataHandle = vlDataSetWrapper_GetExtrinsicData(
            this.handle, key);

        if (extrinsicDataHandle != IntPtr.Zero)
        {
            return new VLExtrinsicDataWrapper(extrinsicDataHandle, true);
        }
        else
        {
            return null;
        }
    }

    [DllImport (VLUnitySdk.dllName)]
    private static extern IntPtr vlDataSetWrapper_GetIntrinsicData(
        IntPtr dataSetWrapper,
        [MarshalAs(UnmanagedType.LPStr)] string key);
    /// <summary>
    ///  Find the IntrinsicData object with the given key.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The IntrinsicData object will be searched only within the own
    ///   sub-database objects. If you want to get an IntrinsicData object
    ///   within an other DataSet, you also need to specify the key of the
    ///   containing DataSet like this: 'subDataSetKey.intrinsicDataKey'.
    ///  </para>
    ///  <para>
    ///   You must call the Dispose method of the returned object after you are
    ///   done using it. Failure to do so will result in leaked memory.
    ///  </para>
    /// </remarks>
    /// <returns>
    ///  <c>VLIntrinsicDataWrapper</c>, if IntrinsicData object was found;
    ///  <c>null</c>, otherwise.
    /// </returns>
    /// <param name="key">Key of the DataSet object.</param>
    public VLIntrinsicDataWrapper GetIntrinsicData(string key)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("VLDataSetWrapper");
        }

        IntPtr intrinsicDataHandle = vlDataSetWrapper_GetIntrinsicData(
            this.handle, key);

        if (intrinsicDataHandle != IntPtr.Zero)
        {
            return new VLIntrinsicDataWrapper(intrinsicDataHandle, true);
        }
        else
        {
            return null;
        }
    }
}

/**@}*/