#nullable enable
using System;
using AVS.CoreLib.REST.Projections;
using AVS.CoreLib.REST.Responses;

namespace AVS.CoreLib.REST.Extensions;

public static class RestResponseProjectionExtensions
{
    public static Response<T> ToResponse<T>(this RestResponse restResponse)
    {
        var isSuccess = restResponse.IsSuccessful();
        var response = Response.Create<T>(restResponse.Source, content: restResponse.Content, restResponse.Error, restResponse.Request);

        if (!isSuccess)
            return response;

        response.Data = restResponse.Deserialize<T>();
        return response;
    }
    
    public static Response<T> ToResponse<T>(this RestResponse restResponse, T? data)
    {
        var isSuccess = restResponse.IsSuccessful();
        var response = Response.Create<T>(restResponse.Source, content: restResponse.Content, restResponse.Error, restResponse.Request);

        if (!isSuccess)
            return response;

        response.Data = data;
        return response;
    }
    
    /// <summary>
    /// Creates <see cref="Response{T}"/>
    /// when response is successful content (json) will be deserialized by means of <see cref="Proj{T}"/> and one of map functions.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="restResponse">rest response</param>
    /// <param name="map">gives control to the caller's code over projection (Map / MapArray / MapDictionary)</param>
    /// <returns><see cref="Response{T}"/></returns>
    public static Response<T> ToResponse<T>(this RestResponse restResponse, Func<Proj<T>, T?> map)
    {
        var isSuccess = restResponse.IsSuccessful();
        var response = Response.Create<T>(restResponse.Source, content: restResponse.Content, restResponse.Error, restResponse.Request);

        if (!isSuccess)
            return response;

        var json = restResponse.Content;

        var proj = new Proj<T>(json);

        response.Data = map(proj);
        return response;
    }

    /// <summary>
    /// Creates <see cref="Response{T}"/>
    /// when response is successful content (json) will be deserialized by means of <see cref="Proj{T}"/> and one of map functions.
    /// </summary>
    /// <typeparam name="T">T data</typeparam>
    /// <typeparam name="TType">Concrete type for deserialization</typeparam>
    /// <param name="restResponse">rest response</param>
    /// <param name="proxy">proxy or builder to produce T data</param>
    /// <param name="map">gives control to the caller's code over projection (Map / MapArray / MapDictionary)</param>
    /// <returns><see cref="Response{T}"/></returns>
    public static Response<T> ToResponse<T, TType>(this RestResponse restResponse, IProxy<TType, T> proxy, Func<ProxyProj<T>, T?> map)
    {
        var isSuccess = restResponse.IsSuccessful();
        var response = Response.Create<T>(restResponse.Source, content: restResponse.Content, restResponse.Error, restResponse.Request);

        if (!isSuccess)
            return response;

        var proj = new ProxyProj<T>(restResponse.Content, proxy);
        response.Data = map(proj);
        return response;
    }

    /// <summary>
    /// Creates <see cref="Response{T}"/>
    /// when response is successful content (json) will be deserialized by means of <see cref="Proj{T}"/> and one of MapDictionary function.
    /// </summary>
    /// <typeparam name="T">T data</typeparam>
    /// <typeparam name="TType">Concrete type for deserialization</typeparam>
    /// <param name="restResponse">rest response</param>
    /// <param name="proxy">proxy or builder to produce T data</param>
    /// <param name="map">gives control to the caller's code over projection (Map / MapArray / MapDictionary)</param>
    /// <returns><see cref="Response{T}"/></returns>
    public static Response<T> ToResponse<T, TType>(this RestResponse restResponse, IKeyedCollectionProxy<T, TType> proxy, Func<KeyedProxyProj<T>, T?> map)
    {
        var isSuccess = restResponse.IsSuccessful();
        var response = Response.Create<T>(restResponse.Source, content: restResponse.Content, restResponse.Error, restResponse.Request);

        if (!isSuccess)
            return response;

        var proj = new KeyedProxyProj<T>(restResponse.Content, proxy);
        response.Data = map(proj);
        return response;
    }
}

/*
    /// <summary>
    /// Creates <see cref="Response{T}"/>
    /// when response is successful content (json) will be deserialized by means of <see cref="Proj{T}"/> and Map function.
    /// </summary>
    public static Response<T> ToResponse<T>(this RestResponse restResponse) where T : new()
    {
        var isSuccess = restResponse.IsSuccessful();
        var response = Response.Create<T>(restResponse.Source, content: restResponse.Content, restResponse.Error, restResponse.Request);

        if (!isSuccess)
            return response;

        var json = restResponse.Content;

        var proj = new Proj<T>(json);
        response.Data = proj.Map();
        return response;
    }

    /// <summary>
    /// Creates <see cref="Response{T}"/>
    /// when response is successful content (json) will be deserialized by means of <see cref="Proj{T}"/> and Map function.
    /// </summary>
    /// <typeparam name="T">abstraction / interface</typeparam>
    /// <typeparam name="TType">concrete type for deserialization</typeparam>
    public static Response<T> ToResponse<T, TType>(this RestResponse restResponse) where TType : class, T, new()
    {
        var isSuccess = restResponse.IsSuccessful();
        var response = Response.Create<T>(restResponse.Source, content: restResponse.Content, restResponse.Error, restResponse.Request);

        if (!isSuccess)
            return response;

        var json = restResponse.Content;

        var proj = new Proj<T>(json);
        response.Data = proj.Map<TType>();
        return response;
    }*/