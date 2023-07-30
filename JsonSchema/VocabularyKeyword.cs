﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Json.Pointer;

namespace Json.Schema;

/// <summary>
/// Handles `$vocabulary`.
/// </summary>
[SchemaKeyword(Name)]
[SchemaPriority(long.MinValue)]
[SchemaSpecVersion(SpecVersion.Draft201909)]
[SchemaSpecVersion(SpecVersion.Draft202012)]
[SchemaSpecVersion(SpecVersion.DraftNext)]
[Vocabulary(Vocabularies.Core201909Id)]
[Vocabulary(Vocabularies.Core202012Id)]
[Vocabulary(Vocabularies.CoreNextId)]
[JsonConverter(typeof(VocabularyKeywordJsonConverter))]
public class VocabularyKeyword : IJsonSchemaKeyword
{
	/// <summary>
	/// The JSON name of the keyword.
	/// </summary>
	public const string Name = "$vocabulary";

	private Dictionary<Uri, bool> _allVocabularies;

	/// <summary>
	/// The collection of vocabulary requirements.
	/// </summary>
	public IReadOnlyDictionary<Uri, bool> Vocabulary { get; }

	/// <summary>
	/// Creates a new <see cref="VocabularyKeyword"/>.
	/// </summary>
	/// <param name="values">The collection of vocabulary requirements.</param>
	public VocabularyKeyword(IReadOnlyDictionary<Uri, bool> values)
	{
		Vocabulary = values ?? throw new ArgumentNullException(nameof(values));
	}

	public KeywordConstraint GetConstraint(SchemaConstraint schemaConstraint,
		IReadOnlyList<KeywordConstraint> localConstraints,
		ConstraintBuilderContext context)
	{
		_allVocabularies = Vocabulary.ToDictionary(x => x.Key, x => x.Value);
		switch (context.EvaluatingAs)
		{
			case SpecVersion.Unspecified:
			case SpecVersion.Draft201909:
				_allVocabularies[new Uri(Vocabularies.Core201909Id)] = true;
				break;
			case SpecVersion.Draft202012:
				_allVocabularies[new Uri(Vocabularies.Core202012Id)] = true;
				break;
			case SpecVersion.DraftNext:
				_allVocabularies[new Uri(Vocabularies.CoreNextId)] = true;
				break;
		}

		return new KeywordConstraint(Name, Evaluator);
	}

	private void Evaluator(KeywordEvaluation evaluation, ConstraintBuilderContext context)
	{
		var violations = new List<Uri>();
		var overallResult = true;
		foreach (var kvp in _allVocabularies)
		{
			var isKnown = context.Options.VocabularyRegistry.IsKnown(kvp.Key);
			var isValid = !kvp.Value || isKnown;
			if (!isValid)
				violations.Add(kvp.Key);
			overallResult &= isValid;
		}

		if (!overallResult)
			evaluation.Results.Fail(Name, ErrorMessages.UnknownVocabularies, ("vocabs", $"[{string.Join(", ", violations)}]"));
	}
}

internal class VocabularyKeywordJsonConverter : JsonConverter<VocabularyKeyword>
{
	public override VocabularyKeyword Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartObject)
			throw new JsonException("Expected object");

		var schema = JsonSerializer.Deserialize<Dictionary<string, bool>>(ref reader, options);
		var withUris = schema!.ToDictionary(kvp => new Uri(kvp.Key), kvp => kvp.Value);
		return new VocabularyKeyword(withUris);
	}
	public override void Write(Utf8JsonWriter writer, VocabularyKeyword value, JsonSerializerOptions options)
	{
		writer.WritePropertyName(VocabularyKeyword.Name);
		writer.WriteStartObject();
		foreach (var kvp in value.Vocabulary)
		{
			writer.WriteBoolean(kvp.Key.OriginalString, kvp.Value);
		}
		writer.WriteEndObject();
	}
}

public static partial class ErrorMessages
{
	private static string? _unknownVocabularies;

	/// <summary>
	/// Gets or sets the error message for when a vocabulary is unknown but required.
	/// </summary>
	/// <remarks>
	///	Available tokens are:
	///   - [[vocabs]] - the URI IDs of the missing vocabularies as a comma-delimited list
	/// </remarks>
	public static string UnknownVocabularies
	{
		get => _unknownVocabularies ?? Get();
		set => _unknownVocabularies = value;
	}
}