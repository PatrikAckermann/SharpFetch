using System;
using System.Collections.Generic;
using System.Text;

namespace SharpFetch
{
    public static class SharpFetchExtensions
    {
        public static async Task<TOut> Then<TIn, TOut>(this Task<TIn> task, Func<TIn, TOut> process)
        {
            var result = await task;
            return process(result);
        }

        public static async Task Then<TIn>(this Task<TIn> task, Action<TIn> process)
        {
            var result = await task;
            process(result);
        }
    }
}
