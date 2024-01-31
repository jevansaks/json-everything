﻿using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Json.More;

namespace Json.Schema.OpenApi;

/// <summary>
/// Declares the vocabularies of the supported drafts.
/// </summary>
public static class Vocabularies
{
	/// <summary>
	/// The data vocabulary ID.
	/// </summary>
	public const string OpenApiId = "https://spec.openapis.org/oas/3.1/vocab/base";

	/// <summary>
	/// The data vocabulary.
	/// </summary>
	public static readonly Vocabulary OpenApi = new(OpenApiId,
		typeof(ExampleKeyword),
		typeof(DiscriminatorKeyword),
		typeof(ExternalDocsKeyword),
		typeof(XmlKeyword)
	);

	/// <summary>
	/// Registers the all components required to use the data vocabulary.
	/// </summary>
	public static void Register(VocabularyRegistry? vocabRegistry = null, SchemaRegistry? schemaRegistry = null)
	{
		vocabRegistry ??= VocabularyRegistry.Global;
		schemaRegistry ??= SchemaRegistry.Global;

		vocabRegistry.Register(OpenApi);
		SchemaKeywordRegistry.Register<ExampleKeyword>(OpenApiSerializerContextBase.Default);
		SchemaKeywordRegistry.RegisterNullValue(new ExampleKeyword(null));
		SchemaKeywordRegistry.Register<DiscriminatorKeyword>(OpenApiSerializerContextBase.Default);
		SchemaKeywordRegistry.Register<ExternalDocsKeyword>(OpenApiSerializerContextBase.Default);
		SchemaKeywordRegistry.Register<XmlKeyword>(OpenApiSerializerContextBase.Default);
		schemaRegistry.Register(MetaSchemas.OpenApiMeta);
	}
}

[JsonSerializable(typeof(ExampleKeyword))]
[JsonSerializable(typeof(DiscriminatorKeyword))]
[JsonSerializable(typeof(DiscriminatorKeywordJsonConverter.Model), TypeInfoPropertyName = "DiscriminatorModel")]
[JsonSerializable(typeof(ExternalDocsKeyword))]
[JsonSerializable(typeof(ExternalDocsKeywordJsonConverter.Model), TypeInfoPropertyName = "ExternalDocsModel")]
[JsonSerializable(typeof(XmlKeyword))]
[JsonSerializable(typeof(XmlKeywordJsonConverter.Model), TypeInfoPropertyName = "XmlModel")]
[JsonSerializable(typeof(JsonNode))]
[JsonSerializable(typeof(IReadOnlyDictionary<string, string>))]
internal partial class OpenApiSerializerContextBase : JsonSerializerContext;

internal class OpenApiSerializerContext : OpenApiSerializerContextBase
{
	new public static OpenApiSerializerContextBase Default => ContextManager.Default;

	public static TypeResolverOptionsManager<OpenApiSerializerContextBase> ContextManager = new(
		(JsonSerializerOptions options) => new OpenApiSerializerContextBase(options),
		() => [JsonSchema.TypeInfoResolver]
		);
}