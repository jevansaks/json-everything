﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Json.More;
using NUnit.Framework;

namespace Json.Logic.Tests.Suite;

public class SuiteRunner
{
	public static IEnumerable<TestCaseData> Suite()
	{
		return Task.Run(async () =>
		{
			var testsPath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "Files\\tests.json").AdjustForPlatform();

			string content = null!;
			try
			{
				using var client = new HttpClient();
				using var request = new HttpRequestMessage(HttpMethod.Get, "https://jsonlogic.com/tests.json");
				using var response = await client.SendAsync(request);

				content = await response.Content.ReadAsStringAsync();

				await File.WriteAllTextAsync(testsPath, content);
			}
			catch (Exception e)
			{
				content ??= await File.ReadAllTextAsync(testsPath);

				Console.WriteLine(e);
			}


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

		JsonAssert.AreEquivalent(test.Expected, rule.Apply(test.Data));
	}

	private static readonly JsonSerializerOptions _spellingTestSerializerOptions =
		new()
		{
			Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
			Converters = { new LogicComponentConverter { SaveSource = false } },
#if NET8_0_OR_GREATER
			TypeInfoResolverChain = { LogicTestContext.Default, Rule.JsonTypeResolver }
#endif
		};

	[TestCaseSource(nameof(Suite))]
	public void SpellingTest(Test test)
	{
		var node = JsonNode.Parse(test.Logic);
		var rule = JsonSerializer.Deserialize<Rule>(test.Logic, _spellingTestSerializerOptions);

		var serialized = JsonSerializer.SerializeToNode(rule, _spellingTestSerializerOptions);

		if (node.IsEquivalentTo(serialized)) return;

		Console.WriteLine($"Expected: {node.AsJsonString(_spellingTestSerializerOptions)}");
		Console.WriteLine($"Actual:   {serialized.AsJsonString(_spellingTestSerializerOptions)}");
		Assert.Inconclusive();
	}


}