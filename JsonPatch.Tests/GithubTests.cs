﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using Json.More;
using Json.Patch.Tests.Suite;
using Json.Pointer;
using NUnit.Framework;

namespace Json.Patch.Tests;

public class GithubTests
{
	[Test]
	public void Issue393_PatchDoesNothing()
	{
		const string mask = "*****";
		var maskJson = JsonValue.Create(mask);

		var pathsToPatch = new[] { "/first_name", "/last_name" };

		var patchOperations = pathsToPatch.Select(path => PatchOperation.Replace(JsonPointer.Parse(path), maskJson));
		var patchConfig = new JsonPatch(patchOperations);

		Console.WriteLine(JsonSerializer.Serialize(patchConfig, JsonPatchTestSuite.DefaultOptions));

		const string singleObjectJson = "{" +
										"\"_id\":\"640729d45434f90313d25c78\"," +
										"\"guid\":\"f2e2767c-03e0-4862-addc-7d46c55efb33\"," +
										"\"first_name\":\"Kathrine\"," +
										"\"last_name\":\"Pate\"" +
										"}";

		var singleObject = JsonDocument.Parse(singleObjectJson).RootElement;
		var patchedSingleObject = patchConfig.Apply(singleObject.AsNode()).Result;
		Console.WriteLine(JsonSerializer.Serialize(patchedSingleObject, JsonPatchTestSuite.DefaultOptions));

		const string arrayObjectJson = "[" +
									   "{" +
									   "\"_id\":\"640729d45434f90313d25c78\"," +
									   "\"guid\":\"f2e2767c-03e0-4862-addc-7d46c55efb33\"," +
									   "\"first_name\":\"Kathrine\"," +
									   "\"last_name\":\"Pate\"" +
									   "}," +
									   "{\"_id\":\"640729d45b5824ffcabc30a5\"," +
									   "\"guid\":\"73193eda-074b-4f31-9f09-507a008ccb75\"," +
									   "\"first_name\":\"Rivers\"," +
									   "\"last_name\":\"Smith\"" +
									   "}" +
									   "]";

		var arrayObject = JsonDocument.Parse(arrayObjectJson).RootElement;

		// Way 1: patch whole array
		var patchedArray = patchConfig.Apply(arrayObject.AsNode()).Result; // <- does nothing

		Console.WriteLine(JsonSerializer.Serialize(patchedArray, JsonPatchTestSuite.DefaultOptions));
	}

	[Test]
	public void Issue393_NodeAlreadyHasParent_2()
	{
		const string mask = "*****";
		var maskJson = JsonValue.Create(mask);

		var pathsToPatch = new[] { "/first_name", "/last_name" };

		var patchOperations = pathsToPatch.Select(path => PatchOperation.Replace(JsonPointer.Parse(path), maskJson));
		var patchConfig = new JsonPatch(patchOperations);

		const string singleObjectJson = "{" +
										"\"_id\":\"640729d45434f90313d25c78\"," +
										"\"guid\":\"f2e2767c-03e0-4862-addc-7d46c55efb33\"," +
										"\"first_name\":\"Kathrine\"," +
										"\"last_name\":\"Pate\"" +
										"}";

		var singleObject = JsonDocument.Parse(singleObjectJson).RootElement;
		var patchedSingleObject = patchConfig.Apply(singleObject.AsNode()).Result;
		Console.WriteLine(JsonSerializer.Serialize(patchedSingleObject, JsonPatchTestSuite.DefaultOptions));

		const string arrayObjectJson = "[" +
									   "{" +
									   "\"_id\":\"640729d45434f90313d25c78\"," +
									   "\"guid\":\"f2e2767c-03e0-4862-addc-7d46c55efb33\"," +
									   "\"first_name\":\"Kathrine\"," +
									   "\"last_name\":\"Pate\"" +
									   "}," +
									   "{\"_id\":\"640729d45b5824ffcabc30a5\"," +
									   "\"guid\":\"73193eda-074b-4f31-9f09-507a008ccb75\"," +
									   "\"first_name\":\"Rivers\"," +
									   "\"last_name\":\"Smith\"" +
									   "}" +
									   "]";

		var arrayObject = JsonDocument.Parse(arrayObjectJson).RootElement;

		var jsonArray = arrayObject.AsNode()!.AsArray();

		// Way 2: just patch every element
		foreach (var element in jsonArray)
		{
			var patchedNode = patchConfig.Apply(element).Result; // <-  throws an error
			Console.WriteLine(JsonSerializer.Serialize(patchedNode, JsonPatchTestSuite.DefaultOptions));
		}
	}

	[Test]
	public void Issue393_NodeAlreadyHasParent_3()
	{
		const string mask = "*****";
		var maskJson = JsonValue.Create(mask);

		var pathsToPatch = new[] { "/first_name", "/last_name" };

		var patchOperations = pathsToPatch.Select(path => PatchOperation.Replace(JsonPointer.Parse(path), maskJson));
		var patchConfig = new JsonPatch(patchOperations);

		const string singleObjectJson = "{" +
										"\"_id\":\"640729d45434f90313d25c78\"," +
										"\"guid\":\"f2e2767c-03e0-4862-addc-7d46c55efb33\"," +
										"\"first_name\":\"Kathrine\"," +
										"\"last_name\":\"Pate\"" +
										"}";

		var singleObject = JsonNode.Parse(singleObjectJson);
		var patchedSingleObject = patchConfig.Apply(singleObject).Result;
		Console.WriteLine(JsonSerializer.Serialize(patchedSingleObject, JsonPatchTestSuite.DefaultOptions));

		const string arrayObjectJson = "[" +
									   "{" +
									   "\"_id\":\"640729d45434f90313d25c78\"," +
									   "\"guid\":\"f2e2767c-03e0-4862-addc-7d46c55efb33\"," +
									   "\"first_name\":\"Kathrine\"," +
									   "\"last_name\":\"Pate\"" +
									   "}," +
									   "{\"_id\":\"640729d45b5824ffcabc30a5\"," +
									   "\"guid\":\"73193eda-074b-4f31-9f09-507a008ccb75\"," +
									   "\"first_name\":\"Rivers\"," +
									   "\"last_name\":\"Smith\"" +
									   "}" +
									   "]";

		var arrayObject = JsonNode.Parse(arrayObjectJson);

		var jsonArray = arrayObject!.AsArray();

		// Way 3: remove from initial array and then patch
		for (int currentIndex = jsonArray.Count - 1; currentIndex >= 0; currentIndex--)
		{
			var nodeToPatch = jsonArray[currentIndex];
			jsonArray.RemoveAt(currentIndex);

			var patchedNode = patchConfig.Apply(nodeToPatch).Result; // <-  throws an error
			Console.WriteLine(JsonSerializer.Serialize(patchedNode, JsonPatchTestSuite.DefaultOptions));
		}
	}

	[Test]
	public void Issue397_ReplaceShouldThrowForMissingValue()
	{
		const string mask = "*****";
		var maskJson = JsonValue.Create(mask);

		var pathsToPatch = new[] { "/first_name", "/last_name" };

		var patchOperations = pathsToPatch.Select(path => PatchOperation.Replace(JsonPointer.Parse(path), maskJson));
		var patchConfig = new JsonPatch(patchOperations);

		const string singleObjectJson = @"{
	""id"":""640729d45434f90313d25c78"",
    ""guid"":""f2e2767c-03e0-4862-addc-7d46c55efb33"",
	""city"":""Boston""
}";

		var singleObject = JsonNode.Parse(singleObjectJson);
		var result = patchConfig.Apply(singleObject);
		Console.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions(JsonPatchTestSuite.DefaultOptions) { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping }));
		Assert.IsNotNull(patchConfig.Apply(singleObject).Error);
	}

	public class Target543
	{
		public List<int> Values { get; set; } = null!;
	}

	[Test]
	public void Issue543_CreatePatchToAddItem()
	{
		var targetObj = new Target543 { Values = new List<int> { 1, 2, 3, 4 } };
		var target = JsonSerializer.SerializeToNode(targetObj, JsonPatchTestSuite.DefaultOptions);

		var jsonPointer = JsonPointer.Create<Target543>(x => x.Values.Last());
		var jsonPatch = new JsonPatch(PatchOperation.Add(jsonPointer, (JsonNode)42));

		var expected = new JsonObject
		{
			["Values"] = new JsonArray { 1, 2, 3, 4, 42 }
		};

		var patchResult = jsonPatch.Apply(target);

		Assert.IsTrue(expected.IsEquivalentTo(patchResult.Result));
	}
}