using System;
using System.Collections;

namespace Earmark
{
    /// <summary>
    /// Keeps a list of fixed size blocks, allocates the first continuous run of free blocks
    /// </summary>
    public class SlabAllocator
        : IAllocator
    {
        private readonly int _pageSize;
        private readonly int _totalSize;

        private readonly BitArray _allocationMap;

        private int _totalAllocated;

        public SlabAllocator(int pageSize, int pageCount)
        {
            _pageSize = pageSize;
            _totalSize = pageCount * pageSize;

            _allocationMap = new BitArray(pageCount);
        }

        public Allocation Allocate(int amount)
        {
            //Early exit if we know we cannot possibly satisfy this request
            if (amount > (_totalSize - _totalAllocated))
                return null;

            var pagesRequired = (int)Math.Ceiling((float)amount / _pageSize);

            var runLength = 0;
            var runStart = 0;
            for (var i = 0; i < _allocationMap.Count; i++)
            {
                var allocated = _allocationMap[i];

                if (!allocated)
                {
                    if (runLength == 0)
                        runStart = i;
                    runLength++;

                    if (runLength == pagesRequired)
                    {
                        //Mark this run as allocated
                        for (var j = 0; j < runLength; j++)
                            _allocationMap[j + runStart] = true;

                        _totalAllocated += runLength * _pageSize;
                        return new Allocation(this, runStart, runLength * _pageSize);
                    }
                }
                else
                {
                    //Page is not free, reset run length counter back to zero and start search again
                    runLength = 0;
                }
            }

            //Cannot find a run of free pages large enough
            return null;
        }

        public void Free(Allocation handle)
        {
            //Don't free the same handle twice
            if (handle.IsFreed)
                return;

            //Update flags and variables as appropriate
            handle.IsFreed = true;
            _totalAllocated -= handle.Count;

            //Set all these pages to free in the bitmap
            for (var i = 0; i < handle.Count / _pageSize; i++)
                _allocationMap[handle.Offset + i] = false;
        }
    }
}
