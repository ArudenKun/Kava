using System.Text.Json.Serialization;
using Kava.Models;
using Kava.Services;

namespace Kava;

[JsonSerializable(typeof(Board))]
[JsonSerializable(typeof(Category))]
[JsonSerializable(typeof(Card))]
[JsonSerializable(typeof(Comment))]
[JsonSerializable(typeof(Attachment))]
[JsonSerializable(typeof(SettingsService))]
[JsonSourceGenerationOptions(WriteIndented = true, UseStringEnumConverter = true)]
public partial class AppJsonContext : JsonSerializerContext;
