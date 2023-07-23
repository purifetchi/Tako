using Tako.NBT.Serialization;

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
    /// <typeparam name="TTag">The value type.</typeparam>
    /// <param name="name">The name of the value.</param>
    /// <returns>The value or nothing.</returns>
    public TValue Get<TValue>(string name)
    {
        if (!Values.TryGetValue(name, out var tag))
            throw new KeyNotFoundException($"Cannot find key of name {name} within this compound tag.");

        if (tag is TValue maybeTag)
            return maybeTag;

        if (tag.GetValue() is TValue maybeValue)
            return maybeValue;

        throw new ArgumentException($"The type passed to Get doesn't match that of the tag at {name}.");
    }

    /// <summary>
    /// Sets a named value and creates a tag for it.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    public void Set<TValue>(string name, TValue value)
    {
        Tag tag = value switch
        {
            sbyte byteValue => new ByteTag(name) { Value = byteValue },
            short shortValue => new ShortTag(name) { Value = shortValue },
            int intValue => new IntTag(name) { Value = intValue },
            string stringValue => new StringTag(name) { Value = stringValue },
            float floatValue => new FloatTag(name) { Value = floatValue },
            double doubleValue => new DoubleTag(name) { Value = doubleValue },
            sbyte[] byteArrayValue => new ByteArrayTag(name) { Values = byteArrayValue },
            IList<Tag> listValue => new ListTag(name) { Values = listValue },
            Tag tagValue => tagValue,
            _ => throw new ArgumentException($"Type of value passed to Get is not supported.", nameof(value)),
        };
        Values[name] = tag;
    }

    /// <inheritdoc/>
    public override object? GetValue()
    {
        return Values;
    }

    /// <inheritdoc/>
    internal override Tag Parse(NBTReader reader)
    {
        Tag tag;
        do
        {
            tag = reader.Read();
            if (tag.Id != TagId.End)
                Values.Add(tag.Name, tag);
        } while (tag.Id != TagId.End);

        return this;
    }

    /// <inheritdoc/>
    internal override void Serialize(NBTWriter writer)
    {
        foreach (var tag in Values.Values)
            writer.Write(tag);

        writer.WriteNameless(new EndTag());
    }
}
