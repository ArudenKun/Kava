// namespace Kava.Core.Extensions;
//
// public static class FlurlExtensions
// {
//     public static async Task DownloadAsync(
//         this string url,
//         string filePath,
//         IProgress<double>? progress = null,
//         int bufferSize = 81920,
//         bool overwrite = true,
//         HttpCompletionOption completionOption = HttpCompletionOption.ResponseHeadersRead,
//         CancellationToken cancellationToken = default
//     )
//     {
//         var response = await url.GetAsync(completionOption, cancellationToken);
//
//         if (!response.ResponseMessage.IsSuccessStatusCode)
//         {
//             return;
//         }
//
//         var totalLength = response.ResponseMessage.Content.Headers.ContentLength ?? 0L;
//         var contentStream = await response.ResponseMessage.Content.ReadAsStreamAsync(
//             cancellationToken
//         );
//
//         var fileStream = await FileSystemHelper.OpenWriteAsync(filePath, overwrite, bufferSize);
//
//         try
//         {
//             await contentStream.CopyToAsync(
//                 fileStream,
//                 totalLength,
//                 bufferSize,
//                 progress,
//                 cancellationToken
//             );
//         }
//         finally
//         {
//             await fileStream.DisposeAsync();
//             await contentStream.DisposeAsync();
//         }
//     }
// }
