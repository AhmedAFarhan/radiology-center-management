using System.Linq.Expressions;

namespace RadiologyCenter.BuildingBlocks.Domain.Specifications;

public class DynamicSpecification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>>? Criteria { get; private set; }
    public List<Expression<Func<T, object>>> Includes { get; } = [];
    public List<string> IncludeStrings { get; } = [];
    public Expression<Func<T, object>>? OrderBy { get; private set; }
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }
    public int? Take { get; private set; }
    public int? Skip { get; private set; }
    public bool IsPagingEnabled { get; private set; }

    public DynamicSpecification(Expression<Func<T, bool>>? criteria = null)
    {
        Criteria = criteria;
    }

    public void AddCriteria(Expression<Func<T, bool>> criteria)
    {
        if (Criteria is null)
        {
            Criteria = criteria;
            return;
        }

        var param = Criteria.Parameters[0];
        var other = new ParameterRebinder(param).Visit(criteria.Body);
        Criteria = Expression.Lambda<Func<T, bool>>(
            Expression.AndAlso(Criteria.Body, other), [param]);
    }

    private sealed class ParameterRebinder : ExpressionVisitor
    {
        private readonly ParameterExpression _replacement;

        public ParameterRebinder(ParameterExpression replacement) => _replacement = replacement;

        protected override Expression VisitParameter(ParameterExpression node) =>
            node.Type == _replacement.Type ? _replacement : node;
    }

    public void AddInclude(Expression<Func<T, object>> includeExpression) =>
        Includes.Add(includeExpression);

    public void AddInclude(string includeString) =>
        IncludeStrings.Add(includeString);

    public void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }

    public void ApplyOrderBy(Expression<Func<T, object>> orderByExpression) =>
        OrderBy = orderByExpression;

    public void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression) =>
        OrderByDescending = orderByDescendingExpression;
}
