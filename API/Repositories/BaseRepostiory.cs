using System.Collections.ObjectModel;
using System.Linq.Expressions;
using API.Entities;
using API.Models.Domain;
using API.Models.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public abstract class BaseRepository<T, TId> where T : class
{
    protected PRN221_DBContext DbContext;

    public BaseRepository(PRN221_DBContext context)
    {
        DbContext = context;
    }

    protected abstract TId GetId(T entity);

    public abstract void PreLoad();
        

    public T? FindOne(TId id)
    {
        if(id == null)
        {
            throw ApiException.Failed("Id should not be null.");
        }
        DbSet<T> context = GetContext();
        PreLoad();
        return context.Find(id);
    }

    public T? FindOne(Expression<Func<T, bool>> whereClause)
    {
        DbSet<T> context = GetContext();
        var entity = context.Where(whereClause).FirstOrDefault();
        return entity;
    }

    public T GetOne(TId id, string message = "Record not found.")
    {
        var entity = FindOne(id);
        if (entity == null)
        {
            throw ApiException.NotFound(message);
        }
        return entity;
    }
    public T GetOne(Expression<Func<T, bool>> whereClause, string message = "Record not found.")
    {
        DbSet<T> context = GetContext();
        var entity = context.Where(whereClause).FirstOrDefault();
        if (entity == null)
        {
            throw ApiException.NotFound(message);
        }
        return entity;
    }
    public bool IsExists(TId id)
    {
        if (id == null)
        {
            return false;
        }
        return FindOne(id) != null;
    }

    public List<T> FindAll()
    {
        DbSet<T> context = GetContext();
        PreLoad();
        return context.ToList();
    }

    public List<T> FindAll(List<TId> ids)
    {
        DbSet<T> context = GetContext();
        HashSet<TId> idSet = ids.ToHashSet();
        PreLoad();
        return context.Where(e => idSet.Contains(GetId(e))).ToList();
    }

    public List<T> FindAll(Expression<Func<T, bool>> whereClause)
    {
        DbSet<T> context = GetContext();
        PreLoad();
        return context.Where(whereClause).ToList();
    }

    public ListResult<T> Search( int page, int size, bool isAscending, string orderBy)
    {
        return Search(page, size, isAscending, orderBy, null);
    }

    public ListResult<T> Search( int page, int size, bool isAscending, string orderBy, Expression<Func<T, bool>>? whereClause)
    {
        Expression<Func<T, object>>? orderExpression = null;
        PreLoad();

        try
        {
            var parameter = Expression.Parameter(typeof(T));
            var property = Expression.Property(parameter, orderBy);
            var propAsObject = Expression.Convert(property, typeof(object));

            orderExpression = Expression.Lambda<Func<T, object>>(propAsObject, parameter);

            return _Search(page, size, isAscending, orderBy, orderExpression, whereClause);
        } catch (Exception e)
        {
            return _Search(page, size, isAscending, null, orderExpression, whereClause);
        }
    }

    private ListResult<T> _Search( int page, int size, bool isAscending, string? orderProp, Expression<Func<T, object>>? orderBy, Expression<Func<T, bool>>? whereClause)
    {
        int total = CountTotalResultFoundUsing(whereClause);
        List<T> content = GetContent(page, size, isAscending, orderBy, whereClause);

        return new ListResult<T>(content, total, page, size, isAscending, orderProp);
    }

    private int CountTotalResultFoundUsing(Expression<Func<T, bool>>? whereClause)
    {
        DbSet<T> context = GetContext();
        bool findAll = whereClause == null;
        int total = findAll ? context.Count() : context.Where(whereClause).Count();
        return total;
    }

    private List<T> GetContent(int page, int size, bool isAscending, Expression<Func<T, object>>? orderBy, Expression<Func<T, bool>>? where)
    {
        if (page < 1)
        {
            page = 1;
        }
        DbSet<T> context = GetContext();

        bool findAll = where == null;
        IQueryable<T> query = findAll ? context : context.Where(where);

        if (orderBy != null)
        {
            query = isAscending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
        }

        int skip = (page - 1) * size;
        List<T> content = query.Skip(skip).Take(size).ToList();
        return content;
    }

    private DbSet<T> GetContext()
    {
        return DbContext.Set<T>();
    }

    public T Add(T entity)
    {
        DbSet<T> context = GetContext();
        context.Add(entity);
        DbContext.SaveChanges();
        return entity;
    }

    public List<T> AddAll(List<T> entities)
    {
        DbSet<T> context = GetContext();
        context.AddRange(entities);
        DbContext.SaveChanges();
        return entities.ToList();
    }

    public T Update(T entity)
    {
        DbSet<T> context = GetContext();
        context.Update(entity);
        DbContext.SaveChanges();
        return entity;
    }

    public List<T> Update(Collection<T> entities)
    {
        DbSet<T> context = GetContext();
        context.UpdateRange(entities);
        DbContext.SaveChanges();
        return entities.ToList();
    }

    public void Delete(TId id)
    {
        DbSet<T> context = GetContext();
        context.Remove(GetOne(id));
        DbContext.SaveChanges();
    }

    public void Delete(T entity)
    {
        DbSet<T> context = GetContext();
        context.Remove(entity);
        DbContext.SaveChanges();
    }

    public void DeleteRange(List<TId> ids)
    {
        DbSet<T> context = GetContext();
        context.RemoveRange(FindAll(ids));
        DbContext.SaveChanges();
    }

    public void DeleteRange(Collection<T> entities)
    {
        DbSet<T> context = GetContext();
        context.RemoveRange(entities);
        DbContext.SaveChanges();
    }

}