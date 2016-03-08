namespace Nancy.JohnnyFive
{
    using System.Collections.Generic;
    using Bootstrapper;
    using Circuits;
    using Database;
    using Models;

    public class JohnnyFiveStartup : IApplicationStartup
    {
        private readonly IJohhnyFiveDatabase _db;

        public JohnnyFiveStartup(IJohhnyFiveDatabase db)
        {
            _db = db;
        }

        public void Initialize(IPipelines pipelines)
        {
            pipelines.AfterRequest += (ctx) =>
            {
                if (!ctx.Items.ContainsKey(Constants.ContextItemName))
                    return;

                EnsureCircuitsAreInDatabase(ctx);

                foreach (var circuit in _db.CircuitBreakers[ctx.Request.Path])
                {
                    circuit.AfterRequest(ctx.Response);
                }

            };

            pipelines.BeforeRequest += (ctx) =>
            {
                if (!_db.CircuitBreakers.ContainsKey(ctx.Request.Path))
                    return null;

                foreach (var circuit in _db.CircuitBreakers[ctx.Request.Path])
                {
                    var result = circuit.BeforeRequest();
                    if (result != null)
                        return result;
                }

                return null;
            };


            pipelines.OnError += (ctx, ex) =>
            {
                if (!ctx.Items.ContainsKey(Constants.ContextItemName))
                    return null;

                EnsureCircuitsAreInDatabase(ctx);

                foreach (var circuit in _db.CircuitBreakers[ctx.Request.Path])
                {
                    var result = circuit.OnError(ex);
                    if (result != null)
                        return result;
                }

                return null;
            };
        }

        private void EnsureCircuitsAreInDatabase(NancyContext ctx)
        {
            // The first time this is called we add the circtuis to the singleton.
            if (!_db.CircuitBreakers.ContainsKey(ctx.Request.Path))
                _db.CircuitBreakers.Add(ctx.Request.Path, (IList<ICircuit>)ctx.Items[Constants.ContextItemName]);
        }
    }
}