#pragma warning disable 4014

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using XayahBot.Database.Model;
using XayahBot.Database.Service;

namespace XayahBot.Command.Remind
{
    public class RemindService
    {
        private static bool _isRunning = false;
        private static SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private static SemaphoreSlim _taskLock = new SemaphoreSlim(1, 1);
        private static Dictionary<int, object> _currentTasks = new Dictionary<int, object>();

        private static async Task Start()
        {
            await _lock.WaitAsync();
            try
            {
                if (!_isRunning)
                {
                    _isRunning = true;
                    Task.Run(() => Run());
                }
            }
            finally
            {
                _lock.Release();
            }
        }

        private static void Run()
        {
            while (_isRunning)
            {
                // every x
                // get list of reminders
                // cycle through list that do not have a current task
                // if expiration in latest x*2 add a task
                // do something, i guess
            }
        }

        //

        private readonly RemindDAO _remindDao = new RemindDAO();

        public async Task AddNew(TRemindEntry entry)
        {
            await this._remindDao.AddAsync(entry);
            //await Start();
        }

        public async Task Remove(int id, ulong userId)
        {
            await this._remindDao.RemoveAsync(id, userId);
            await _taskLock.WaitAsync();
            try
            {
                if (_currentTasks.TryGetValue(id, out object task))
                {
                    // kill task
                    _currentTasks.Remove(id);
                }
            }
            finally
            {
                _taskLock.Release();
            }
        }
    }
}
