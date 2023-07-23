using Tako.NBT.Serialization;

namespace Tako.NBT.Tags;

/// <summary>
/// An NBT tag.
/// </summary>
public abstract class Tag
{
    /// <summary>
    /// The ID of the tag.
    /// </summary>
    public TagId Id { get; init; }

    /// <summary>
    /// The name of the tag.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Creates a new NBT tag.
    /// </summary>
    /// <param name="tagId">The tag id.</param>
    /// <param name="name">The name.</param>
    public Tag(TagId tagId, string name)
    {
        Id = tagId;
        Name = name;
    }

    /// <summary>
    /// Gets the tag's value (as a C# object).
    /// </summary>
    /// <returns>The tag's value.</returns>
    public abstract object? GetValue();

    /// <summary>
    /// Parses a tag from the NBT reader.
    /// </summary>
    /// <param name="reader">The reader.</param>
    internal abstract Tag Parse(NBTReader reader);

    /// <summary>
    /// Serializes this tag into the NBT writer.
    /// </summary>
    /// <param name="writer">The writer.</param>
    internal abstract void Serialize(NBTWriter writer);
}
