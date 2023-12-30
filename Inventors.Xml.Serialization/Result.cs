using System;
using System.Collections.Generic;
using System.Text;

namespace Inventors.Xml.Serialization
{
    public class Result<TValue, TError>
        where TValue : class
        where TError : class
    {
        private Result(TValue value)
        {
            _value = value;
            _error = null;
        }

        private Result(TError error)
        {
            _value = null;
            _error = error;
        }

        public static implicit operator Result<TValue, TError>(TValue value) => new Result<TValue, TError>(value);
        public static implicit operator Result<TValue, TError>(TError error) => new Result<TValue, TError>(error);
        public static implicit operator TValue(Result<TValue, TError> result) => result.Value;

        public Result<TValue, TError> OnSuccess(Action<TValue> action)
        {
            if (_value is null)
                return this;

            action(_value);
            return this;
        }

        public Result<TValue, TError> OnError(Action<TError> action)
        {
            if (_error is null)
                return this;

            action(_error);
            return this;
        }

        public TValue Value
        {
            get
            {
                if (_value is null)
                    throw new InvalidOperationException("Operation failed");

                return _value;
            }
        }

        private readonly TValue? _value;
        private readonly TError? _error;
    }
}
