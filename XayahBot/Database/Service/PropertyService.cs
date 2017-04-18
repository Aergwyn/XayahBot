﻿using System.Linq;
using System.Threading.Tasks;
using XayahBot.Database.Model;
using XayahBot.Utility;

namespace XayahBot.Database.Service
{
    public static class PropertyService
    {
        public static string GetValue(Property property)
        {
            using (GeneralContext db = new GeneralContext())
            {
                TProperty dbProperty = db.Properties.FirstOrDefault(x => x.Name.Equals(property.Name));
                if (dbProperty != null)
                {
                    return dbProperty.Value;
                }
            }
            return null;
        }

        public static Task SetValueAsync(Property property)
        {
            using (GeneralContext db = new GeneralContext())
            {
                TProperty dbProperty = db.Properties.FirstOrDefault(x => x.Name.Equals(property.Name));
                if (dbProperty == null)
                {
                    db.Properties.Add(new TProperty { Name = property.Name, Value = property.Value });
                }
                else
                {
                    dbProperty.Value = property.Value;
                }
                db.SaveChangesAsync();
            }
            return Task.CompletedTask;
        }
    }
}
