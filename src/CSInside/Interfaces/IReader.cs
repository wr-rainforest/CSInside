using System.Threading.Tasks;

namespace CSInside
{
    /// <summary>
    /// 값을 가져오는 API 요청을 정의합니다.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IReader<TResult>
    {
        public int Position { get; }

        public int Count { get; }

        /// <summary>
        /// API 요청을 실행하고 응답을 반환합니다.
        /// </summary>
        /// <returns></returns>
        public Task<TResult> ReadAsync();
    }
}
