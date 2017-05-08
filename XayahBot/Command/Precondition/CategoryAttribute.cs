#pragma warning disable 1998

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

        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            return PreconditionResult.FromSuccess();
        }
    }
}
