using System;
using System.Threading.Tasks;
using Discord.Commands;
using Orabot.Objects;

namespace Orabot.TypeReaders
{
    public class DiscordMessageIdentifierTypeReader : BaseTypeReader
    {
        public override Type SupportedType => typeof(DiscordMessageIdentifier);

        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
	        return Task.FromResult(DiscordMessageIdentifier.TryParse(input, out var identifier)
		        ? TypeReaderResult.FromSuccess(identifier)
		        : TypeReaderResult.FromError(CommandError.ParseFailed, "Input could not be parsed as a Discord Message Identifier."));
        }
    }
}
