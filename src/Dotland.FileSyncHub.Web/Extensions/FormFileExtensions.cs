using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Dotland.FileSyncHub.Web.Extensions;

/// <summary>
/// Extension methods for IFormFile.
/// </summary>
public static class FormFileExtensions
{
    /// <summary>
    /// Converts an IFormFile to a byte array.
    /// </summary>
    /// <param name="formFile">The form file to convert.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Byte array containing the file content.</returns>
    public static async Task<byte[]> ToByteArrayAsync(
        this IFormFile? formFile,
        CancellationToken cancellationToken)
    {
        if (formFile == null || formFile.Length == 0)
        {
            return Array.Empty<byte>();
        }

        using var memoryStream = new MemoryStream();
        await formFile.CopyToAsync(memoryStream, cancellationToken);
        return memoryStream.ToArray();
    }
}
