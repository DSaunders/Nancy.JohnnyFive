namespace Nancy.JohnnyFive.Startup
{
    using System.Collections.Generic;
    using Bootstrapper;
    using Models;
    using Store;

    public class AppStartup : IApplicationStartup
    {
        private readonly IStore _db;

        public AppStartup(IStore db)
        {
            _db = db;
        }

        public void Initialize(IPipelines pipelines)
        {
            pipelines.AfterRequest += (ctx) =>
            {
                if (!ctx.Items.ContainsKey(Constants.ContextItemName))
                    return;
                
                _db.AddIfNotExists(ctx.Request.Path, ctx.Items[Constants.ContextItemName] as IList<RouteConfig>);

                foreach (var config in _db.GetForRoute(ctx.Request.Path))
                {
                    config.Circuit.AfterRequest(ctx.Response);
                    config.Responder.AfterRequest(ctx.Response);
                }
            };

            pipelines.BeforeRequest += (ctx) =>
            {
                foreach (var config in _db.GetForRoute(ctx.Request.Path))
                {
                    config.Circuit.BeforeRequest(ctx.Request);

                    if (config.Circuit.State == CircuitState.ShortCircuit)
                    {
                        if (config.OnShortCircuitCallback != null)
                            config.OnShortCircuitCallback();
                        return config.Responder.GetResponse();
                    }
                }

                return null;
            };

            pipelines.OnError += (ctx, ex) =>
            {
                if (ctx.Items.ContainsKey(Constants.ContextItemName))
                    _db.AddIfNotExists(ctx.Request.Path, ctx.Items[Constants.ContextItemName] as IList<RouteConfig>);
                
                foreach (var config in _db.GetForRoute(ctx.Request.Path))
                {
                    config.Circuit.OnError(ex);
                    if (config.Circuit.State == CircuitState.ShortCircuit)
                    {
                        if (config.OnShortCircuitCallback != null)
                            config.OnShortCircuitCallback();
                        return config.Responder.GetResponse();
                    }
                        
                }

                return null;
            };
        }
    }
}