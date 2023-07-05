using System.Threading.Tasks;
using Discord.Commands;
using Orabot.Core.Transformers.DocumentationToEmbedTransformers;

namespace Orabot.Core.Modules
{
	public class OpenRaWeaponsModule : ModuleBase<SocketCommandContext>
	{
		private readonly WeaponToEmbedTransformer _weaponToEmbedTransformer;

		public OpenRaWeaponsModule(WeaponToEmbedTransformer weaponToEmbedTransformer)
		{
			_weaponToEmbedTransformer = weaponToEmbedTransformer;
		}

		[Command("weapons")]
		[Summary("Provides a link to the OpenRA Weapons documentation page. Can be used with an optional weapon name to link directly.")]
		public async Task Weapons(string weaponName = null)
		{
			var embed = await _weaponToEmbedTransformer.CreateEmbed(weaponName, "release");
			if (embed != null)
				await ReplyAsync("", false, embed);
		}

		[Command("weapons-pt")]
		[Summary("Provides a link to the OpenRA playtest Weapons documentation page. Can be used with an optional weapon name to link directly.")]
		public async Task WeaponsPt(string weaponName = null)
		{
			var embed = await _weaponToEmbedTransformer.CreateEmbed(weaponName, "playtest");
			if (embed != null)
				await ReplyAsync("", false, embed);
		}

		[Command("weapons-dev")]
		[Summary("Provides a link to the OpenRA development Weapons documentation page. Can be used with an optional weapon name to link directly.")]
		public async Task WeaponsDev(string weaponName = null)
		{
			var embed = await _weaponToEmbedTransformer.CreateEmbed(weaponName, "development");
			if (embed != null)
				await ReplyAsync("", false, embed);
		}
	}
}
