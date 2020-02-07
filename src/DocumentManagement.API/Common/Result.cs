using DocumentManagement.API.Services;

namespace DocumentManagement.API.Common
{
    public class Result
    {
        private static readonly Result _cachedOk = new Result();

        protected Result()
        {
            Successful = true;
        }

        protected Result(Error error)
        {
            Successful = false;
            Error = error;
        }

        public bool Successful { get; protected set; }

        public Error Error { get; protected set; }

        public static Result Ok() => _cachedOk;

        public static implicit operator Result(Error error) => Fail(error);

        public static Result Fail(Error error) => new Result(error);
    }
}
