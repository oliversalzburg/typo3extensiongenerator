namespace Typo3ExtensionGenerator.Model {
  public class AbstractContainer<TCollectionType,TMemberType> where TCollectionType : new() {
    public TCollectionType Children { get; set; }

    public string Name { get; set; }
    public TMemberType Value { get; set; }

    public AbstractContainer() {
      Children = new TCollectionType();
    }
  }
}
