using System.Text.Json;
using NUnit.Framework;

using static Json.Logic.JsonLogic;

namespace Json.Logic.Tests;

public class DevTest
{
	[Test]
	public void Check()
	{
		var text = "null";

		var rule = JsonSerializer.Deserialize<Rule>(text, 
			new JsonSerializerOptions
			{
#if NET8_0_OR_GREATER
				TypeInfoResolver = Rule.JsonTypeResolver
#endif
			});
	}
}