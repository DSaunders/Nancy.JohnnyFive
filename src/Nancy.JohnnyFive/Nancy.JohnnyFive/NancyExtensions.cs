namespace Nancy.JohnnyFive
{
    using System.Collections;
    using System.Collections.Generic;
    using Circuits;
    using Models;
    using Responders;

    public static class NancyExtensions
    {
        public static RouteConfig CanShortCircuit(this NancyModule module)
        {
            // New up config with defaults (can be overridden by extension methods on RouteConfig)
            var config = new RouteConfig
            {
                Circuit = new OnErrorCircuit(),
                Responder = new LastGoodResponseResponder()
            };

            if (!module.Context.Items.ContainsKey(Constants.ContextItemName))
                module.Context.Items[Constants.ContextItemName] = new List<RouteConfig>();

            ((IList)module.Context.Items[Constants.ContextItemName]).Add(config);

            return config;
        }
    }

}