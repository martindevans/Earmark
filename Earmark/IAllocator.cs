namespace Earmark
{
    /// <summary>
    /// Common interface for all allocators
    /// </summary>
    public interface IAllocator
    {
        /// <summary>
        /// Allocate a given contiguous block of the resource
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        Allocation Allocate(int amount);

        /// <summary>
        /// Free a previously allocated block of the resource
        /// </summary>
        /// <param name="handle"></param>
        void Free(Allocation handle);
    }
}
