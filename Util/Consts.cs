using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public static class Consts
    {
        public static readonly List<AdStatus> ShouldBeRePostedStatuses = new()
        {
            AdStatus.New,
            AdStatus.DeleteFailed,
            AdStatus.PostedFailed,
            AdStatus.ReadFailed,
            AdStatus.ValidateFailed
        };

        public static readonly List<AdStatus> ShouldBeRePostedEvenWhenNotPresentStatuses = new()
        {
            AdStatus.PostedFailed,
            AdStatus.ValidateFailed,
        };
    }
}
