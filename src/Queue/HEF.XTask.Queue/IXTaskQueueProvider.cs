﻿using System;
using System.Threading.Tasks;

namespace HEF.XTask.Queue
{
    public interface IXTaskQueueProvider
    {
        Task<bool> PushToRunTaskAsync<TParam>(XTask<TParam> toRunTask);

        Task<XTask<TParam>> PopGetToRunTaskWithTimeoutAsync<TParam>(TimeSpan timeout, bool confirmCompleted);

        Task ConfirmToRunTaskCompletedAsync<TParam>(XTask<TParam> toRunTask);
    }
}