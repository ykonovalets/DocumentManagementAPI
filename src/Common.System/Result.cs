using System;

namespace Common.System
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

    public sealed class Result<T> : Result
    {
        private static readonly Result<T> _cachedEmpty = new Result<T>(default(T));

        private readonly T _value;

        private Result(T value)
        {
            _value = value;
        }

        private Result(Error error) : base(error)
        {
        }

        public T Value
        {
            get
            {
                if (!Successful)
                {
                    throw new InvalidOperationException("Accessed result value of unsuccessful operation.");
                }
                return _value;
            }
        }

        public static implicit operator Result<T>(T value) => Ok(value);

        public static implicit operator Result<T>(Error error) => Fail(error);

        public new static Result<T> Fail(Error error) => new Result<T>(error);

        public static Result<T> Ok(T value) => new Result<T>(value);
    }
}
