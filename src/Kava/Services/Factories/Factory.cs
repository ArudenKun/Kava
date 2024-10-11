using System;
using Kava.Services.Abstractions.Factories;

namespace Kava.Services.Factories;

public class Factory<T>(Func<T> initFunc) : IFactory<T>
{
    public T Create() => initFunc();
}