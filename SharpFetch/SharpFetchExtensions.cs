namespace SharpFetch
{
    public static class SharpFetchExtensions
    {
        // Sync continuation: .Then(res => res.Json<T>())
        public static async Task<TOut> Then<TIn, TOut>(this Task<TIn> task, Func<TIn, TOut> process)
        {
            var result = await task;
            return process(result);
        }

        // Sync void continuation: .Then(res => Console.WriteLine(res))
        public static async Task Then<TIn>(this Task<TIn> task, Action<TIn> process)
        {
            var result = await task;
            process(result);
        }

        // Async continuation: .Then(async res => await res.JsonAsync<T>())
        public static async Task<TOut> Then<TIn, TOut>(this Task<TIn> task, Func<TIn, Task<TOut>> process)
        {
            var result = await task;
            return await process(result);
        }

        // Async void continuation: .Then(async res => await res.SaveAsync())
        public static async Task Then<TIn>(this Task<TIn> task, Func<TIn, Task> process)
        {
            var result = await task;
            await process(result);
        }

        // Catch: .Catch(ex => Console.WriteLine(ex.Message))
        public static async Task<TIn> Catch<TIn>(this Task<TIn> task, Func<Exception, TIn> handler)
        {
            try
            {
                return await task;
            }
            catch (Exception ex)
            {
                return handler(ex);
            }
        }

        // Catch void: .Catch(ex => logger.LogError(ex, "Request failed"))
        public static async Task Catch<TIn>(this Task<TIn> task, Action<Exception> handler)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                handler(ex);
            }
        }
    }
}
