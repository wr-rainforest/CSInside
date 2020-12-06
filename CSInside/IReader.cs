using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CSInside
{
    public interface IReader<TResult>
    {
        public int Position { get; set; }

        public int Count { get; }

        public Task<TResult> ReadAsync();
    }
}
