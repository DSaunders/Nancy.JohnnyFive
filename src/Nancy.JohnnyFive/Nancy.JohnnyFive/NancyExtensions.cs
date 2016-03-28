namespace Nancy.JohnnyFive
{
    using System.Collections;
    using System.Collections.Generic;
    using Circuits;
    using Models;

    public static class NancyExtensions
    {
        public static void CanShortCircuit(this NancyModule module, ICircuit circuit = null)
        {
            if (!module.Context.Items.ContainsKey(Constants.ContextItemName))
                module.Context.Items[Constants.ContextItemName] = new List<ICircuit>();

            ((IList)module.Context.Items[Constants.ContextItemName]).Add(circuit);

            // ?? new LastGoodResponseOnErrorCircuit()
        }
    }

}