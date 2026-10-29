using System.Threading;
using System.Threading.Tasks;

namespace Orabot.Bot.Infrastructure.BackgroundWork
{
	public interface ILongRunningService
	{
		Task ExecuteAsync(CancellationToken cancellationToken);
	}
}
