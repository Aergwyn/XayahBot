using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XayahBot.Database.Model;

namespace XayahBot.Database.Service
{
    public static class IgnoreService
    {
        public static List<TIgnoreEntry> GetIgnoreList(ulong guild)
        {
            using (GeneralContext db = new GeneralContext())
            {
                return db.IgnoreList.Where(x => x.Guild.Equals(guild)).ToList();
            }
        }

        public static async Task<int> AddAsync(ulong guild, ulong subjectId, string subjectName, bool isChannel)
        {
            using (GeneralContext db = new GeneralContext())
            {
                TIgnoreEntry match = db.IgnoreList.FirstOrDefault(x => x.Guild.Equals(guild) && x.SubjectId.Equals(subjectId));
                if (match == null)
                {
                    db.IgnoreList.Add(new TIgnoreEntry { Guild = guild, SubjectId = subjectId, SubjectName = subjectName, IsChannel = isChannel });
                    if (await db.SaveChangesAsync() > 0)
                    {
                        return 0; // Success
                    }
                    else
                    {
                        return 1; // Insert failed
                    }
                }
            }
            return 2; // Already existing
        }

        public static async Task<int> UpdateAsync(ulong subjectId, string newSubjectName)
        {
            using (GeneralContext db = new GeneralContext())
            {
                TIgnoreEntry match = db.IgnoreList.FirstOrDefault(x => x.SubjectId.Equals(subjectId));
                if (match != null)
                {
                    match.SubjectName = newSubjectName;
                    if (await db.SaveChangesAsync() > 0)
                    {
                        return 0; // Success
                    }
                    else
                    {
                        return 1; // Update failed
                    }
                }
            }
            return 2; // No entry found
        }

        public static async Task<int> RemoveAsync(ulong guild, ulong subjectId)
        {
            using (GeneralContext db = new GeneralContext())
            {
                TIgnoreEntry match = db.IgnoreList.FirstOrDefault(x => x.Guild.Equals(guild) && x.SubjectId.Equals(subjectId));
                if (match != null)
                {
                    db.Remove(match);
                    if (await db.SaveChangesAsync() > 0)
                    {
                        return 0; // Success
                    }
                    else
                    {
                        return 1; // Delete failed
                    }
                }
            }
            return 2; // No entry found
        }

        public static async Task<int> RemoveByGuildAsync(ulong guild)
        {
            using (GeneralContext db = new GeneralContext())
            {
                List<TIgnoreEntry> matches = db.IgnoreList.Where(x => x.Guild.Equals(guild)).ToList();
                if (matches.Count > 0)
                {
                    foreach (TIgnoreEntry match in matches)
                    {
                        db.Remove(match);
                    }
                    if (await db.SaveChangesAsync() > 0)
                    {
                        return 0; // Success
                    }
                    else
                    {
                        return 1; // Delete failed
                    }
                }
            }
            return 2; // No entry found
        }

        public static async Task<int> RemoveBySubjectIdAsync(ulong subjectId)
        {
            using (GeneralContext db = new GeneralContext())
            {
                List<TIgnoreEntry> matches = db.IgnoreList.Where(x => x.SubjectId.Equals(subjectId)).ToList();
                if (matches.Count > 0)
                {
                    foreach (TIgnoreEntry match in matches)
                    {
                        db.Remove(match);
                    }
                    if (await db.SaveChangesAsync() > 0)
                    {
                        return 0; // Success
                    }
                    else
                    {
                        return 1; // Delete failed
                    }
                }
            }
            return 2; // No entry found
        }

        //

        public static bool IsIgnored(ulong guild, ulong subjectId)
        {
            using (GeneralContext db = new GeneralContext())
            {
                if (db.IgnoreList.FirstOrDefault(x => x.Guild.Equals(guild) && x.SubjectId.Equals(subjectId)) != null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
