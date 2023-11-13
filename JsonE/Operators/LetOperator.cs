﻿using System.Text.Json.Nodes;

namespace Json.JsonE.Operators;

internal class LetOperator : IOperator
{
	public const string Name = "$let";
	
	public void Validate(JsonNode? template)
	{
		var obj = template!.AsObject();

		var parameter = obj[Name];
		if (parameter.IsTemplateOr<JsonObject>()) return;

		obj.VerifyNoUndefinedProperties(Name, "in");

		if (obj.Count != 2)
			throw new TemplateException($"{Name} requires `in` property");
	}

	public JsonNode? Evaluate(JsonNode? template, EvaluationContext context)
	{
		var obj = template!.AsObject();
		var value = obj[Name];

		var additionalContext = JsonE.Evaluate(value, context);
		additionalContext.ValidateAsContext(Name);

		context.Push(additionalContext);
		var result = JsonE.Evaluate(obj["in"], context);
		context.Pop();

		return result;
	}
}