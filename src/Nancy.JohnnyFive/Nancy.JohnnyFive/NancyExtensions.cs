namespace Nancy.JohnnyFive
{
    using System.Collections;
    using System.Collections.Generic;
    using Circuits;
    using Models;

    public static class NancyExtensions
    {
        public static RouteConfig CanShortCircuit(this NancyModule module)
        {
            var config = new RouteConfig();

            // TODO: Set defaults

            if (!module.Context.Items.ContainsKey(Constants.ContextItemName))
                module.Context.Items[Constants.ContextItemName] = new List<ICircuit>();

            ((IList)module.Context.Items[Constants.ContextItemName]).Add(config);

            // ?? new LastGoodResponseOnErrorCircuit()
        }
    }

}