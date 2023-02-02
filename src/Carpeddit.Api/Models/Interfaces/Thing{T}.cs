namespace Carpeddit.Api.Models
{
    public abstract class Thing<T>
    {
        public abstract string Id { get; set; }

        public abstract string Name { get; set; }

        public abstract string Kind { get; set; }

        public abstract T Data { get; set; }
    }
}
