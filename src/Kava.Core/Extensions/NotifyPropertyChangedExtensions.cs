﻿using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using Kava.Core.Utilities;

namespace Kava.Core.Extensions;

public static class NotifyPropertyChangedExtensions
{
    public static IDisposable WatchProperty<TOwner, TProperty>(
        this TOwner? owner,
        Expression<Func<TOwner, TProperty>> propertyExpression,
        Action callback,
        bool watchInitialValue = false
    )
        where TOwner : INotifyPropertyChanged?
    {
        var memberExpression = propertyExpression.Body as MemberExpression;
        if (memberExpression?.Member is not PropertyInfo property)
            throw new ArgumentException("Provided expression must reference a property.");

        ArgumentNullException.ThrowIfNull(owner);

        owner.PropertyChanged += OnPropertyChanged;

        if (watchInitialValue)
            callback();

        return Disposable.Create(() => owner.PropertyChanged -= OnPropertyChanged);

        void OnPropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            if (
                string.IsNullOrWhiteSpace(args.PropertyName)
                || string.Equals(args.PropertyName, property.Name, StringComparison.Ordinal)
            )
            {
                callback();
            }
        }
    }

    public static IDisposable WatchAllProperties<TOwner>(
        this TOwner? owner,
        Action callback,
        bool watchInitialValues = false
    )
        where TOwner : INotifyPropertyChanged?
    {
        ArgumentNullException.ThrowIfNull(owner);

        owner.PropertyChanged += OnPropertyChanged;

        if (watchInitialValues)
            callback();

        return Disposable.Create(() => owner.PropertyChanged -= OnPropertyChanged);

        void OnPropertyChanged(object? sender, PropertyChangedEventArgs args) => callback();
    }
}
