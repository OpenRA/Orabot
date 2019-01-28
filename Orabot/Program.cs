namespace Orabot
{
	internal class Program
	{
		private static void Main()
		{
			using (var bot = new Bot())
			{
				bot.RunAsync().Wait();
			}
		}
	}
}
