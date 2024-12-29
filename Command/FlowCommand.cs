using System.Collections.Generic;

internal sealed class FlowCommand
{
    private readonly Stack<ICommand> _commands = new();
    private ICommand _curCommand;

    public void AddCommand(ICommand command)
    {
        _commands.Push(command);
    }

    public void Execute()
    {
        if (!Has())
        {
            return;
        }

        if (IsFinished())
        {
            return;
        }

        _curCommand = _commands.Pop();
        _curCommand.Execute();
    }

    private bool Has()
    {
        return !_commands.IsNullOrEmpty();
    }

    private bool IsFinished()
    {
        return _curCommand != null && _curCommand.IsFinished();
    }
}