using System.Collections.Generic;
using System.Linq;
using XayahBot.Error;

namespace XayahBot.Command.Precondition
{
    public class Category
    {
        public static Category Account = new Category(CategoryType.ACCOUNT, "account");
        public static Category Data = new Category(CategoryType.DATA, "data");
        public static Category Help = new Category(CategoryType.HELP, "help");
        public static Category Ignore = new Category(CategoryType.IGNORE, "ignore");
        public static Category Incidents = new Category(CategoryType.INCIDENTS, "incidents");
        public static Category Misc = new Category(CategoryType.MISC, "misc");
        public static Category Quiz = new Category(CategoryType.QUIZ, "quiz");
        public static Category Remind = new Category(CategoryType.REMIND, "remind me");
        public static Category Owner = new Category(CategoryType.OWNER, "owner");

        public static IEnumerable<Category> Values
        {
            get
            {
                yield return Account;
                yield return Data;
                yield return Help;
                yield return Ignore;
                yield return Incidents;
                yield return Misc;
                yield return Quiz;
                yield return Remind;
                yield return Owner;
            }
        }

        public static Category GetByName(string name)
        {
            return Values.FirstOrDefault(x => x.Name.ToLower().Equals(name.Trim().ToLower())) ?? Help;
        }

        public static Category GetByType(CategoryType categoryType)
        {
            return Values.FirstOrDefault(x => x.CategoryType.Equals(categoryType)) ?? throw new UnknownTypeException();
        }

        // ---

        public CategoryType CategoryType { get; }
        public string Name { get; }

        private Category(CategoryType categoryType, string name)
        {
            this.CategoryType = categoryType;
            this.Name = name;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
