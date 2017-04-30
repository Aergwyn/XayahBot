using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XayahBot.Database.Model;
using XayahBot.Error;

namespace XayahBot.Database.Service
{
    public class IgnoreDAO
    {
        public List<TIgnoreEntry> GetIgnoreList(ulong guildId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                return database.IgnoreList.Where(x => x.GuildId.Equals(guildId)).ToList();
            }
        }

        public async Task AddAsync(TIgnoreEntry entry)
        {
            using (GeneralContext database = new GeneralContext())
            {
                TIgnoreEntry match = database.IgnoreList.FirstOrDefault(x => x.GuildId.Equals(entry.GuildId) && x.SubjectId.Equals(entry.SubjectId));
                if (match == null)
                {
                    database.IgnoreList.Add(entry);
                    if (await database.SaveChangesAsync() <= 0)
                    {
                        throw new NotSavedException();
                    }
                }
                else
                {
                    throw new AlreadyExistingException();
                }
            }
        }

        public async Task UpdateAsync(ulong subjectId, string newSubjectName)
        {
            using (GeneralContext database = new GeneralContext())
            {
                TIgnoreEntry match = database.IgnoreList.FirstOrDefault(x => x.SubjectId.Equals(subjectId));
                if (match != null)
                {
                    match.SubjectName = newSubjectName;
                    if (await database.SaveChangesAsync() <= 0)
                    {
                        throw new NotSavedException();
                    }
                }
                else
                {
                    throw new NotExistingException();
                }
            }
        }

        public async Task RemoveAsync(ulong guildId, ulong subjectId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                TIgnoreEntry match = database.IgnoreList.FirstOrDefault(x => x.GuildId.Equals(guildId) && x.SubjectId.Equals(subjectId));
                if (match != null)
                {
                    database.Remove(match);
                    if (await database.SaveChangesAsync() <= 0)
                    {
                        throw new NotSavedException();
                    }
                }
                else
                {
                    throw new NotExistingException();
                }
            }
        }

        public async Task RemoveByGuildAsync(ulong guildId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                List<TIgnoreEntry> matches = database.IgnoreList.Where(x => x.GuildId.Equals(guildId)).ToList();
                if (matches.Count > 0)
                {
                    database.RemoveRange(matches);
                    if (await database.SaveChangesAsync() <= 0)
                    {
                        throw new NotSavedException();
                    }
                }
                else
                {
                    throw new NotExistingException();
                }
            }
        }

        public async Task RemoveBySubjectIdAsync(ulong subjectId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                List<TIgnoreEntry> matches = database.IgnoreList.Where(x => x.SubjectId.Equals(subjectId)).ToList();
                if (matches.Count > 0)
                {
                    database.RemoveRange(matches);
                    if (await database.SaveChangesAsync() <= 0)
                    {
                        throw new NotSavedException();
                    }
                }
                else
                {
                    throw new NotExistingException();
                }
            }
        }

        //

        public bool IsIgnored(ulong guildId, ulong subjectId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                if (database.IgnoreList.FirstOrDefault(x => x.GuildId.Equals(guildId) && x.SubjectId.Equals(subjectId)) != null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
