/** @addtogroup vlUnitySDK
 *  @{
 */

using System;

/// <summary>
///  VLTrackingIssues stores the issues arrising during startup
/// More information on possible codes: @ref tracking-init-issues
/// Such an object is usually passed, when subscribing to the OnTrackerInitializedWithIssues event.
/// </summary>
[Serializable]
public class VLTrackingIssues
{
    /// <summary>
    ///  TrackingIssue stores an issue when tracking or when initialized
    /// </summary>
    [Serializable]
    public class VLTrackingIssue
    {
        /// <summary>The english human readable info on the error/warning.</summary>
        /// <remarks>
        ///  This might be matter of change - but can help to understand the issue
        ///  Do not rely on the string except that it holds special information like the device name etc..
        /// </remarks>
        public string info;

        /// <summary>The unique code for the error</summary>
        /// <remarks>
        ///  Currently the following codes are possible:
        /// </remarks>
        public int code;

        /// <summary>The error level</summary>
        /// <remarks>
        ///  0=Notification,1=Warning,2=Error
        /// </remarks>
        public int level;

    }

    /// <summary>
    ///  Array with the tracking state of all tracking objects.
    /// </summary>
    public VLTrackingIssue[] issues;

    /// <summary>
    /// String message describing the global reason. Have a look into the
    /// issues array in order to get a machine readable impression of what
    /// was going wrong or what happened.
    /// </summary>
    public string message;

    /// <summary>
    /// Checks if the issues contain some code.
    /// </summary>
    /// <returns><c>true</c>, if code was found, <c>false</c> false.</returns>
    /// <param name="code">the code that describes the issue (please revise @ref tracking-init-issues)</param>
    public bool hasCode(int code)
    {
        if (issues == null)
        {
            return false;
        }
        foreach (VLTrackingIssue issue in issues)
        {
            if (issue.code == code)
            {
                return true;
            }
        }
        return false;

    }

    /// <summary>
    /// Checks if the issues contain some code and returns the associated info string.
    /// </summary>
    /// <returns><c>string</c>, if code was found, <c>""</c> if no code has been found.</returns>
    /// <param name="code">the code that describes the issue (please revise @ref tracking-init-issues)</param>
    public string getInfoForCode(int code)
    {
        if (issues == null)
        {
            return "";
        }
        foreach (VLTrackingIssue issue in issues)
        {
            if (issue.code == code)
            {
                return issue.info;
            }
        }
        return "";
    }
}
/**@}*/
