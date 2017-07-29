using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace XayahBot.Command.Precondition
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CategoryAttribute : PreconditionAttribute
    {
        public CategoryType CategoryType { get; }

        public CategoryAttribute(CategoryType category) : base()
        {
            this.CategoryType = category;
        }

        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
