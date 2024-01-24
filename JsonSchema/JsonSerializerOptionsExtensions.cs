﻿using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Json.Schema;

// TODO: .NET 5+ would have these methods marked with `RequiresUnreferencedCodeAttribute` to warn against tree trimming
internal static class JsonSerializerOptionsExtensions
{
	private abstract class ArbitraryDeserializer
	{
		private static readonly ConcurrentDictionary<Type, ArbitraryDeserializer> _deserializerCache = new();

		public static ArbitraryDeserializer GetConverter(Type arbitraryType)
		{
			return _deserializerCache.GetOrAdd(arbitraryType, t => (ArbitraryDeserializer)Activator.CreateInstance(typeof(ArbitraryDeserializer<>).MakeGenericType(t)));
		}

		public abstract object? Read(ref Utf8JsonReader reader, JsonSerializerOptions options);
	}

	private class ArbitraryDeserializer<T> : ArbitraryDeserializer
	{
		public override object? Read(ref Utf8JsonReader reader, JsonSerializerOptions options)
		{
			var converter = (JsonConverter<T>)options.GetConverter(typeof(T));

			return converter.Read(ref reader, typeof(T), options);
		}
	}

	internal static object? Read(this JsonSerializerOptions options, ref Utf8JsonReader reader, Type arbitraryType)
	{
		//var typeinfo = options.GetTypeInfo(arbitraryType);
		//typeinfo.Deserializea
		//JsonSerializer.Deserialize(reader, arbitraryType, options);

		var converter = ArbitraryDeserializer.GetConverter(arbitraryType);
		return converter.Read(ref reader, options);
	}
}
