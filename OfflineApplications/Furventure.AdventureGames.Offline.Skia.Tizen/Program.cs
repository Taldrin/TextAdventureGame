using Tizen.Applications;
using Uno.UI.Runtime.Skia;

namespace Furventure.AdventureGames.Offline.Skia.Tizen
{
	class Program
{
	static void Main(string[] args)
	{
		var host = new TizenHost(() => new Furventure.AdventureGames.Offline.App(), args);
		host.Run();
	}
}
}
