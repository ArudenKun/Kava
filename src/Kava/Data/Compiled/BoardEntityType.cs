﻿// <auto-generated />
using System;
using System.Reflection;
using Kava.Core.Models;
using Kava.Core.Models.Abstractions;
using Kava.Data.Converters;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Sqlite.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#pragma warning disable 219, 612, 618
#nullable disable

namespace Kava.Data.Compiled
{
    internal partial class BoardEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "Kava.Core.Models.Board",
                typeof(Board),
                baseEntityType);

            var id = runtimeEntityType.AddProperty(
                "Id",
                typeof(Ulid),
                propertyInfo: typeof(BaseEntity).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(BaseEntity).GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw,
                maxLength: 26,
                valueConverter: new UlidToStringConverter());
            id.TypeMapping = SqliteStringTypeMapping.Default.Clone(
                comparer: new ValueComparer<Ulid>(
                    (Ulid v1, Ulid v2) => v1.Equals(v2),
                    (Ulid v) => v.GetHashCode(),
                    (Ulid v) => v),
                keyComparer: new ValueComparer<Ulid>(
                    (Ulid v1, Ulid v2) => v1.Equals(v2),
                    (Ulid v) => v.GetHashCode(),
                    (Ulid v) => v),
                providerValueComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                mappingInfo: new RelationalTypeMappingInfo(
                    size: 26),
                converter: new ValueConverter<Ulid, string>(
                    (Ulid x) => x.ToString(),
                    (string x) => Ulid.Parse(x)),
                jsonValueReaderWriter: new JsonConvertedValueReaderWriter<Ulid, string>(
                    JsonStringReaderWriter.Instance,
                    new ValueConverter<Ulid, string>(
                        (Ulid x) => x.ToString(),
                        (string x) => Ulid.Parse(x))));
            id.SetSentinelFromProviderValue("00000000000000000000000000");

            var createdAt = runtimeEntityType.AddProperty(
                "CreatedAt",
                typeof(DateTime),
                propertyInfo: typeof(BaseEntity).GetProperty("CreatedAt", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(BaseEntity).GetField("<CreatedAt>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            createdAt.TypeMapping = SqliteDateTimeTypeMapping.Default;

            var name = runtimeEntityType.AddProperty(
                "Name",
                typeof(string),
                propertyInfo: typeof(Board).GetProperty("Name", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Board).GetField("<Name>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 50);
            name.TypeMapping = SqliteStringTypeMapping.Default;

            var updatedAt = runtimeEntityType.AddProperty(
                "UpdatedAt",
                typeof(DateTime),
                propertyInfo: typeof(BaseEntity).GetProperty("UpdatedAt", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(BaseEntity).GetField("<UpdatedAt>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            updatedAt.TypeMapping = SqliteDateTimeTypeMapping.Default;

            var key = runtimeEntityType.AddKey(
                new[] { id });
            runtimeEntityType.SetPrimaryKey(key);

            return runtimeEntityType;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "Boards");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
