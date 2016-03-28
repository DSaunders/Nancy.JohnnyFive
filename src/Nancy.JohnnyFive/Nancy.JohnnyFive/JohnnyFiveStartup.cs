//namespace Nancy.JohnnyFive
//{
//    using System.Collections.Generic;
//    using Bootstrapper;
//    using Circuits;
//    using Models;
//    using Store;

//    public class JohnnyFiveStartup : IApplicationStartup
//    {
//        private readonly IJohhnyFiveStore _db;

//        public JohnnyFiveStartup(IJohhnyFiveStore db)
//        {
//            _db = db;
//        }

//        public void Initialize(IPipelines pipelines)
//        {
//            pipelines.AfterRequest += (ctx) =>
//            {
//                if (!ctx.Items.ContainsKey(Constants.ContextItemName))
//                    return;

//                _db.AddIfNotExists(ctx.Request.Path, ctx.Items[Constants.ContextItemName] as IList<ICircuit>);

//                foreach (var circuit in _db.GetForRoute(ctx.Request.Path))
//                    circuit.AfterRequest(ctx.Response);
//            };

//            pipelines.BeforeRequest += (ctx) =>
//            {
//                foreach (var circuit in _db.GetForRoute(ctx.Request.Path))
//                {
//                    var result = circuit.BeforeRequest();
//                    if (result != null)
//                        return result;
//                }

//                return null;
//            };

//            pipelines.OnError += (ctx, ex) =>
//            {
//                if (!ctx.Items.ContainsKey(Constants.ContextItemName))
//                    return null;

//                _db.AddIfNotExists(ctx.Request.Path, ctx.Items[Constants.ContextItemName] as IList<ICircuit>);

//                foreach (var circuit in _db.GetForRoute(ctx.Request.Path))
//                {
//                    var result = circuit.OnError(ex);
//                    if (result != null)
//                        return result;
//                }

//                return null;
//            };
//        }
//    }
//}