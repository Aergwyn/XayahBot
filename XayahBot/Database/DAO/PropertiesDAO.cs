using System.Linq;
using System.Threading.Tasks;
using XayahBot.Database.Model;
using XayahBot.Error;
using XayahBot.Utility;

namespace XayahBot.Database.DAO
{
    public class PropertiesDAO
    {
        public string GetValue(Property property)
        {
            using (GeneralContext database = new GeneralContext())
            {
                TProperty match = database.Properties.FirstOrDefault(x => x.Name.Equals(property.Name));
                if (match != null)
                {
                    return match.Value;
                }
            }
            throw new NotExistingException();
        }

        public Task SetValueAsync(Property property)
        {
            using (GeneralContext database = new GeneralContext())
            {
                TProperty dbProperty = database.Properties.FirstOrDefault(x => x.Name.Equals(property.Name));
                if (dbProperty == null)
                {
                    database.Properties.Add(new TProperty
                    {
                        Name = property.Name,
                        Value = property.Value
                    });
                }
                else
                {
                    dbProperty.Value = property.Value;
                }
                database.SaveChangesAsync();
            }
            return Task.CompletedTask;
        }
    }
}
