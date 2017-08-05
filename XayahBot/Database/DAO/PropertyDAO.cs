using System.Linq;
using System.Threading.Tasks;
using XayahBot.Database.Model;
using XayahBot.Error;
using XayahBot.Utility;

namespace XayahBot.Database.DAO
{
    public class PropertyDAO
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

        public void SetValue(Property property)
        {
            using (GeneralContext database = new GeneralContext())
            {
                TProperty dbProperty = database.Properties.FirstOrDefault(x => x.Name.Equals(property.Name));
                if (dbProperty == null)
                {
                    dbProperty = new TProperty
                    {
                        Name = property.Name
                    };
                    database.Properties.Add(dbProperty);
                }
                dbProperty.Value = property.Value;
                database.SaveChanges();
            }
        }
    }
}
