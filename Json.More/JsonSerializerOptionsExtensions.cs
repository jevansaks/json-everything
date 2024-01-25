﻿using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Json.More;

// TODO: .NET 5+ would have these methods marked with `RequiresUnreferencedCodeAttribute` to warn against tree trimming
/// <summary>
/// Provides extension functionality for <see cref="JsonSerializerOptions"/>.
/// </summary>
public static class JsonSerializerOptionsExtensions
{
	/// <summary>
	/// Returns the converter for the specified type.
	/// </summary>
	/// <typeparam name="T">The <see cref="Type"/> to convert.</typeparam>
	/// <param name="options">The <see cref="JsonSerializerOptions"/> being used.</param>
	/// <returns>An implementation of <see cref="JsonConverter<>"/> as determined by the provided options</returns>
	public static JsonConverter<T> GetConverter<T>(this JsonSerializerOptions options)
	{
		if (options.TryGetTypeInfo(typeof(T), out var typeinfo))
		{
			return (JsonConverter<T>)typeinfo.Converter;
		}

		return (JsonConverter<T>)options.GetConverter(typeof(T));
	}

	/// <summary>
	/// Read and convert the JSON to T.
	/// </summary>
	/// <remarks>
	/// A converter may throw any Exception, but should throw <cref>JsonException</cref> when the JSON is invalid.
	/// </remarks>
	/// <typeparam name="T">The <see cref="Type"/> to convert.</typeparam>
	/// <param name="options">The <see cref="JsonSerializerOptions"/> being used.</param>
	/// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
	/// <returns>The value that was converted.</returns>
	public static T? Read<T>(this JsonSerializerOptions options, ref Utf8JsonReader reader)
	{
		return options.GetConverter<T>().Read(ref reader, typeof(T), options);
	}
}
