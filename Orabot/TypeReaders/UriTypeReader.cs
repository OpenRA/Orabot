using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace SteakBot.Core.TypeReaders
{
    public class UriTypeReader : BaseTypeReader
    {
        public override Type SupportedType => typeof(Uri);

        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            if (Uri.IsWellFormedUriString(input, UriKind.Absolute))
            {
                return Task.FromResult(TypeReaderResult.FromSuccess(new Uri(input)));
            }

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Input could not be parsed as an URI."));
        }
    }
}
