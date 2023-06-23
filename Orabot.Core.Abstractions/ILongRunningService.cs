using System.Threading;
using System.Threading.Tasks;

namespace Orabot.Core.Abstractions
{
	public interface ILongRunningService
	{
		Task ExecuteAsync(CancellationToken cancellationToken);
	}
}
