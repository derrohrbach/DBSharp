using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSharp
{
    public interface IMergeable<T>
    {
        void MergeChanges(T changes);
    }
}
