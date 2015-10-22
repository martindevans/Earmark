using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Earmark.Test
{
    [TestClass]
    public class SlabAllocatorTest
        : BaseAllocatorTest
    {
        public SlabAllocatorTest()
            : base(new SlabAllocator(PAGE_SIZE, PAGE_COUNT))
        {
        }
    }
}
