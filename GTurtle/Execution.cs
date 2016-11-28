using Gemini.Framework.Commands;
using GTurtle.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTurtle
{
    public class Execution :
        ICommandHandler<Play>,
        ICommandHandler<Pause>,
        ICommandHandler<StepInto>,
        ICommandHandler<StepOut>,
        ICommandHandler<StepOver>,
        ICommandHandler<Stop>

    {
        Task ICommandHandler<StepInto>.Run(Command command)
        {
            return Task.CompletedTask;
        }

        Task ICommandHandler<StepOver>.Run(Command command)
        {
            return Task.CompletedTask;
        }

        Task ICommandHandler<Stop>.Run(Command command)
        {
            return Task.CompletedTask;
        }

        Task ICommandHandler<StepOut>.Run(Command command)
        {
            return Task.CompletedTask;
        }

        Task ICommandHandler<Pause>.Run(Command command)
        {
            return Task.CompletedTask;
        }

        Task ICommandHandler<Play>.Run(Command command)
        {
            return Task.CompletedTask;
        }

        void ICommandHandler<StepInto>.Update(Command command)
        {
            
        }

        void ICommandHandler<StepOver>.Update(Command command)
        {
            
        }

        void ICommandHandler<Stop>.Update(Command command)
        {
            
        }

        void ICommandHandler<StepOut>.Update(Command command)
        {
            
        }

        void ICommandHandler<Pause>.Update(Command command)
        {
            
        }

        void ICommandHandler<Play>.Update(Command command)
        {
            
        }
    }
}
