using System.IO;

namespace Kava.Services.Abstractions.Factories;

public interface IFileDbFactory : IFactory<FileDb>
{
    FileDb Create(FileAccess fileAccess);
}