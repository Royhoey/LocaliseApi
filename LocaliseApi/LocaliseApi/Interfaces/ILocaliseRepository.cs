using LocaliseApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocaliseApi.Interfaces
{
	public interface ILocaliseRepository
	{
		Task<IEnumerable<Locale>> GetLocales();

		string GetTranslation(string id, string locale);

		Task<string> GetTranslations(string id);

		Task CreateAsset(string name, string id, List<string> tags, string type = "text");

		Task AddTranslation(string id, Locale locale, string translation);

		Task<bool> AssetExists(string id);
	}
}
