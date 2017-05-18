using System;
using System.Threading.Tasks;
using Discord.Commands;
using XayahBot.API.Riot;
using XayahBot.Error;
using XayahBot.Utility;

namespace XayahBot.Command.Precondition
{
    public class RegionTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> Read(ICommandContext context, string input)
        {
            try
            {
                Region match = Region.GetByName(input);
                return Task.FromResult(TypeReaderResult.FromSuccess(match));
            }
            catch (UnknownTypeException)
            {
                string regions = ListUtil.BuildEnumeration(Region.Values);
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, $"Region \"{input}\" does not exist. Choose one: {regions}"));
            }
        }
    }
}
