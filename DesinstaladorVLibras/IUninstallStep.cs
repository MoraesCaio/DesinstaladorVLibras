using System;
using System.Collections.Generic;

namespace DesinstaladorVLibras
{
    public interface IUninstallStep : IDisposable
    {
        void Prepare(List<string> componentsToRemove);

        void PrintDebugInformation();

        void Execute();
    }
}
