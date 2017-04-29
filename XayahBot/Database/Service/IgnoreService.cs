using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XayahBot.Database.Model;
using XayahBot.Error;

namespace XayahBot.Database.Service
{
    public class IgnoreService
    {
        public List<TIgnoreEntry> GetIgnoreList(ulong guild)
        {
            using (GeneralContext database = new GeneralContext())
            {
                return database.IgnoreList.Where(x => x.Guild.Equals(guild)).ToList();
            }
        }

        public async Task AddAsync(TIgnoreEntry ignoreEntry)
        {
            using (GeneralContext database = new GeneralContext())
            {
                TIgnoreEntry match = database.IgnoreList.FirstOrDefault(x => x.Guild.Equals(ignoreEntry.Guild) && x.SubjectId.Equals(ignoreEntry.SubjectId));
                if (match == null)
                {
                    database.IgnoreList.Add(ignoreEntry);
                    if (await database.SaveChangesAsync() <= 0)
                    {
                        throw new NotSavedException();
                    }
                }
            }
            throw new AlreadyExistingException();
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
            }
            throw new NotExistingException();
        }

        public async Task RemoveAsync(ulong guild, ulong subjectId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                TIgnoreEntry match = database.IgnoreList.FirstOrDefault(x => x.Guild.Equals(guild) && x.SubjectId.Equals(subjectId));
                if (match != null)
                {
                    database.Remove(match);
                    if (await database.SaveChangesAsync() <= 0)
                    {
                        throw new NotSavedException();
                    }
                }
            }
            throw new NotExistingException();
        }

        public async Task RemoveByGuildAsync(ulong guild)
        {
            using (GeneralContext database = new GeneralContext())
            {
                List<TIgnoreEntry> matches = database.IgnoreList.Where(x => x.Guild.Equals(guild)).ToList();
                if (matches.Count > 0)
                {
                    database.RemoveRange(matches);
                    if (await database.SaveChangesAsync() <= 0)
                    {
                        throw new NotSavedException();
                    }
                }
            }
            throw new NotExistingException();
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
            }
            throw new NotExistingException();
        }

        //

        public bool IsIgnored(ulong guild, ulong subjectId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                if (database.IgnoreList.FirstOrDefault(x => x.Guild.Equals(guild) && x.SubjectId.Equals(subjectId)) != null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
