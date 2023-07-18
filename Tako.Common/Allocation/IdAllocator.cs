using System.Numerics;

namespace Tako.Common.Allocation;

/// <summary>
/// An allocator for numeric ids.
/// </summary>
/// <typeparam name="TNumericType">The numeric type.</typeparam>
public class IdAllocator<TNumericType>
	where TNumericType : struct, INumber<TNumericType>
{
	/// <summary>
	/// The last assigned id.
	/// </summary>
	private TNumericType _lastId = TNumericType.Zero;

	/// <summary>
	/// The used ids set.
	/// </summary>
	private HashSet<TNumericType> _usedIds;

	/// <summary>
	/// The free ids stack.
	/// </summary>
	private Stack<TNumericType> _freeIds;

	/// <summary>
	/// The max id we can assign.
	/// </summary>
	private TNumericType _maxId;

	/// <summary>
	/// Creates a new id allocation.
	/// </summary>
	/// <param name="maxId">The max id it can assign.</param>
	public IdAllocator(TNumericType maxId)
	{
		_maxId = maxId;

		_usedIds = new();
		_freeIds = new();
	}

	/// <summary>
	/// Get an id.
	/// </summary>
	/// <returns>The id.</returns>
	public TNumericType GetId()
	{
		if (_freeIds.Count > 0)
			return _freeIds.Pop();

		if (!_usedIds.Contains(_lastId))
		{
			_usedIds.Add(_lastId);
			return _lastId;
		}

		_lastId++;
		
		if (_lastId >= _maxId)
			_lastId = TNumericType.Zero;

		_usedIds.Add(_lastId);
		return _lastId;
	}
	
	/// <summary>
	/// Return an id.
	/// </summary>
	/// <param name="id">The id.</param>
	public void ReleaseId(TNumericType id)
	{
		_freeIds.Push(id);
	}
}
