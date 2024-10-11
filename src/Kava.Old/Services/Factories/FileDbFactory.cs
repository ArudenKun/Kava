using System.IO;
using Kava.Services.Abstractions.Factories;

namespace Kava.Services.Factories;

public class FileDbFactory(string fileDbPath) : IFileDbFactory
{
    public FileDb Create() => new(fileDbPath, FileAccess.ReadWrite);

    public FileDb Create(FileAccess fileAccess) => new(fileDbPath, fileAccess);
}