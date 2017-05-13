using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XayahBot.Database.Model;
using XayahBot.Error;

namespace XayahBot.Database.DAO
{
    public class IgnoreListDAO
    {
        public TIgnoreEntry GetSingle(ulong subjectId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                TIgnoreEntry match = database.IgnoreList.FirstOrDefault(x => x.SubjectId.Equals(subjectId));
                return match ?? throw new NotExistingException();
            }
        }

        public TIgnoreEntry GetSingle(ulong guildId, ulong subjectId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                TIgnoreEntry match = this.GetAll(guildId).FirstOrDefault(x => x.SubjectId.Equals(subjectId));
                return match ?? throw new NotExistingException();
            }
        }

        public List<TIgnoreEntry> GetAll(ulong guildId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                return database.IgnoreList.Where(x => x.GuildId.Equals(guildId)).ToList();
            }
        }

        public async Task SaveAsync(TIgnoreEntry entry)
        {
            if (this.HasSubject(entry.GuildId, entry.SubjectId))
            {
                throw new AlreadyExistingException();
            }
            else
            {
                await this.AddAsync(entry);
            }
        }

        private async Task AddAsync(TIgnoreEntry entry)
        {
            using (GeneralContext database = new GeneralContext())
            {
                database.IgnoreList.Add(entry);
                if (await database.SaveChangesAsync() <= 0)
                {
                    throw new NotSavedException();
                }
            }
        }

        public async Task RemoveBySubjectIdAsync(ulong subjectId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                TIgnoreEntry match = this.GetSingle(subjectId);
                database.Remove(match);
                if (await database.SaveChangesAsync() <= 0)
                {
                    throw new NotSavedException();
                }
            }
        }

        public async Task RemoveByGuildIdAsync(ulong guildId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                List<TIgnoreEntry> matches = this.GetAll(guildId);
                if (matches.Count > 0)
                {
                    database.RemoveRange(matches);
                    if (await database.SaveChangesAsync() <= 0)
                    {
                        throw new NotSavedException();
                    }
                }
            }
        }

        public async Task RemoveByGuildAndSubjectIdAsync(ulong guildId, ulong subjectId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                TIgnoreEntry match = this.GetSingle(guildId, subjectId);
                database.Remove(match);
                if (await database.SaveChangesAsync() <= 0)
                {
                    throw new NotSavedException();
                }
            }
        }

        public bool HasSubject(ulong subjectId)
        {
            try
            {
                this.GetSingle(subjectId);
                return true;
            }
            catch (NotExistingException)
            {
                return false;
            }
        }

        public bool HasSubject(ulong guildId, ulong subjectId)
        {
            try
            {
                this.GetSingle(guildId, subjectId);
                return true;
            }
            catch (NotExistingException)
            {
                return false;
            }
        }
    }
}
