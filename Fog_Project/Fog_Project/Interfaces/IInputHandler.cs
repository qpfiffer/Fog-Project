using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fog_Project.Utilities;

namespace Fog_Project.Interfaces
{
    public interface IInputHandler
    {
        void handleInput(ref InputInfo info);
    }
}
