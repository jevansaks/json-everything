using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Json.More;
using NUnit.Framework;

namespace Json.Logic.Tests.Suite;

[JsonSerializable(typeof(TestSuite))]
[JsonSerializable(typeof(Test))]
[JsonSerializable(typeof(List<Test>))]
[JsonSerializable(typeof(JsonNode))]
internal partial class LogicTestContext : JsonSerializerContext;

public class MoreTests
{
	public static IEnumerable<TestCaseData> Suite()
	{
		return Task.Run(async () =>
		{
			var testsPath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "Files\\more-tests.json").AdjustForPlatform();

			var content = await File.ReadAllTextAsync(testsPath);

			var testSuite = JsonSerializer.Deserialize<TestSuite>(content, LogicTestContext.Default.Options);

			return testSuite!.Tests.Select(t => new TestCaseData(t) { TestName = $"{t.Logic}  |  {t.Data.AsJsonString()}  |  {t.Expected.AsJsonString()}" });
		}).Result;
	}

	[TestCaseSource(nameof(Suite))]
	public void Run(Test test)
	{
		var rule = JsonSerializer.Deserialize<Rule>(test.Logic, new JsonSerializerOptions
		{
#if NET8_0_OR_GREATER
			TypeInfoResolverChain = { LogicTestContext.Default, Rule.JsonTypeResolver }
#endif
		});

		if (rule == null)
		{
			Assert.IsNull(test.Expected);
			return;
		}

		JsonAssert.AreEquivalent(test.Expected ?? JsonNull.SignalNode, rule.Apply(test.Data));
	}
}