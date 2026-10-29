using System;
using Discord.Commands;

namespace Orabot.Bot.Discord.Commands
{
    public abstract class BaseTypeReader : TypeReader
    {
        public abstract Type SupportedType { get; }
    }
}
