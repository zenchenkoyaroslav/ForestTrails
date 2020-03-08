namespace ForestTrails.Command
{
    public class Invoker
    {
        private ICommand command;

        public void SetCommand(ICommand command)
        {
            this.command = command;
        }

        public void Execute()
        {
            if(command != null)
            {
                command.Execute();
            }
        }

        public void ClearCommand()
        {
            command = null;
        }
    }
}
