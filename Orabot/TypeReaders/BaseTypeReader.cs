using System;
using Discord.Commands;

namespace SteakBot.Core.TypeReaders
{
    public abstract class BaseTypeReader : TypeReader
    {
        public abstract Type SupportedType { get; }
    }
}
