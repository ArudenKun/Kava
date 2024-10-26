using System.Text.Json.Serialization;
using Kava.Services;

namespace Kava;

[JsonSerializable(typeof(SettingsService))]
[JsonSourceGenerationOptions(WriteIndented = true, UseStringEnumConverter = true)]
public partial class KavaJsonContext : JsonSerializerContext;
