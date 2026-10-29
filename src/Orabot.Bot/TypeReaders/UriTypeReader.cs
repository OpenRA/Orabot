using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace Orabot.Core.TypeReaders
{
    public class UriTypeReader : BaseTypeReader
    {
        public override Type SupportedType => typeof(Uri);

        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
	        return Task.FromResult(Uri.IsWellFormedUriString(input, UriKind.Absolute)
		        ? TypeReaderResult.FromSuccess(new Uri(input))
		        : TypeReaderResult.FromError(CommandError.ParseFailed, "Input could not be parsed as an URI."));
        }
    }
}
