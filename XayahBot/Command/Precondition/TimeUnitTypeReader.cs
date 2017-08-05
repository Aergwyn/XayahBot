using System;
using System.Threading.Tasks;
using Discord.Commands;
using XayahBot.Command.Remind;
using XayahBot.Error;
using XayahBot.Utility;

namespace XayahBot.Command.Precondition
{
    public class TimeUnitTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> Read(ICommandContext context, string input, IServiceProvider services)
        {
            try
            {
                TimeUnit match = TimeUnit.Get(input);
                return Task.FromResult(TypeReaderResult.FromSuccess(match));
            }
            catch (NotExistingException)
            {
                string timeUnits = ListUtil.BuildEnumeration(TimeUnit.Values);
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, $"Time-unit `{input}` doesn't exist. Choose one of `{timeUnits}`"));
            }
        }
    }
}
