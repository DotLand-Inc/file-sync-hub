namespace Dotland.FileSyncHub.Domain.Enums;

/// <summary>
/// Document lifecycle status
/// </summary>
public enum DocumentStatus
{
    /// <summary>
    /// Document is in draft state, not yet submitted
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Document is pending review/approval
    /// </summary>
    PendingReview = 1,

    /// <summary>
    /// Document is under review
    /// </summary>
    UnderReview = 2,

    /// <summary>
    /// Document has been approved
    /// </summary>
    Approved = 3,

    /// <summary>
    /// Document has been rejected
    /// </summary>
    Rejected = 4,

    /// <summary>
    /// Document is published and active
    /// </summary>
    Published = 5,

    /// <summary>
    /// Document is archived
    /// </summary>
    Archived = 6,

    /// <summary>
    /// Document is marked for deletion
    /// </summary>
    Deleted = 7
}
