using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Json.More;

/// <summary>
/// Manages a <see cref="JsonSerializer"/> object that incorporates the type resolvers
/// found in a <see cref="JsonSerializerContext"/>.
/// </summary>
public class TypeResolverOptionsManager<T>
{
	private readonly JsonSerializerOptions _baseOptions;
	private JsonSerializerOptions? _serializerOptions;
	private Func<IEnumerable<IJsonTypeInfoResolver>> _getTypeInfoResolvers;
	private readonly object _serializerOptionsLock = new();

	private T? _default;
	private Func<JsonSerializerOptions, T> _creator;

	/// <summary>
	/// Returns a <typeparamref name="T"/> combined with other TypeInfoResolvers.
	/// </summary>
	public T Default
	{
		get
		{
			lock (_serializerOptionsLock)
			{
				_default ??= _creator(SerializerOptions);
				return _default!;
			}
		}
	}

	/// <summary>
	/// Rebuilds the Context with a new set of resolvers.
	/// </summary>
	/// <remarks>
	/// This should be built with the same set of resolvers that the options manager was
	/// created with, except for the base resolver.
	/// </remarks>
	public void NotifyTypeInfoResolverChanged()
	{
		lock (_serializerOptionsLock)
		{
			_serializerOptions = null;
			_default = default(T);
		}

		TypeInfoResolverUpdated?.Invoke(this, EventArgs.Empty);
	}

	/// <summary>
	/// Raised when the type info resolver is updated.  (See <see cref="NotifyTypeInfoResolverChanged"/>)
	/// </summary>
	public event EventHandler? TypeInfoResolverUpdated;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="contextCreator"></param>
	/// <param name="getResolvers"></param>
	/// <param name="baseOptions"></param>
	public TypeResolverOptionsManager(Func<JsonSerializerOptions, T> contextCreator, Func<IEnumerable<IJsonTypeInfoResolver>> getResolvers, JsonSerializerOptions? baseOptions = null)
	{
		_creator = contextCreator;
		_baseOptions = baseOptions ?? new JsonSerializerOptions();
		_getTypeInfoResolvers = getResolvers;
	}

	/// <summary>
	/// Gets the serializer options.
	/// </summary>
	private JsonSerializerOptions SerializerOptions
	{
		get
		{
			lock (_serializerOptionsLock)
			{
				if (_serializerOptions == null)
				{
					_serializerOptions = new JsonSerializerOptions(_baseOptions);
					foreach (var resolver in _getTypeInfoResolvers())
					{
						_serializerOptions.TypeInfoResolverChain.Add(resolver);
					}
				}

				return _serializerOptions!;
			}
		}
	}
}


public class TypeResolverOptionsManager
{
	private readonly JsonSerializerOptions _baseOptions;
	private JsonSerializerOptions? _serializerOptions;
	private readonly IJsonTypeInfoResolver _baseResolver;
	private IJsonTypeInfoResolver _typeInfoResolver;
	private readonly object _serializerOptionsLock = new();

	/// <summary>
	/// Gets the serializer options.
	/// </summary>
	public JsonSerializerOptions SerializerOptions
	{
		get
		{
			lock (_serializerOptionsLock)
			{
				_serializerOptions ??= new JsonSerializerOptions(_baseOptions)
				{
					TypeInfoResolver = _typeInfoResolver
				};

				return _serializerOptions!;
			}
		}
	}

	/// <summary>
	/// Gets the type info resolver for the associated context.
	/// </summary>
	public IJsonTypeInfoResolver TypeInfoResolver => _typeInfoResolver;

	/// <summary>
	/// Raised when the type info resolver is updated.  (See <see cref="RebuildTypeResolver"/>)
	/// </summary>
	public event EventHandler? TypeInfoResolverUpdated;

	/// <summary>
	/// Creates a new instance of the <see cref="TypeResolverOptionsManager"/> class.
	/// </summary>
	/// <param name="baseResolver">
	/// The base resolver.  This will generally be the `Default` property on your
	/// <see cref="JsonSerializerContext"/>.
	/// </param>
	/// <param name="resolvers">Any additional resolvers to be included.</param>
	public TypeResolverOptionsManager(IJsonTypeInfoResolver baseResolver, params IJsonTypeInfoResolver[] resolvers)
	{
		_baseOptions = new JsonSerializerOptions();
		_baseResolver = baseResolver;
		_typeInfoResolver = JsonTypeInfoResolver.Combine([baseResolver, .. resolvers]);
	}

	/// <summary>
	/// Creates a new instance of the <see cref="TypeResolverOptionsManager"/> class
	/// that includes a custom options object to use as a base.
	/// </summary>
	/// <param name="baseOptions">The base options.</param>
	/// <param name="baseResolver">
	/// The base resolver.  This will generally be the `Default` property on your
	/// <see cref="JsonSerializerContext"/>.
	/// </param>
	/// <param name="resolvers">Any additional resolvers to be included.</param>
	public TypeResolverOptionsManager(JsonSerializerOptions baseOptions, IJsonTypeInfoResolver baseResolver, params IJsonTypeInfoResolver[] resolvers)
	{
		_baseOptions = baseOptions;
		_baseResolver = baseResolver;
		_typeInfoResolver = JsonTypeInfoResolver.Combine([baseResolver, .. resolvers]);
	}

	/// <summary>
	/// Rebuilds the type resolver with a new set of resolvers.
	/// </summary>
	/// <param name="resolvers">The resolvers to incorporate</param>
	/// <remarks>
	/// This should be built with the same set of resolvers that the options manager was
	/// created with, except for the base resolver.
	/// </remarks>
	public void RebuildTypeResolver(params IJsonTypeInfoResolver[] resolvers)
	{
		lock (_serializerOptionsLock)
		{
			_typeInfoResolver = JsonTypeInfoResolver.Combine([_baseResolver, .. resolvers]);
			_serializerOptions = null;
		}

		TypeInfoResolverUpdated?.Invoke(this, EventArgs.Empty);
	}
}