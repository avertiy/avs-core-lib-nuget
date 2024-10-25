#nullable enable
using System;
using AVS.CoreLib.REST.Projections;

namespace AVS.CoreLib.REST.Extensions
{
    public static class ProjectionExtensions
    {
        #region Projection<T> and Projection<T,TType>
        /// <summary>
        /// Creates <see cref="Projections.Projection{T}"/>
        /// <code>
        ///   // 1. T is a concrete type (direct projection)
        ///   var projection = restResponse.Projection{Order}();
        ///   Response{Order} response = projection.Map();
        ///   
        ///   // 2. T is an abstraction (direct projection)
        ///   var projection = restResponse.Projection{IOrder}();
        ///   Response{Order} response = projection.Map{BinanceOrder}();
        ///   
        ///   // 3. T is an abstraction/interface (projection via proxy)
        ///   var projection = restResponse.Projection{IOrderBook}();
        ///   Response{IOrderBook} response = projection.MapWith{OrderBookBuilder}();
        /// </code>
        /// </summary>
        public static Projection<T> ToProjection<T>(this RestResponse response)
        {
            return new Projection<T>(response);
        }

        /// <summary>
        /// Create <see cref="Projections.Projection{T,TType}"/> projection
        /// where T is an abstraction of TType i.e. TType:T
        /// basically it 
        /// <code>
        ///   // simple direct projection
        ///   var projection = restResponse.Projection{IOrder, BinanceOrder}()
        ///     .PostProcess(order=> order.Symbol = ...);
        ///   Response{IOrder} response = projection.Map();
        /// </code>
        /// </summary>
        public static Projection<T, TType> ToProjection<T, TType>(this RestResponse response) where TType : T
        {
            return new Projection<T, TType>(response);
        }


        #endregion

        /// <summary>
        /// Creates <see cref="Projections.IndirectProjection{T,TType}"/> projection.
        /// It suits well when T-container and T-map are decoupled i.e there is no inheritance between them
        /// <code>
        ///   //indirect projection through proxy
        ///   var projection = restResponse.IndirectProjection{IOrderResult, BinanceOrder}();
        ///   Response{IOrder} response = projection.MapWith{OrderBuilder}();
        /// </code>
        /// </summary>
        public static IndirectProjection<T, TType> ToIndirectProjection<T, TType>(this RestResponse response)
        {
            return new IndirectProjection<T, TType>(response);
        }

        #region ListProjection<TItem>

        /// <summary>
        /// Creates <see cref="ListProjection{T}"/> to map json of array type structure into <see cref="List{T}"/>
        /// it is a strait mapping approach
        /// <code>
        ///    // 1. T is a concrete type
        ///    var projection = response.ListProjection{Order}();
        ///    Response{List{Order}} list = projection.Map();
        /// 
        ///    // 2. T is an abstraction/interface
        ///    var projection = response.ListProjection{IOrder}();
        ///    Response{List{IOrder}} list = projection.Map{Order}();
        ///     
        ///    // 3. indirect projection via proxy (concreate types)
        ///    var projection = response.ListProjection{Ticker}();
        ///    Response{List{Ticker}}  = projection.MapWith{TickerListBuilder}();    
        ///    
        ///    // 4. indirect projection via proxy (abstraction)
        ///    var projection = response.ListProjection{ITicker}();
        ///    Response{List{Ticker}} volume = projection.MapWith{TickerListBuilder, BinanceTicker}();  
        /// </code>
        /// </summary>
        public static ListProjection<T> ToList<T>(this RestResponse response) where T : class
        {
            return new ListProjection<T>(response);
        }

        /// <summary>
        /// Creates <see cref="Projections.EnumerableProjection{T}"/> to map json of array type structure and pick FirstOrDefault for example
        /// it is a strait mapping approach
        /// <code>
        ///    // 1. FirstOrDefault concrete type
        ///    var projection = response.Enumerable{Order}();
        ///    Response{Order} list = projection.FirstOrDefault();
        /// 
        ///    // 2. FirstOrDefault abstraction
        ///    var projection = response.Enumerable{IOrder}();
        ///    Response{IOrder} list = projection.FirstOrDefault{BinanceOrder}();
        /// </code>
        /// </summary>
        public static EnumerableProjection<T> ToEnumerableProjection<T>(this RestResponse response) where T : class
        {
            return new EnumerableProjection<T>(response);
        }

        #endregion

        #region ContainerProjection<TItem>

        /// <summary>
        /// Creates <see cref="ContainerProjection{TContainer, TItem}"/> to map json array type structure [...]
        /// <code>
        ///     // 1. when container implements ICollection{TItem}
        ///     var projection = response.ToContainerProjection{IOpenOrders, BinanceOrder}()
        ///     Response{IOpenOrders} result = projection.Map{OpenOrdersCollection}();
        ///     
        ///     // 2. when container does not implement ICollection{TItem}
        ///     var projection = response.ToContainerProjection{IOpenOrders, BinanceOrder}()
        ///     Response{IOpenOrders} result = projection.Map{OpenOrdersCollection}((x,item) => x.Add(item));
        ///     
        ///     // 3. when we use proxy to produce container 
        ///     var projection = response.ToContainerProjection{IOpenOrders, BinanceOrder}()
        ///     // OpenOrdersBuilder: IListProxy{IOpenOrders, BinanceOrder} see IListProxy{out T, in TItem}
        ///     Response{IOpenOrders} result = projection.MapWith{OpenOrdersBuilder}();
        /// </code>
        /// </summary>
        public static ContainerProjection<TContainer, TItem> ToContainerProjection<TContainer, TItem>(this RestResponse response) where TContainer : class
        {
            return new ContainerProjection<TContainer, TItem>(response);
        }

        /// <summary>
        /// Create <see cref="ContainerProjection{T,TItem}"/> to map json array type structure [...] into T items container
        /// <code>
        ///     // 1. when container implements ICollection{TItem}
        ///     var projection = response.ToContainerProjection{IOpenOrders, BinanceOrder}()
        ///     Response{IOpenOrders} result = projection.Map{OpenOrdersCollection}();
        ///     
        ///     // 2. when container does not implement ICollection{TItem}
        ///     var projection = response.ToContainerProjection{IOpenOrders, BinanceOrder}()
        ///     Response{IOpenOrders} result = projection.Map{OpenOrdersCollection}((x,item) => x.Add(item));
        ///     
        ///     // 3. when we use proxy to produce container 
        ///     var projection = response.ToContainerProjection{IOpenOrders, BinanceOrder}()
        ///     // OpenOrdersBuilder: IListProxy{IOpenOrders, BinanceOrder} see IListProxy{out T, in TItem}
        ///     Response{IOpenOrders} result = projection.MapWith{OpenOrdersBuilder}();
        /// </code>
        /// </summary>
        [Obsolete("Use ToContainerProjection()")]
        public static ContainerProjection<TContainer, TItem> ToList<TContainer, TItem>(this RestResponse response) where TContainer : class
        {
            return new ContainerProjection<TContainer, TItem>(response);
        }

        #endregion

        /// <summary>
        /// Creates <see cref="DictionaryProjection<TValue>"/>
        /// <code>
        ///     // 1. map dictionary direct mapping
        ///     var projection = response.Dictionary{ICancelOrderResult}();
        ///     Response{IDictionary{string,ICancelOrderResult}} = projection.Map{CancelOrderResult}();
        ///     
        ///     // 2. map dictionary via proxy
        ///     var projection = response.Dictionary{ICancelOrderResult}();
        ///     Response{IDictionary{string,ICancelOrderResult}} = projection.MapWith{OrdersBuilder}();
        /// </code>
        /// </summary>
        public static DictionaryProjection<TValue> ToDictionary<TValue>(this RestResponse response)
        {
            return new DictionaryProjection<TValue>(response);
        }
    }
}