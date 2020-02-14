namespace Common.System
{
    public class Error
    {
        public Error(string name, string message)
        {
            Ensure.NotNullOrEmpty(name, nameof(name));
            Ensure.NotNullOrEmpty(message, nameof(message));

            Name = name;
            Message = message;
        }

        /// <summary>
        /// Unique error name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Meaningful error description
        /// </summary>
        public string Message { get; }
    }
}
