using System.Linq.Expressions;

namespace XWingTO.Data
{
	public interface IQuery<T>
	{
		IQuery<T> Where(Expression<Func<T, bool>> filter);

		bool Any(Expression<Func<T, bool>> filter);

		Task<List<T>> ExecuteAsync(CancellationToken cancellationToken = new CancellationToken());

		IQuery<TResult> Select<TResult>(Expression<Func<T, TResult>> selector) where TResult : class;

		IQuery<T> Order(Func<IQueryable<T>, IQueryable<T>> orderBy);

		IQuery<T> Take(int skip, int take);

		IQuery<T> Take(int take);

		Task<T> FirstOrDefault(Expression<Func<T, bool>> filter);

		IQuery<T> Include<TProperty>(Expression<Func<T, TProperty>> include);

		IQuery<T> Distinct();

		IQuery<TResult> SelectMany<TCollection, TResult>(
			Expression<Func<T, IEnumerable<TCollection>>> collectionSelector,
			Expression<Func<T, TCollection, TResult>> resultSelector) where TResult : class;

		IQuery<TResult> Join<TInner, TKey, TResult>(IEnumerable<TInner> inner,
			Expression<Func<T, TKey>> outerKeySelector,
			Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<T, TInner, TResult>> resultSelector)
			where TInner : class where TResult : class;

		IQuery<TResult> GroupJoin<TInner, TKey, TResult>(IQuery<TInner> inner,
			Expression<Func<T, TKey>> outerKeySelector,
			Expression<Func<TInner, TKey>> innerKeySelector,
			Expression<Func<T, IEnumerable<TInner>, TResult>> resultSelector)
			where TInner : class where TResult : class;

		IQuery<IGrouping<TKey, TValue>> GroupBy<TKey, TValue>(Expression<Func<T, TKey>> keySelector,
			Expression<Func<T, TValue>> valueSelector);

		IQuery<IGrouping<TKey, T>> GroupBy<TKey>(Expression<Func<T, TKey>> groupBy);


	}
}
