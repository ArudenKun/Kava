using System.Text.Json.Serialization;
using Kava.Models;
using Kava.Services;

namespace Kava;

[JsonSerializable(typeof(ConfigService))]
[JsonSerializable(typeof(Board))]
[JsonSerializable(typeof(Card))]
[JsonSerializable(typeof(Entry))]
[JsonSerializable(typeof(Comment))]
[JsonSerializable(typeof(Attachment))]
[JsonSourceGenerationOptions(WriteIndented = true, UseStringEnumConverter = true)]
public partial class AppJsonContext : JsonSerializerContext;
