using System;

namespace IndustrialPark.Scripting;

public sealed class ScriptGlobals
{
    public Action<string> Log { get; }

    public ScriptGlobals(Action<string> log)
    {
        Log = log;
    }
}
