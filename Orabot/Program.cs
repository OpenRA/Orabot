namespace Orabot
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			using (var bot = new Bot())
			{
				bot.RunAsync().Wait();
			}
		}
	}
}
