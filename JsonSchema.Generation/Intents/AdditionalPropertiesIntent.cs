﻿using System.Collections.Generic;

namespace Json.Schema.Generation.Intents;

/// <summary>
/// Provides intent to create an `additionalProperties` keyword.
/// </summary>
public class AdditionalPropertiesIntent : ISchemaKeywordIntent, IContextContainer
{
	/// <summary>
	/// The context that represents the inner requirements.
	/// </summary>
	public SchemaGenerationContextBase Context { get; private set; }

	/// <summary>
	/// Creates a new <see cref="AdditionalPropertiesIntent"/> instance.
	/// </summary>
	/// <param name="context">The context.</param>
	public AdditionalPropertiesIntent(SchemaGenerationContextBase context)
	{
		Context = context;
	}

	/// <summary>
	/// Replaces one context with another.
	/// </summary>
	/// <param name="hashCode">The hashcode of the context to replace.</param>
	/// <param name="newContext">The new context.</param>
	public void Replace(int hashCode, SchemaGenerationContextBase newContext)
	{
		if (Context.Hash == hashCode)
			Context = newContext;
	}

	/// <summary>
	/// Applies the keyword to the <see cref="JsonSchemaBuilder"/>.
	/// </summary>
	/// <param name="builder">The builder.</param>
	public void Apply(JsonSchemaBuilder builder)
	{
		builder.AdditionalProperties(Context.Apply());
	}
}