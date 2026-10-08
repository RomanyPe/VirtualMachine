using ExtensionsVMApplication.Optimizators.AstRules;
using Kernel.Contracts;
using VMApplication.Project;

namespace ExtensionsVMApplication.Optimizators;

public static class AstOptimizerExtension
{
    extension(VMAstOptimizer optimizer)
    {
        public VMAstOptimizer AddStdRules()
        {
            optimizer.Add(new PropagateConstantsRule());
            optimizer.Add(new FoldConstantsRule());
            optimizer.Add(new RemoveUnusedVariablesRule());
            optimizer.Add(new RemoveUnreachableCodeRule());
            optimizer.Add(new InlineSmallVoidFunctionsRule());
            optimizer.Add(new RemoveUnreachableCodeRule());
            return optimizer;
        }
    }


    public static readonly OptimizationId FunctionInlined = new("inline.func");
    public static readonly OptimizationId ConstantPropagated = new("const.propagate");
    public static readonly OptimizationId ConstantFolded = new("const.fold");
    public static readonly OptimizationId NodeRemoved = new("node.removed");
    public static readonly OptimizationId VariableRemoved = new("node.removed.variable-unused");

}
