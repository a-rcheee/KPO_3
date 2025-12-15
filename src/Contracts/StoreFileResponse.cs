namespace Contracts;

public sealed record StoreFileResponse(
    string FileId,
    string OriginalFileName,
    string Sha256
);