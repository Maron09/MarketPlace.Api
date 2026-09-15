namespace Marketplace.Modules.Catalog.Domain
{
    internal sealed class Category
    {
        public Guid Id { get; private set; }
        public string Name { get; private set;} = null!;

        private Category() { }

        private Category(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public static Category Create(string name)
            => new(Guid.NewGuid(), name.Trim());
        
        public void Rename(string name)
            => Name = name.Trim();
    }
}