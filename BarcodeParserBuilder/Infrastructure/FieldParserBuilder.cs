﻿using BarcodeParserBuilder.Exceptions;

namespace BarcodeParserBuilder.Infrastructure
{
    public interface IFieldParserBuilder
    {
        object Parse(object obj, int? minimumLength, int? maximumLength);
        object Parse(string value, int? minimumLength, int? maximumLength);
        string Build(object obj);
    }

    internal abstract class BaseFieldParserBuilder<T> : IFieldParserBuilder
    {
        public object Parse(object obj, int? minimumLength, int? maximumLength) => ValidateAndReTypeObject(obj, minimumLength, maximumLength);
        public object Parse(string value, int? minimumLength, int? maximumLength) => ValidateAndParseString(value, minimumLength, maximumLength);
        public string Build(object obj) => ValidateAndBuildString(obj);

        private T ValidateAndParseString(string value, int? minimumLength, int? maximumLength)
        {
            if (maximumLength.HasValue && (value?.Length ?? 0) > 0 && value.Length > maximumLength)
                throw new ValidateException($"Invalid string value '{value}' : Too large ({value.Length}/{maximumLength.Value}).");

            if (minimumLength.HasValue && (value?.Length ?? 0) > 0 && value.Length < minimumLength)
                throw new ValidateException($"Invalid string value '{value}' : Too small ({value.Length}/{minimumLength.Value}).");

            if (!Validate(value))
                throw new ValidateException($"Failed to validate object (value rejected).");

            if (string.IsNullOrWhiteSpace(value))
                return default;

            return Parse(value);
        }

        private T ValidateAndReTypeObject(object obj, int? minimumLength = null, int? maximumLength = null)
        {
            if (obj == null)
                return default;

            var objType = obj.GetType();
            var resultedType = typeof(T);

            if (!resultedType.IsAssignableFrom(objType) || (T)obj == null)
                throw new ValidateException($"Failed to validate object : received {objType.Name} but expected {resultedType.Name}");

            if(!ValidateObject((T)obj) || 
                ((minimumLength.HasValue || maximumLength.HasValue) && !ValidateObjectLength((T)obj, null, null)))
                throw new ValidateException($"Failed to validate object (value rejected).");

            return (T)obj;
        }

        private string ValidateAndBuildString(object obj)
        {
            if (obj == null)
                return null;

            var input = ValidateAndReTypeObject(obj);
            return Build(input);
        }

        protected abstract bool Validate(string value);
        protected virtual bool ValidateObject(T obj) => true;
        protected virtual bool ValidateObjectLength(T obj, int? minimumLength, int? maximumLength) => true;
        protected abstract T Parse(string value);
        protected abstract string Build(T obj);
    }
}
