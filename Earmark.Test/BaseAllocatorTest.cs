using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Earmark.Test
{
    public abstract class BaseAllocatorTest
    {
        private readonly IAllocator _allocator;

        protected const int PAGE_SIZE = 128;
        protected const int PAGE_COUNT = 13104;
        protected const int TOTAL_SIZE = PAGE_SIZE * PAGE_COUNT;

        protected BaseAllocatorTest(IAllocator allocator)
        {
            _allocator = allocator;
        }

        [TestMethod]
        public void AssertThat_AllocateLessThanOnePage_AllocatesSufficientSpace()
        {
            var a = _allocator.Allocate(1);

            Assert.IsTrue(a.Count >= 1);
        }

        [TestMethod]
        public void AssertThat_AllocateMoreThanOnePage_AllocatesSufficientSpace()
        {
            var a = _allocator.Allocate(PAGE_SIZE + 1);

            Assert.IsTrue(a.Count >= PAGE_SIZE + 1);
        }

        [TestMethod]
        public void AssertThat_AllocateEntirePool_MakesNextAllocationFail()
        {
            var a = _allocator.Allocate(TOTAL_SIZE);
            var b = _allocator.Allocate(1);

            Assert.IsNotNull(a);
            Assert.IsNull(b);
        }

        [TestMethod]
        public void AssertThat_FreeingSpace_MakesAllocationsStopFailing()
        {
            var a = _allocator.Allocate(TOTAL_SIZE);
            var b = _allocator.Allocate(1);

            _allocator.Free(a);

            var c = _allocator.Allocate(1);

            Assert.IsNotNull(a);
            Assert.IsNull(b);
            Assert.IsNotNull(c);
        }

        [TestMethod]
        public void BenchmarkFragmentationResistance_RandomAllocations()
        {
            Random r = new Random(1000);
            int failures = 0;
            int wasteage = 0;
            int allocated = 0;

            while (failures < 1000)
            {
                var amount = r.Next(20, 100000);

                var alloc = _allocator.Allocate(amount);
                if (alloc == null)
                    failures++;
                else
                {
                    allocated += amount;
                    wasteage += alloc.Count - amount;
                }
            }

            Console.WriteLine("Failed Allocation Count: {0}", failures);
            Console.WriteLine("Wasted Count: {0}", wasteage);
            Console.WriteLine("Wasted Percentage: {0}%", ((float)wasteage / allocated) * 100);
        }
    }
}
