using System;
using System.Threading.Tasks;
using Discord.Commands;
using XayahBot.Command.ChampionGGData;
using XayahBot.Error;
using XayahBot.Utility;

namespace XayahBot.Command.Logic
{
    public class LeagueRoleTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> Read(ICommandContext context, string input, IServiceProvider services)
        {
            try
            {
                LeagueRole match = LeagueRole.Get(input);
                return Task.FromResult(TypeReaderResult.FromSuccess(match));
            }
            catch (NotExistingException)
            {
                string roles = ListUtil.BuildEnumeration(LeagueRole.Values());
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, $"Role `{input}` doesn't exist. Choose one of `{roles}`"));
            }
        }
    }
}
