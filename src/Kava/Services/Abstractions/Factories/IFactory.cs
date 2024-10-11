namespace Kava.Services.Abstractions.Factories;

public interface IFactory<out T> : IService
{
    T Create();
}