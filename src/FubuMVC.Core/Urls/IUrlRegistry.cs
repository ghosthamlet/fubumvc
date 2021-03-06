using System;
using System.Linq.Expressions;
using System.Reflection;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Urls
{
    public interface IUrlRegistry
    {
        /// <summary>
        /// Finds the url by input model and uses the "model" input to fill any
        /// substitutions in the route pattern
        /// </summary>
        /// <param name="model"></param>
        /// <param name="categoryOrHttpMethod"></param>
        /// <returns></returns>
        string UrlFor(object model, string categoryOrHttpMethod = null);
        
        /// <summary>
        /// Finds the url for a route by input model type first, then handler type.  Will throw an exception
        /// if more than one possible behavior chain is found for the type and category/http method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="categoryOrHttpMethod"></param>
        /// <returns></returns>
        string UrlFor<T>(string categoryOrHttpMethod = null) where T : class;
        
        /// <summary>
        /// Find the url for a behavior chain with the designated handlerType, method, and optionally a category
        /// or Http method
        /// </summary>
        /// <param name="handlerType"></param>
        /// <param name="method"></param>
        /// <param name="categoryOrHttpMethodOrHttpMethod"></param>
        /// <returns></returns>
        string UrlFor(Type handlerType, MethodInfo method = null, string categoryOrHttpMethodOrHttpMethod = null);
        
        
        /// <summary>
        /// Find the url for a behavior chain with the designated handlerType, method, and optionally a category
        /// or Http method
        /// </summary>
        /// <typeparam name="TController"></typeparam>
        /// <param name="expression"></param>
        /// <param name="categoryOrHttpMethod"></param>
        /// <returns></returns>
        string UrlFor<TController>(Expression<Action<TController>> expression, string categoryOrHttpMethod = null);

        /// <summary>
        /// Finds the url for the behavior chain that would create the designated type.  Matches against
        /// BehaviorChain.UrlCategory.Creates list
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        string UrlForNew(Type entityType);

        /// <summary>
        /// Tests whether or not there is an endpoint in the system for creating new entities of the entityType
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool HasNewUrl(Type type);

        /// <summary>
        /// Resolves an "open" url for the given input model object and optional categoryOrHttpMethod for use in client side 
        /// scripting.  Any missing values required by the Route pattern not on the model will be left as "${name}" substitutions
        /// </summary>
        /// <param name="model"></param>
        /// <param name="categoryOrHttpMethod"></param>
        /// <returns></returns>
        string TemplateFor(object model, string categoryOrHttpMethod = null);

        /// <summary>
        /// Resolves an "open" url for the given input model type and optional categoryOrHttpMethod for use in client side 
        /// scripting.  Any missing values required by the Route pattern not on the model will be left as "${name}" substitutions
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="hash"></param>
        /// <returns></returns>
        string TemplateFor<TModel>(params Func<object, object>[] hash) where TModel : class, new();

        /// <summary>
        /// Resolve a url for a model type, but using a RouteParameters object to define route substitutions.
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="parameters"></param>
        /// <param name="categoryOrHttpMethod"></param>
        /// <returns></returns>
        string UrlFor(Type modelType, RouteParameters parameters, string categoryOrHttpMethod = null);

        /// <summary>
        /// Resolve the url for a named asset
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        string UrlForAsset(AssetFolder? folder, string name);
    }

    public static class UrlRegistryExtensions
    {

        /// <summary>
        /// Finds the url for the behavior chain that would create the designated type.  Matches against
        /// BehaviorChain.UrlCategory.Creates list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="registry"></param>
        /// <returns></returns>
        public static string UrlForNew<T>(this IUrlRegistry registry)
        {
            return registry.UrlForNew(typeof (T));
        }

        /// <summary>
        /// Tests whether or not there is an endpoint in the system for creating new entities of the entityType
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="registry"></param>
        /// <returns></returns>
        public static bool HasNewUrl<T>(this IUrlRegistry registry)
        {
            return registry.HasNewUrl(typeof(T));
        }

        [Obsolete("This is an ancient Dovetail hack.  Getting eliminated whenever the DT guys say it's okay")]
        public static string UrlForPropertyUpdate(this IUrlRegistry registry, object model)
        {
            return registry.UrlFor(model, Categories.PROPERTY_EDIT);
        }

        [Obsolete("This is an ancient Dovetail hack.  Getting eliminated whenever the DT guys say it's okay")]
        public static string UrlForPropertyUpdate(this IUrlRegistry registry, Type type)
        {
            var o = Activator.CreateInstance(type);
            return registry.UrlForPropertyUpdate(o);
        }

        /// <summary>
        /// Resolve a url for a model type, but using a RouteParameters object to define route substitutions.
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <param name="registry"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string UrlFor<TInput>(this IUrlRegistry registry, RouteParameters parameters)
        {
            return registry.UrlFor(typeof(TInput), parameters);
        }

        /// <summary>
        /// Resolve a url for a model type, but using a RouteParameters object to define route substitutions.
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <param name="urls"></param>
        /// <param name="category"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string UrlFor<TInput>(this IUrlRegistry urls, string category, RouteParameters parameters)
        {
            return urls.UrlFor(typeof(TInput), parameters, category);
        }

        /// <summary>
        /// Inline way to resolve a url for an input model type by defining the RouteParameters object by
        /// expressions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="registry"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static string UrlFor<T>(this IUrlRegistry registry, Action<RouteParameters<T>> configure)
        {
            var parameters = new RouteParameters<T>();
            configure(parameters);

            return registry.UrlFor<T>(parameters);
        }

        /// <summary>
        /// Inline way to resolve a url for an input model type by defining the RouteParameters object by
        /// expressions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="registry"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static string UrlFor<T>(this IUrlRegistry registry, string category, Action<RouteParameters<T>> configure)
        {
            var parameters = new RouteParameters<T>();
            configure(parameters);

            return registry.UrlFor(typeof(T), parameters, category);
        }
    }
}