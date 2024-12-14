internal interface ICommand
{
    void Execute();
    bool IsFinished();
}