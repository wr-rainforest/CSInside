using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CSInside
{
    /// <summary>
    /// Api 응답을 읽어 TResult로 변환합니다. 
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IReader<TResult>
    {
        public int Position { get; }

        public int Count { get; }

        public Task<TResult> ReadAsync();
    }
}
