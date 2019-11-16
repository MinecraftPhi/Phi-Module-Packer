using System;
using System.Collections.Generic;
using System.Text;

namespace Phi.Packer.Common.Helpers
{
    public static class Extensions
    {
        public static T? As<T>(this object obj)
            where T : class => obj as T;
    }
}
