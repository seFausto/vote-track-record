using Microsoft.Extensions.Options;
using VotingTrackRecord;

namespace Propublica
{
    public class PropublicaApi
    {
        PropublicaSettings propublicaSettings;

        public PropublicaApi(IOptions<PropublicaSettings> options)
        {
            propublicaSettings = options.Value;
        }
        

        public async Task<string> GetCongressMembers(string state, string district)
        {
            var client = new HttpClient();
            var response = await client.GetAsync($"https://api.propublica.org/congress/v1/members/{state}/{district}/current.json");
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }
    }
}