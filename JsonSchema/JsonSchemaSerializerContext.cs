﻿using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Json.More;
using Json.Pointer;

namespace Json.Schema;

[JsonSerializable(typeof(JsonSchema))]
[JsonSerializable(typeof(AdditionalItemsKeyword))]
[JsonSerializable(typeof(AdditionalPropertiesKeyword))]
[JsonSerializable(typeof(AllOfKeyword))]
[JsonSerializable(typeof(AnchorKeyword))]
[JsonSerializable(typeof(AnyOfKeyword))]
[JsonSerializable(typeof(CommentKeyword))]
[JsonSerializable(typeof(ConstKeyword))]
[JsonSerializable(typeof(ContainsKeyword))]
[JsonSerializable(typeof(ContentEncodingKeyword))]
[JsonSerializable(typeof(ContentMediaTypeKeyword))]
[JsonSerializable(typeof(ContentSchemaKeyword))]
[JsonSerializable(typeof(DefaultKeyword))]
[JsonSerializable(typeof(DefinitionsKeyword))]
[JsonSerializable(typeof(DefsKeyword))]
[JsonSerializable(typeof(DependenciesKeyword))]
[JsonSerializable(typeof(DependentRequiredKeyword))]
[JsonSerializable(typeof(DependentSchemasKeyword))]
[JsonSerializable(typeof(DeprecatedKeyword))]
[JsonSerializable(typeof(DescriptionKeyword))]
[JsonSerializable(typeof(DynamicAnchorKeyword))]
[JsonSerializable(typeof(DynamicRefKeyword))]
[JsonSerializable(typeof(ElseKeyword))]
[JsonSerializable(typeof(EnumKeyword))]
[JsonSerializable(typeof(ExamplesKeyword))]
[JsonSerializable(typeof(ExclusiveMaximumKeyword))]
[JsonSerializable(typeof(ExclusiveMinimumKeyword))]
[JsonSerializable(typeof(FormatKeyword))]
[JsonSerializable(typeof(IdKeyword))]
[JsonSerializable(typeof(IfKeyword))]
[JsonSerializable(typeof(ItemsKeyword))]
[JsonSerializable(typeof(MaxContainsKeyword))]
[JsonSerializable(typeof(MaximumKeyword))]
[JsonSerializable(typeof(MaxItemsKeyword))]
[JsonSerializable(typeof(MaxLengthKeyword))]
[JsonSerializable(typeof(MaxPropertiesKeyword))]
[JsonSerializable(typeof(MinContainsKeyword))]
[JsonSerializable(typeof(MinimumKeyword))]
[JsonSerializable(typeof(MinItemsKeyword))]
[JsonSerializable(typeof(MinLengthKeyword))]
[JsonSerializable(typeof(MinPropertiesKeyword))]
[JsonSerializable(typeof(MultipleOfKeyword))]
[JsonSerializable(typeof(NotKeyword))]
[JsonSerializable(typeof(OneOfKeyword))]
[JsonSerializable(typeof(PatternKeyword))]
[JsonSerializable(typeof(PatternPropertiesKeyword))]
[JsonSerializable(typeof(PrefixItemsKeyword))]
[JsonSerializable(typeof(PropertiesKeyword))]
[JsonSerializable(typeof(PropertyDependenciesKeyword))]
[JsonSerializable(typeof(PropertyNamesKeyword))]
[JsonSerializable(typeof(ReadOnlyKeyword))]
[JsonSerializable(typeof(RecursiveAnchorKeyword))]
[JsonSerializable(typeof(RecursiveRefKeyword))]
[JsonSerializable(typeof(RefKeyword))]
[JsonSerializable(typeof(RequiredKeyword))]
[JsonSerializable(typeof(SchemaKeyword))]
[JsonSerializable(typeof(ThenKeyword))]
[JsonSerializable(typeof(TitleKeyword))]
[JsonSerializable(typeof(TypeKeyword))]
[JsonSerializable(typeof(UnevaluatedItemsKeyword))]
[JsonSerializable(typeof(UnevaluatedPropertiesKeyword))]
[JsonSerializable(typeof(UniqueItemsKeyword))]
[JsonSerializable(typeof(UnrecognizedKeyword))]
[JsonSerializable(typeof(VocabularyKeyword))]
[JsonSerializable(typeof(WriteOnlyKeyword))]
[JsonSerializable(typeof(decimal))]
[JsonSerializable(typeof(Dictionary<string, bool>))]
[JsonSerializable(typeof(Dictionary<string, JsonSchema>))]
[JsonSerializable(typeof(Dictionary<string, List<string>>))]
[JsonSerializable(typeof(Dictionary<string, PropertyDependency>))]
[JsonSerializable(typeof(Dictionary<string, SchemaOrPropertyList>))]
[JsonSerializable(typeof(Dictionary<string, string[]>))]
[JsonSerializable(typeof(IReadOnlyCollection<JsonNode?>))]
[JsonSerializable(typeof(IReadOnlyDictionary<string, JsonNode?>))]
[JsonSerializable(typeof(IReadOnlyDictionary<string, JsonSchema>))]
[JsonSerializable(typeof(IReadOnlyDictionary<string, string>))]
[JsonSerializable(typeof(IReadOnlyDictionary<string, PropertyDependency>))]
[JsonSerializable(typeof(IReadOnlyList<EvaluationResults>))]
[JsonSerializable(typeof(IReadOnlyList<string>))]
[JsonSerializable(typeof(int[]))]
[JsonSerializable(typeof(JsonArray))]
[JsonSerializable(typeof(JsonNode))]
[JsonSerializable(typeof(JsonNode[]))]
[JsonSerializable(typeof(JsonPointer))]
[JsonSerializable(typeof(List<JsonSchema>))]
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(SchemaValueType))]
[JsonSerializable(typeof(string[]))]
[JsonSerializable(typeof(uint))]
[JsonSerializable(typeof(Uri))]
internal partial class JsonSchemaSerializerContextBase : JsonSerializerContext;

internal class JsonSchemaSerializerContext(JsonSerializerOptions options) : JsonSchemaSerializerContextBase(options), IJsonTypeInfoResolver
{
	new public static JsonSchemaSerializerContext Default => ContextManager.Default;

	JsonTypeInfo? IJsonTypeInfoResolver.GetTypeInfo(Type type, JsonSerializerOptions options) => ContextManager.GetTypeInfoImpl(this, type, options);

	public static TypeResolverOptionsManager<JsonSchemaSerializerContext> ContextManager = new(
		(JsonSerializerOptions options) => new JsonSchemaSerializerContext(options),
		() => SchemaKeywordRegistry.ExternalTypeInfoResolvers
		);
}
