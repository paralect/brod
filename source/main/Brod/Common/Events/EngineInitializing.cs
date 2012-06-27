using System;

namespace Brod.Common.Events
{
    [Serializable]
    public sealed class EngineInitializing : ISystemEvent
    {
        public override string ToString()
        {
            return "Engine initializing";
        }
    }
}