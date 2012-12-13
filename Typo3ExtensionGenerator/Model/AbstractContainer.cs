namespace Typo3ExtensionGenerator.Model {
  /// <summary>
  /// The AbstractContainer has 3 properties:
  /// - A name
  /// - A value
  /// - A collection of children
  /// </summary>
  /// <typeparam name="TCollectionType">The type of the container for the children.</typeparam>
  /// <typeparam name="TMemberType">The type of the value.</typeparam>
  public class AbstractContainer<TCollectionType,TMemberType> where TCollectionType : new() {
    /// <summary>
    /// The children inside this container.
    /// </summary>
    public TCollectionType Children { get; set; }

    /// <summary>
    /// The name of this container.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The value of this container.
    /// </summary>
    public TMemberType Value { get; set; }

    /// <summary>
    /// Constructs an AbstractContainer.
    /// </summary>
    public AbstractContainer() {
      Children = new TCollectionType();
    }
  }
}
