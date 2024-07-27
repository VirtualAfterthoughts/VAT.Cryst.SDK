using System.Collections.Generic;

namespace VAT.Logic
{
    public interface INode
    {
        void OnLogicUpdate(float deltaTime);
    }

    public interface IDonorNode : INode
    {
        Signal ProcessedSignal { get; }

        List<Port> Outputs { get; }

        void AddOutput(Port output)
        {
            Outputs.Add(output);
        }

        void RemoveOutput(Port output)
        {
            Outputs.Remove(output);
        }
    }

    public interface IReceiverNode : INode
    {
        List<Port> Inputs { get; }

        void AddInput(Port input)
        {
            Inputs.Add(input);
        }

        void RemoveInput(Port input)
        {
            Inputs.Remove(input);
        }
    }
}
