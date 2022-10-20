using System.Collections;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace XWingTO.Data;

public class Query<T> : IQuery<T>, IEnumerable where T : class
{
	private IQueryable<T> _query;

	

	public Query(IQueryable<T> query)
	{
		_query = query;
	}

	public IEnumerator<T> GetEnumerator()
	{
		return _query.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _query.GetEnumerator();
	}

	public IQuery<T> Where(Expression<Func<T, bool>> filter)
	{
		_query = _query.Where(filter);
		return this;
	}

	public bool Any(Expression<Func<T, bool>> filter)
	{
		return _query.Any(filter);
	}

	public IQuery<T> Order(Func<IQueryable<T>, IQueryable<T>> orderBy)
	{
		_query = orderBy(_query);
		return this;
	}

	public IQuery<T> Include<TProperty>(Expression<Func<T, TProperty>> include)
	{
		_query = _query.Include(include);
		return this;
	}

	public IQuery<T> Take(int skip, int take)
	{
		_query = _query.Skip(skip).Take(take);
		return this;
	}

	public IQuery<T> Take(int take)
	{
		_query = _query.Take(take);
		return this;
	}

	public IQuery<T> Skip(int skip)
	{
		_query = _query.Skip(skip);
		return this;
	}

	public IQuery<TResult> Select<TResult>(Expression<Func<T, TResult>> selector) where TResult : class
	{
		return new Query<TResult>(_query.Select(selector));
	}

	public IQuery<IGrouping<TKey, T>> GroupBy<TKey>(Expression<Func<T, TKey>> groupBy)
	{
		return new Query<IGrouping<TKey, T>>(_query.GroupBy(groupBy));
	}

	public IQuery<IGrouping<TKey, TValue>> GroupBy<TKey, TValue>(Expression<Func<T, TKey>> keySelector, Expression<Func<T, TValue>> valueSelector)
	{
		return new Query<IGrouping<TKey, TValue>>(_query.GroupBy(keySelector, valueSelector));
	}

	public async Task<List<T>> ExecuteAsync(CancellationToken cancellationToken = new CancellationToken())
	{
		return await _query.ToListAsync(cancellationToken);
	}

	public async Task<bool> AnyAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = new CancellationToken())
	{
		return await _query.AnyAsync(filter, cancellationToken);
	}

	public IQuery<TResult> GroupJoin<TInner, TKey, TResult>(IQuery<TInner> inner, Expression<Func<T, TKey>> outerKeySelector,
		Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<T, IEnumerable<TInner>, TResult>> resultSelector) where TInner : class where TResult : class
	{
		var innerQuery = (inner as Query<TInner>)?._query;
		if (innerQuery == null) throw new ArgumentException("Problem in GroupJoin: inner as EntityFrameworkQuery<TInner>");

		return new Query<TResult>(_query.GroupJoin(innerQuery, outerKeySelector, innerKeySelector, resultSelector));
	}

	public IQuery<TResult> Join<TInner, TKey, TResult>(IEnumerable<TInner> inner, Expression<Func<T, TKey>> outerKeySelector,
		Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<T, TInner, TResult>> resultSelector) where TInner : class where TResult : class
	{
		var innerQuery = (inner as Query<TInner>)?._query;
		if (innerQuery == null)
			return new Query<TResult>(_query.Join(inner, outerKeySelector, innerKeySelector, resultSelector));

		return new Query<TResult>(_query.Join(innerQuery, outerKeySelector, innerKeySelector, resultSelector));
	}

	public IQuery<TResult> SelectMany<TCollection, TResult>(Expression<Func<T, IEnumerable<TCollection>>> collectionSelector, Expression<Func<T, TCollection, TResult>> resultSelector) where TResult : class
	{
		return new Query<TResult>(_query.SelectMany(collectionSelector, resultSelector));
	}
	public async Task<T> FirstOrDefault(Expression<Func<T, bool>> filter)
	{
		return await _query.FirstOrDefaultAsync(filter);
	}
	public async Task<int> SumAsync(Expression<Func<T, int>> selector)
	{
		return await _query.SumAsync(selector);
	}

	public IQuery<T> Distinct()
	{
		_query = _query.Distinct();
		return this;
	}

	public IQuery<T> Union(IQuery<T> inner)
	{
		var innerQuery = (inner as Query<T>)?._query;
		if (innerQuery == null) throw new ArgumentException("Problem in GroupJoin: inner as EntityFrameworkQuery<TInner>");

		return new Query<T>(_query.Union(innerQuery));
	}
}