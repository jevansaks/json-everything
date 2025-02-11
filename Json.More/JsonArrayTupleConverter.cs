﻿using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Json.More;

/// <summary>
/// Provides JSON serialization for the <see cref="ValueTuple{T1}"/> family of types.
/// </summary>
public class JsonArrayTupleConverter : JsonConverterFactory
{
	/// <summary>When overridden in a derived class, determines whether the converter instance can convert the specified object type.</summary>
	/// <param name="typeToConvert">The type of the object to check whether it can be converted by this converter instance.</param>
	/// <returns>
	/// <see langword="true" /> if the instance can convert the specified object type; otherwise, <see langword="false" />.</returns>
	public override bool CanConvert(Type typeToConvert)
	{
		return typeToConvert.FullName?.StartsWith("System.ValueTuple`") ?? false;
	}

	/// <summary>Creates a converter for a specified type.</summary>
	/// <param name="typeToConvert">The type handled by the converter.</param>
	/// <param name="options">The serialization options to use.</param>
	/// <returns>A converter which is compatible with <paramref name="typeToConvert" />.</returns>
	public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
	{
		var typeParams = typeToConvert.GetGenericArguments();
		Type converterType = typeParams.Length switch
		{
			1 => typeof(JsonArrayTupleConverter<>).MakeGenericType(typeParams),
			2 => typeof(JsonArrayTupleConverter<,>).MakeGenericType(typeParams),
			3 => typeof(JsonArrayTupleConverter<,,>).MakeGenericType(typeParams),
			4 => typeof(JsonArrayTupleConverter<,,,>).MakeGenericType(typeParams),
			5 => typeof(JsonArrayTupleConverter<,,,,>).MakeGenericType(typeParams),
			6 => typeof(JsonArrayTupleConverter<,,,,,>).MakeGenericType(typeParams),
			7 => typeof(JsonArrayTupleConverter<,,,,,,>).MakeGenericType(typeParams),
			8 => typeof(JsonArrayTupleConverter<,,,,,,,>).MakeGenericType(typeParams),
			_ => throw new ArgumentOutOfRangeException()
		};

		return (JsonConverter)Activator.CreateInstance(converterType, Array.Empty<object>());
	}
}

internal class JsonArrayTupleConverter<T> : JsonConverter<ValueTuple<T>>
{
	public override ValueTuple<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartArray)
			throw new JsonException("Expected tuple to be encoded as an array.");

		var array = JsonElement.ParseValue(ref reader);
		var enumerator = array.EnumerateArray();

		enumerator.MoveNext();
		var value = enumerator.Current.Deserialize<T>()!;

		if (enumerator.MoveNext())
			throw new JsonException("Expected an array with 1 value but received more.");

		return new ValueTuple<T>(value);
	}

	public override void Write(Utf8JsonWriter writer, ValueTuple<T> value, JsonSerializerOptions options)
	{
		writer.WriteStartArray();
		JsonSerializer.Serialize(writer, value.Item1, typeof(T), options);
		writer.WriteEndArray();
	}
}

internal class JsonArrayTupleConverter<T1, T2> : JsonConverter<(T1, T2)>
{
	public override (T1, T2) Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartArray)
			throw new JsonException("Expected tuple to be encoded as an array.");

		var array = JsonElement.ParseValue(ref reader);
		var enumerator = array.EnumerateArray();

		return ValueReader.ReadValues2<T1, T2>(enumerator, options);
	}

	public override void Write(Utf8JsonWriter writer, (T1, T2) value, JsonSerializerOptions options)
	{
		writer.WriteStartArray();
		JsonSerializer.Serialize(writer, value.Item1, typeof(T1), options);
		JsonSerializer.Serialize(writer, value.Item2, typeof(T2), options);
		writer.WriteEndArray();
	}
}

internal class JsonArrayTupleConverter<T1, T2, T3> : JsonConverter<(T1, T2, T3)>
{
	public override (T1, T2, T3) Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartArray)
			throw new JsonException("Expected tuple to be encoded as an array.");
	
		var array = JsonElement.ParseValue(ref reader);
		var enumerator = array.EnumerateArray();

		return ValueReader.ReadValues3<T1, T2, T3>(enumerator, options);
	}

	public override void Write(Utf8JsonWriter writer, (T1, T2, T3) value, JsonSerializerOptions options)
	{
		writer.WriteStartArray();
		JsonSerializer.Serialize(writer, value.Item1, typeof(T1), options);
		JsonSerializer.Serialize(writer, value.Item2, typeof(T2), options);
		JsonSerializer.Serialize(writer, value.Item3, typeof(T3), options);
		writer.WriteEndArray();
	}
}

internal class JsonArrayTupleConverter<T1, T2, T3, T4> : JsonConverter<(T1, T2, T3, T4)>
{
	public override (T1, T2, T3, T4) Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartArray)
			throw new JsonException("Expected tuple to be encoded as an array.");

		var array = JsonElement.ParseValue(ref reader);
		var enumerator = array.EnumerateArray();

		return ValueReader.ReadValues4<T1, T2, T3, T4>(enumerator, options);
	}

	public override void Write(Utf8JsonWriter writer, (T1, T2, T3, T4) value, JsonSerializerOptions options)
	{
		writer.WriteStartArray();
		JsonSerializer.Serialize(writer, value.Item1, typeof(T1), options);
		JsonSerializer.Serialize(writer, value.Item2, typeof(T2), options);
		JsonSerializer.Serialize(writer, value.Item3, typeof(T3), options);
		JsonSerializer.Serialize(writer, value.Item4, typeof(T4), options);
		writer.WriteEndArray();
	}
}

internal class JsonArrayTupleConverter<T1, T2, T3, T4, T5> : JsonConverter<(T1, T2, T3, T4, T5)>
{
	public override (T1, T2, T3, T4, T5) Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartArray)
			throw new JsonException("Expected tuple to be encoded as an array.");

		var array = JsonElement.ParseValue(ref reader);
		var enumerator = array.EnumerateArray();

		return ValueReader.ReadValues5<T1, T2, T3, T4, T5>(enumerator, options);
	}

	public override void Write(Utf8JsonWriter writer, (T1, T2, T3, T4, T5) value, JsonSerializerOptions options)
	{
		writer.WriteStartArray();
		JsonSerializer.Serialize(writer, value.Item1, typeof(T1), options);
		JsonSerializer.Serialize(writer, value.Item2, typeof(T2), options);
		JsonSerializer.Serialize(writer, value.Item3, typeof(T3), options);
		JsonSerializer.Serialize(writer, value.Item4, typeof(T4), options);
		JsonSerializer.Serialize(writer, value.Item5, typeof(T5), options);
		writer.WriteEndArray();
	}
}

internal class JsonArrayTupleConverter<T1, T2, T3, T4, T5, T6> : JsonConverter<(T1, T2, T3, T4, T5, T6)>
{
	public override (T1, T2, T3, T4, T5, T6) Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartArray)
			throw new JsonException("Expected tuple to be encoded as an array.");

		var array = JsonElement.ParseValue(ref reader);
		var enumerator = array.EnumerateArray();

		return ValueReader.ReadValues6<T1, T2, T3, T4, T5, T6>(enumerator, options);
	}

	public override void Write(Utf8JsonWriter writer, (T1, T2, T3, T4, T5, T6) value, JsonSerializerOptions options)
	{
		writer.WriteStartArray();
		JsonSerializer.Serialize(writer, value.Item1, typeof(T1), options);
		JsonSerializer.Serialize(writer, value.Item2, typeof(T2), options);
		JsonSerializer.Serialize(writer, value.Item3, typeof(T3), options);
		JsonSerializer.Serialize(writer, value.Item4, typeof(T4), options);
		JsonSerializer.Serialize(writer, value.Item5, typeof(T5), options);
		JsonSerializer.Serialize(writer, value.Item6, typeof(T6), options);
		writer.WriteEndArray();
	}
}

internal class JsonArrayTupleConverter<T1, T2, T3, T4, T5, T6, T7> : JsonConverter<(T1, T2, T3, T4, T5, T6, T7)>
{
	public override (T1, T2, T3, T4, T5, T6, T7) Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartArray)
			throw new JsonException("Expected tuple to be encoded as an array.");

		var array = JsonElement.ParseValue(ref reader);
		var enumerator = array.EnumerateArray();

		return ValueReader.ReadValues7<T1, T2, T3, T4, T5, T6, T7>(enumerator, options);
	}

	public override void Write(Utf8JsonWriter writer, (T1, T2, T3, T4, T5, T6, T7) value, JsonSerializerOptions options)
	{
		writer.WriteStartArray();
		JsonSerializer.Serialize(writer, value.Item1, typeof(T1), options);
		JsonSerializer.Serialize(writer, value.Item2, typeof(T2), options);
		JsonSerializer.Serialize(writer, value.Item3, typeof(T3), options);
		JsonSerializer.Serialize(writer, value.Item4, typeof(T4), options);
		JsonSerializer.Serialize(writer, value.Item5, typeof(T5), options);
		JsonSerializer.Serialize(writer, value.Item6, typeof(T6), options);
		JsonSerializer.Serialize(writer, value.Item7, typeof(T7), options);
		writer.WriteEndArray();
	}
}

internal class JsonArrayTupleConverter<T1, T2, T3, T4, T5, T6, T7, TRest> : JsonConverter<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>>
	where TRest : struct
{
	public override ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartArray)
			throw new JsonException("Expected tuple to be encoded as an array.");

		var array = JsonElement.ParseValue(ref reader);
		var enumerator = array.EnumerateArray();

		return ValueReader.ReadValues8<T1, T2, T3, T4, T5, T6, T7, TRest>(enumerator, options);
	}

	public override void Write(Utf8JsonWriter writer, ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> value, JsonSerializerOptions options)
	{
		writer.WriteStartArray();
		ValueWriter.WriteValues(writer, ValueWriter.Unwrap8(value), options);
		writer.WriteEndArray();
	}
}