using System;

namespace Earmark
{
    public class Allocation
        : IDisposable
    {
        private readonly IAllocator _allocator;
        internal bool IsFreed = false;

        public int Offset { get; }
        public int Count { get; }

        public Allocation(IAllocator allocator, int offset, int count)
        {
            _allocator = allocator;

            Offset = offset;
            Count = count;
        }

        ~Allocation()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (!IsFreed)
                _allocator.Free(this);
        }
    }
}
