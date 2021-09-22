using System;
using Discord.Commands;

namespace Orabot.Core.TypeReaders
{
    public abstract class BaseTypeReader : TypeReader
    {
        public abstract Type SupportedType { get; }
    }
}
