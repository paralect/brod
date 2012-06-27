using System;

namespace Brod.Common.Events
{
    [Serializable]
    public sealed class EngineInitialized : ISystemEvent
    {
        public override string ToString()
        {
            return "Engine initialized";
        }
    }
}