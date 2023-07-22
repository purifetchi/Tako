namespace Tako.NBT.Tags;

/// <summary>
/// Effectively a list of a named tags. Order is not guaranteed.
/// </summary>
public class CompoundTag : Tag
{
    /// <summary>
    /// The values dictionary.
    /// </summary>
    public IDictionary<string, Tag> Values { get; }

    /// <summary>
    /// Creates a new compound tag.
    /// </summary>
    /// <param name="name">The name.</param>
    public CompoundTag(string name)
        : base(TagId.Compound, name)
    {
        Values = new Dictionary<string, Tag>();
    }

    /// <summary>
    /// Gets the value from the compound tag as a given type.
    /// </summary>
    /// <typeparam name="TTag">The tag type.</typeparam>
    /// <param name="name">The name of the value.</param>
    /// <returns>The value or nothing.</returns>
    public TTag? Get<TTag>(string name)
        where TTag : Tag
    {
        if (!Values.TryGetValue(name, out var tag))
            return null;

        if (tag is not TTag typedTag)
            return null;

        return typedTag;
    }

    /// <inheritdoc/>
    public override object? GetValue()
    {
        return Values;
    }

    /// <inheritdoc/>
    internal override Tag Parse(BinaryReader reader)
    {
        Tag tag;
        do
        {
            tag = ReadTag(reader);
            if (tag.Id != TagId.End)
                Values.Add(tag.Name, tag);
        } while (tag.Id != TagId.End);

        return this;
    }
}
