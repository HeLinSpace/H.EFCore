using System;
namespace H.EF.Core.Extensions
{
    public static class GuidExtend
    {
        public static string NewGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
