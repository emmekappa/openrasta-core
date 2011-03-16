using System.Collections.Generic;
using System.Linq;
using OpenRasta.Diagnostics;
using OpenRasta.OperationModel;
using OpenRasta.Pipeline.Diagnostics;
using OpenRasta.Web;
using OpenRasta.Pipeline;

namespace OpenRasta.Pipeline.Contributors
{
    public class OperationCreatorContributor : KnownStages.IOperationCreation
    {
        readonly IOperationCreator _creator;

        public OperationCreatorContributor(IOperationCreator creator)
        {
            _creator = creator;
            Logger = NullLogger<PipelineLogSource>.Instance;
        }

        public ILogger<PipelineLogSource> Logger { get; set; }

        public void Initialize(IPipeline pipelineRunner)
        {
            pipelineRunner.Notify(CreateOperations).After<KnownStages.IHandlerSelection>();
        }

        PipelineContinuation CreateOperations(ICommunicationContext context)
        {
            if (context.Environment.SelectedHandlers != null)
            {
                context.Environment.Operations = _creator.CreateOperations(context.Environment.SelectedHandlers).ToList();
                LogOperations(context.Environment.Operations);
                if (context.Environment.Operations.Count() == 0)
                {
                    context.OperationResult = CreateMethodNotAllowed(context);
                    return PipelineContinuation.RenderNow;
                }
            }
            return PipelineContinuation.Continue;
        }

        OperationResult.MethodNotAllowed CreateMethodNotAllowed(ICommunicationContext context)
        {
            return new OperationResult.MethodNotAllowed(context.Request.Uri, context.Request.HttpMethod, context.Environment.ResourceKey);
        }

        void LogOperations(IEnumerable<IOperation> operations)
        {
            if (operations.Count() > 0)
            {
                foreach (var operation in operations)
                    Logger.WriteDebug("Created operation named {0} with signature {1}", operation.Name, operation.ToString());
            }
            else
            {
                Logger.WriteDebug("No operation was created.");
            }
        }
    }
}
